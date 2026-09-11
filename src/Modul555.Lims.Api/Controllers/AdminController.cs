using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modul555.Lims.Application.Catalogs;
using Modul555.Lims.Application.Common;
using Modul555.Lims.Domain.Common;
using Modul555.Lims.Infrastructure.Services;

namespace Modul555.Lims.Api.Controllers;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
[Route("api/admin")]
public sealed class AdminController : ControllerBase
{
    private readonly AdminService _admin;

    public AdminController(AdminService admin) => _admin = admin;

    [HttpGet("users")]
    public Task<PagedResult<UserAccountDto>> Users(
        [FromQuery] PagedQuery q,
        CancellationToken ct
    ) => _admin.Users(q, ct);

    [HttpPost("users")]
    public Task<UserAccountDto> CreateUser(
        [FromBody] UserAccountWriteDto dto,
        CancellationToken ct
    ) => _admin.SaveUserAsync(null, dto, ct);

    [HttpPut("users/{id:guid}")]
    public Task<UserAccountDto> UpdateUser(
        Guid id,
        [FromBody] UserAccountWriteDto dto,
        CancellationToken ct
    ) => _admin.SaveUserAsync(id, dto, ct);

    [HttpPost("competencies")]
    public Task<CompetencyDto> CreateCompetency(
        [FromBody] CompetencyWriteDto dto,
        CancellationToken ct
    ) => _admin.SaveCompetencyAsync(null, dto, ct);

    [HttpPut("competencies/{id:guid}")]
    public Task<CompetencyDto> UpdateCompetency(
        Guid id,
        [FromBody] CompetencyWriteDto dto,
        CancellationToken ct
    ) => _admin.SaveCompetencyAsync(id, dto, ct);

    [HttpDelete("competencies/{id:guid}")]
    public async Task<IActionResult> DeleteCompetency(Guid id, CancellationToken ct) =>
        await _admin.DeleteCompetencyAsync(id, ct) ? NoContent() : NotFound();

    [HttpGet("audit")]
    public Task<PagedResult<AuditLogDto>> Audit([FromQuery] PagedQuery q, CancellationToken ct) =>
        _admin.AuditLog(q, ct);

    [HttpGet("material-batches")]
    public Task<PagedResult<BatchAdminDto>> MaterialBatches(
        [FromQuery] PagedQuery q,
        CancellationToken ct
    ) => _admin.MaterialBatches(q, ct);

    [HttpPut("material-batches/{id:guid}")]
    public async Task<ActionResult<BatchAdminDto>> UpdateMaterial(
        Guid id,
        [FromBody] BatchAdminWriteDto dto,
        CancellationToken ct
    )
    {
        var r = await _admin.UpdateMaterialBatchAsync(id, dto, ct);
        return r is null ? NotFound() : Ok(r);
    }

    [HttpGet("product-batches")]
    public Task<PagedResult<BatchAdminDto>> ProductBatches(
        [FromQuery] PagedQuery q,
        CancellationToken ct
    ) => _admin.ProductBatches(q, ct);

    [HttpPut("product-batches/{id:guid}")]
    public async Task<ActionResult<BatchAdminDto>> UpdateProduct(
        Guid id,
        [FromBody] BatchAdminWriteDto dto,
        CancellationToken ct
    )
    {
        var r = await _admin.UpdateProductBatchAsync(id, dto, ct);
        return r is null ? NotFound() : Ok(r);
    }

    [HttpGet("roles")]
    public IActionResult RolesList() =>
        Ok(Roles.All.Select(r => new { value = r, label = RoleLabel(r) }));

    [HttpPost("demo/clear")]
    public Task<AdminService.DemoDataResult> ClearDemo(CancellationToken ct) =>
        _admin.ClearDemoDataAsync(ct);

    [HttpPost("demo/generate")]
    public Task<AdminService.DemoDataResult> GenerateDemo(CancellationToken ct) =>
        _admin.GenerateDemoDataAsync(ct);

    private static string RoleLabel(string role) =>
        role switch
        {
            Roles.Laborant => "Лаборант",
            Roles.LabHead => "Начальник лаборатории",
            Roles.QualityInspector => "ОТК",
            Roles.Technologist => "Технолог",
            Roles.ElectricalLaborant => "Электролаборант",
            Roles.Admin => "Администратор",
            Roles.Manager => "Менеджер",
            Roles.Tester => "Тестировщик",
            Roles.Developer => "Разработчик",
            _ => role,
        };
}
