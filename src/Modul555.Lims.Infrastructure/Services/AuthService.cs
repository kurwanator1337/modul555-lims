using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Modul555.Lims.Application.Auth;
using Modul555.Lims.Domain.Common;
using Modul555.Lims.Domain.Reference;
using Modul555.Lims.Infrastructure.Persistence;

namespace Modul555.Lims.Infrastructure.Services;

public sealed class AuthService
{
    private readonly LimsDbContext _db;
    private readonly ICurrentUser _user;
    private readonly IHttpClientFactory _httpFactory;
    private readonly IConfiguration _configuration;

    public AuthService(
        LimsDbContext db,
        ICurrentUser user,
        IHttpClientFactory httpFactory,
        IConfiguration configuration
    )
    {
        _db = db;
        _user = user;
        _httpFactory = httpFactory;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> LoginViaKeycloakAsync(
        LoginRequest request,
        CancellationToken ct
    )
    {
        if (
            string.IsNullOrWhiteSpace(request.UserName)
            || string.IsNullOrWhiteSpace(request.Password)
        )
            return null;

        var tokens = await RequestTokenAsync(
            new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["username"] = request.UserName.Trim(),
                ["password"] = request.Password,
                ["scope"] = "openid profile email",
            },
            ct
        );
        if (tokens is null)
            return null;

        var identity = await ResolveIdentityAsync(tokens, ct);
        if (identity is null)
            return null;

        var profile = await SyncProfileAsync(identity.UserName, identity.Roles, ct);
        if (profile is null)
            return null;

        return new LoginResponse
        {
            Token = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            User = profile,
        };
    }

    public async Task<RefreshResponse?> RefreshAsync(string refreshToken, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return null;

        var tokens = await RequestTokenAsync(
            new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken,
            },
            ct
        );
        if (tokens is null)
            return null;

        return new RefreshResponse
        {
            Token = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken ?? refreshToken,
        };
    }

    public async Task<UserProfileDto?> GetOrSyncProfileAsync(CancellationToken ct)
    {
        var userName = _user.UserName?.Trim();
        if (string.IsNullOrWhiteSpace(userName))
            return null;

        var tokenRoles = _user.Roles.Count > 0 ? _user.Roles.ToArray() : new[] { userName };
        return await SyncProfileAsync(userName, tokenRoles, ct);
    }

    private async Task<UserProfileDto?> SyncProfileAsync(
        string userName,
        IReadOnlyList<string> tokenRoles,
        CancellationToken ct
    )
    {
        var account = await _db
            .UserAccounts.Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.UserName == userName, ct);

        Employee employee;
        if (account?.Employee is not null)
        {
            employee = account.Employee;
            if (account.IsLocked)
                return null;

            account.Roles = MergeRoles(account.Roles, tokenRoles);
            employee.UserName = userName;
            employee.IsActive = true;
        }
        else
        {
            employee =
                await _db.Employees.FirstOrDefaultAsync(e => e.UserName == userName, ct)
                ?? await _db.Employees.FirstOrDefaultAsync(e => e.Code == userName, ct);

            if (employee is null)
            {
                var meta = KeycloakUserMeta(userName);
                employee = new Employee
                {
                    Code = userName,
                    Name = meta.FullName,
                    UserName = userName,
                    LastName = meta.LastName,
                    FirstName = meta.FirstName,
                    Position = meta.Position,
                    IsActive = true,
                };
                var lab = await _db.Subdivisions.FirstOrDefaultAsync(s => s.Code == "LAB", ct);
                if (lab is not null)
                    employee.SubdivisionId = lab.Id;
                _db.Employees.Add(employee);
                await _db.SaveChangesAsync(ct);
            }
            else
            {
                employee.UserName = userName;
                employee.IsActive = true;
            }

            if (account is null)
            {
                account = new UserAccount
                {
                    EmployeeId = employee.Id,
                    UserName = userName,
                    PasswordHash = "KEYCLOAK",
                    Roles = MergeRoles("", tokenRoles),
                    IsLocked = false,
                };
                _db.UserAccounts.Add(account);
            }
            else
            {
                account.EmployeeId = employee.Id;
                account.UserName = userName;
                account.Roles = MergeRoles(account.Roles, tokenRoles);
                account.IsLocked = false;
            }
        }

        await _db.SaveChangesAsync(ct);

        var profileRoles = MergeRoles(account.Roles, tokenRoles)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return new UserProfileDto
        {
            Id = employee.Id,
            UserName = userName,
            FullName = employee.FullName,
            Position = employee.Position,
            Roles = profileRoles,
        };
    }

    private async Task<KeycloakTokenResponse?> RequestTokenAsync(
        Dictionary<string, string> form,
        CancellationToken ct
    )
    {
        var authority =
            _configuration["Keycloak:Authority"]
            ?? "http://72.56.252.95:8080/realms/stroy-company";
        var clientId = _configuration["Keycloak:TokenClientId"] ?? "admin-cli";
        form["client_id"] = clientId;

        var http = _httpFactory.CreateClient("keycloak");
        using var content = new FormUrlEncodedContent(form);
        using var resp = await http.PostAsync(
            $"{authority.TrimEnd('/')}/protocol/openid-connect/token",
            content,
            ct
        );
        if (!resp.IsSuccessStatusCode)
            return null;

        return await resp.Content.ReadFromJsonAsync<KeycloakTokenResponse>(
            cancellationToken: ct
        );
    }

    private async Task<ResolvedIdentity?> ResolveIdentityAsync(
        KeycloakTokenResponse tokens,
        CancellationToken ct
    )
    {
        var userName = TryPreferredUsername(tokens.IdToken) ?? TryPreferredUsername(tokens.AccessToken);
        var roles = ExtractRoles(tokens.AccessToken);

        if (string.IsNullOrWhiteSpace(userName))
        {
            var authority =
                _configuration["Keycloak:Authority"]
                ?? "http://72.56.252.95:8080/realms/stroy-company";
            var http = _httpFactory.CreateClient("keycloak");
            using var req = new HttpRequestMessage(
                HttpMethod.Get,
                $"{authority.TrimEnd('/')}/protocol/openid-connect/userinfo"
            );
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
            using var resp = await http.SendAsync(req, ct);
            if (resp.IsSuccessStatusCode)
            {
                await using var stream = await resp.Content.ReadAsStreamAsync(ct);
                using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
                if (
                    doc.RootElement.TryGetProperty("preferred_username", out var un)
                    && un.GetString() is { Length: > 0 } name
                )
                    userName = name;
            }
        }

        if (string.IsNullOrWhiteSpace(userName))
            return null;

        if (roles.Count == 0)
            roles = [userName];

        return new ResolvedIdentity(userName, roles);
    }

    private static string? TryPreferredUsername(string? jwt)
    {
        var payload = ReadJwtPayload(jwt);
        return payload?.TryGetProperty("preferred_username", out var u) == true
            ? u.GetString()
            : null;
    }

    private static List<string> ExtractRoles(string? jwt)
    {
        var roles = new List<string>();
        var payload = ReadJwtPayload(jwt);
        if (payload is null)
            return roles;

        if (
            payload.Value.TryGetProperty("realm_access", out var realm)
            && realm.TryGetProperty("roles", out var arr)
        )
        {
            foreach (var r in arr.EnumerateArray())
                if (r.GetString() is { Length: > 0 } name)
                    roles.Add(name);
        }

        return roles;
    }

    private static JsonElement? ReadJwtPayload(string? jwt)
    {
        if (string.IsNullOrWhiteSpace(jwt))
            return null;
        var parts = jwt.Split('.');
        if (parts.Length < 2)
            return null;
        try
        {
            var payload = parts[1].Replace('-', '+').Replace('_', '/');
            switch (payload.Length % 4)
            {
                case 2:
                    payload += "==";
                    break;
                case 3:
                    payload += "=";
                    break;
            }
            var bytes = Convert.FromBase64String(payload);
            using var doc = JsonDocument.Parse(bytes);
            return doc.RootElement.Clone();
        }
        catch
        {
            return null;
        }
    }

    private static string MergeRoles(string existing, IEnumerable<string> fromToken)
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (
            var r in existing.Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            )
        )
            set.Add(r);
        foreach (var r in fromToken)
            if (!string.IsNullOrWhiteSpace(r))
                set.Add(r);

        return string.Join(',', set.Where(r => Roles.All.Contains(r)));
    }

    private static (string FullName, string LastName, string FirstName, string Position) KeycloakUserMeta(
        string userName
    ) =>
        userName.ToLowerInvariant() switch
        {
            "admin" => ("Администратор системы", "Администратор", "Системы", "Администратор LIMS"),
            "manager" => ("Менеджер", "Менеджер", "Keycloak", "Менеджер"),
            "tester" => ("Тестировщик", "Тестировщик", "Keycloak", "Тестировщик"),
            "developer" => ("Разработчик", "Разработчик", "Keycloak", "Разработчик"),
            _ => (userName, userName, "", "Пользователь Keycloak"),
        };

    private sealed record ResolvedIdentity(string UserName, IReadOnlyList<string> Roles);

    private sealed class KeycloakTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("id_token")]
        public string? IdToken { get; set; }
    }
}
