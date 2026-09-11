using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Modul555.Lims.Infrastructure.Persistence;

namespace Modul555.Lims.Api.Auth;

public sealed class HttpCurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor;

    public HttpCurrentUser(IHttpContextAccessor accessor) => _accessor = accessor;

    public string? UserName =>
        _accessor.HttpContext?.User.FindFirstValue("preferred_username")
        ?? _accessor.HttpContext?.User.FindFirstValue("username")
        ?? _accessor.HttpContext?.User.Identity?.Name;

    public Guid? EmployeeId
    {
        get
        {
            var http = _accessor.HttpContext;
            if (http is null)
                return null;

            if (http.Items.TryGetValue("__employeeId", out var cached) && cached is Guid id)
                return id;

            var claim = http.User.FindFirstValue("employee_id");
            if (Guid.TryParse(claim, out var fromClaim))
            {
                http.Items["__employeeId"] = fromClaim;
                return fromClaim;
            }

            var userName = UserName;
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            var db = http.RequestServices.GetService<LimsDbContext>();
            if (db is null)
                return null;

            var empId = db
                .Employees.AsNoTracking()
                .Where(e => e.UserName == userName)
                .Select(e => (Guid?)e.Id)
                .FirstOrDefault();

            if (empId is Guid resolved)
                http.Items["__employeeId"] = resolved;

            return empId;
        }
    }

    public IReadOnlyList<string> Roles =>
        _accessor.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray()
        ?? [];
}
