using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Modul555.Lims.Domain.Common;

namespace Modul555.Lims.Infrastructure.Persistence;

public class LimsDbContext : DbContext
{
    private readonly ICurrentUser? _currentUser;

    public LimsDbContext(DbContextOptions<LimsDbContext> options, ICurrentUser? currentUser = null)
        : base(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<ProductionLine> ProductionLines => Set<ProductionLine>();
    public DbSet<Subdivision> Subdivisions => Set<Subdivision>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<NomenclatureCategory> NomenclatureCategories => Set<NomenclatureCategory>();
    public DbSet<Nomenclature> Nomenclatures => Set<Nomenclature>();
    public DbSet<StandardDocument> StandardDocuments => Set<StandardDocument>();
    public DbSet<QualityParameter> QualityParameters => Set<QualityParameter>();
    public DbSet<MeasuredQuantity> MeasuredQuantities => Set<MeasuredQuantity>();
    public DbSet<Defect> Defects => Set<Defect>();
    public DbSet<NonconformanceCause> NonconformanceCauses => Set<NonconformanceCause>();
    public DbSet<TestMethod> TestMethods => Set<TestMethod>();
    public DbSet<TestMethodQuantity> TestMethodQuantities => Set<TestMethodQuantity>();
    public DbSet<TestMethodEquipmentType> TestMethodEquipmentTypes =>
        Set<TestMethodEquipmentType>();
    public DbSet<TestMethodMaterial> TestMethodMaterials => Set<TestMethodMaterial>();
    public DbSet<ControlProgram> ControlPrograms => Set<ControlProgram>();
    public DbSet<ControlProgramParameter> ControlProgramParameters =>
        Set<ControlProgramParameter>();
    public DbSet<EquipmentType> EquipmentTypes => Set<EquipmentType>();
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<EquipmentEvent> EquipmentEvents => Set<EquipmentEvent>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Competency> Competencies => Set<Competency>();
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<DocumentTemplate> DocumentTemplates => Set<DocumentTemplate>();

    public DbSet<MaterialBatch> MaterialBatches => Set<MaterialBatch>();
    public DbSet<ProductBatch> ProductBatches => Set<ProductBatch>();
    public DbSet<ProductUnit> ProductUnits => Set<ProductUnit>();
    public DbSet<ControlOrder> ControlOrders => Set<ControlOrder>();
    public DbSet<Sample> Samples => Set<Sample>();
    public DbSet<SampleStateHistory> SampleStateHistory => Set<SampleStateHistory>();
    public DbSet<TestRun> TestRuns => Set<TestRun>();
    public DbSet<Measurement> Measurements => Set<Measurement>();
    public DbSet<ParameterResult> ParameterResults => Set<ParameterResult>();
    public DbSet<Protocol> Protocols => Set<Protocol>();
    public DbSet<ProtocolSignature> ProtocolSignatures => Set<ProtocolSignature>();
    public DbSet<Nonconformance> Nonconformances => Set<Nonconformance>();
    public DbSet<CorrectiveAction> CorrectiveActions => Set<CorrectiveAction>();
    public DbSet<TraceabilityLink> TraceabilityLinks => Set<TraceabilityLink>();
    public DbSet<AuditLogEntry> AuditLog => Set<AuditLogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgcrypto");
        ApplyConventions(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LimsDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampAuditFields();
        WriteAuditLog();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void StampAuditFields()
    {
        var now = DateTimeOffset.UtcNow;
        var user = _currentUser?.UserName;
        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy ??= user;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = user;
            }
        }
    }

    private void WriteAuditLog()
    {
        var user = _currentUser?.UserName ?? "system";
        var now = DateTimeOffset.UtcNow;
        var entries = ChangeTracker
            .Entries()
            .Where(e =>
                e.Entity is not AuditLogEntry
                && e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted
            )
            .ToList();

        foreach (var entry in entries)
        {
            var id =
                entry
                    .Properties.FirstOrDefault(p => p.Metadata.Name == "Id")
                    ?.CurrentValue?.ToString()
                ?? string.Empty;
            AuditLog.Add(
                new AuditLogEntry
                {
                    OccurredAt = now,
                    UserName = user,
                    EntityType = entry.Metadata.ClrType.Name,
                    EntityId = id,
                    Action = entry.State.ToString(),
                    Changes = SerializeChanges(entry),
                }
            );
        }
    }

    private static string? SerializeChanges(EntityEntry entry)
    {
        if (entry.State == EntityState.Added)
            return null;

        var changes = entry
            .Properties.Where(p => p.IsModified || entry.State == EntityState.Deleted)
            .Select(p => new
            {
                p.Metadata.Name,
                Old = p.OriginalValue,
                New = entry.State == EntityState.Deleted ? null : p.CurrentValue,
            })
            .ToList();

        return changes.Count == 0 ? null : JsonSerializer.Serialize(changes);
    }

    private static void ApplyConventions(ModelBuilder modelBuilder)
    {
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            v => v.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            v => DateOnly.FromDateTime(v)
        );
        var dateOnlyNullableConverter = new ValueConverter<DateOnly?, DateTime?>(
            v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc) : null,
            v =>
                v.HasValue
                    ? DateOnly.FromDateTime(DateTime.SpecifyKind(v.Value, DateTimeKind.Utc))
                    : null
        );

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(string) && property.GetMaxLength() is null)
                    property.SetMaxLength(400);

                if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                {
                    property.SetPrecision(18);
                    property.SetScale(4);
                }

                if (property.ClrType == typeof(DateOnly))
                    property.SetValueConverter(dateOnlyConverter);
                if (property.ClrType == typeof(DateOnly?))
                    property.SetValueConverter(dateOnlyNullableConverter);
            }
        }
    }
}

public interface ICurrentUser
{
    string? UserName { get; }
    Guid? EmployeeId { get; }
    IReadOnlyList<string> Roles { get; }
}
