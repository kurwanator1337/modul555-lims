using Microsoft.EntityFrameworkCore;
using Modul555.Lims.Application.Catalogs;
using Modul555.Lims.Infrastructure.Persistence;

namespace Modul555.Lims.Infrastructure.Services;

public sealed class MetrologyService
{
    private readonly LimsDbContext _db;

    public MetrologyService(LimsDbContext db) => _db = db;

    /// <summary>
    /// Переводит СИ с истёкшей поверкой в статус «непригоден» и возвращает приборы,
    /// у которых до окончания поверки осталось не более 30 дней.
    /// </summary>
    public async Task<IReadOnlyList<EquipmentDto>> RefreshAndRemind(CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var expired = await _db
            .Equipment.Where(e =>
                e.Status == EquipmentStatus.Operational
                && e.VerificationValidUntil != null
                && e.VerificationValidUntil < today
            )
            .ToListAsync(ct);
        foreach (var item in expired)
            item.Status = EquipmentStatus.VerificationExpired;
        if (expired.Count > 0)
            await _db.SaveChangesAsync(ct);

        var soon = today.AddDays(30);
        var due = await _db
            .Equipment.Include(e => e.EquipmentType)
            .Where(e =>
                e.VerificationValidUntil != null
                && e.VerificationValidUntil >= today
                && e.VerificationValidUntil <= soon
            )
            .OrderBy(e => e.VerificationValidUntil)
            .ToListAsync(ct);

        return due.Select(e => new EquipmentDto
            {
                Id = e.Id,
                Code = e.Code,
                Name = e.Name,
                EquipmentTypeId = e.EquipmentTypeId,
                EquipmentTypeName = e.EquipmentType?.Name,
                InventoryNumber = e.InventoryNumber,
                Status = e.Status,
                VerificationValidUntil = e.VerificationValidUntil,
                DaysToVerification = e.VerificationValidUntil!.Value.DayNumber - today.DayNumber,
            })
            .ToList();
    }

    public async Task<EquipmentEventDto> RegisterEvent(EquipmentEventDto dto, CancellationToken ct)
    {
        var equipment =
            await _db.Equipment.FindAsync([dto.EquipmentId], ct)
            ?? throw new InvalidOperationException("Средство измерения не найдено.");
        var ev = new EquipmentEvent
        {
            EquipmentId = dto.EquipmentId,
            Kind = dto.Kind,
            EventDate = dto.EventDate,
            ValidUntil = dto.ValidUntil,
            CertificateNumber = dto.CertificateNumber,
            Organization = dto.Organization,
            Comment = dto.Comment,
        };
        _db.EquipmentEvents.Add(ev);

        if (dto.Kind is EquipmentEventKind.Verification or EquipmentEventKind.Calibration)
        {
            equipment.VerificationValidUntil =
                dto.ValidUntil ?? dto.EventDate.AddMonths(equipment.VerificationIntervalMonths);
            equipment.Status = EquipmentStatus.Operational;
        }
        else if (dto.Kind == EquipmentEventKind.Repair)
            equipment.Status = EquipmentStatus.UnderRepair;
        else if (dto.Kind == EquipmentEventKind.Maintenance)
            equipment.Status = EquipmentStatus.UnderMaintenance;
        else if (dto.Kind == EquipmentEventKind.Decommissioning)
            equipment.Status = EquipmentStatus.Decommissioned;

        await _db.SaveChangesAsync(ct);
        dto.Id = ev.Id;
        return dto;
    }

    public Task<List<EquipmentEventDto>> Events(Guid equipmentId, CancellationToken ct) =>
        _db
            .EquipmentEvents.Where(e => e.EquipmentId == equipmentId)
            .OrderByDescending(e => e.EventDate)
            .Select(e => new EquipmentEventDto
            {
                Id = e.Id,
                EquipmentId = e.EquipmentId,
                Kind = e.Kind,
                EventDate = e.EventDate,
                ValidUntil = e.ValidUntil,
                CertificateNumber = e.CertificateNumber,
                Organization = e.Organization,
                Comment = e.Comment,
            })
            .ToListAsync(ct);
}
