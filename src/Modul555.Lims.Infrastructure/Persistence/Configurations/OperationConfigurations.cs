using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modul555.Lims.Domain.Common;

namespace Modul555.Lims.Infrastructure.Persistence.Configurations;

internal sealed class MaterialBatchConfig : IEntityTypeConfiguration<MaterialBatch>
{
    public void Configure(EntityTypeBuilder<MaterialBatch> b)
    {
        ConfigHelpers.ConfigureEntity(b, "material_batches");
        b.Property(x => x.Number).HasMaxLength(64).IsRequired();
        b.Property(x => x.QuantityUnit).HasMaxLength(32);
        b.Property(x => x.SupplierDocumentNumber).HasMaxLength(128);
        b.Property(x => x.DecisionComment).HasMaxLength(1000);
        b.HasIndex(x => x.Number).IsUnique();
        b.HasOne(x => x.Nomenclature)
            .WithMany()
            .HasForeignKey(x => x.NomenclatureId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Supplier)
            .WithMany(s => s.Batches)
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class ProductBatchConfig : IEntityTypeConfiguration<ProductBatch>
{
    public void Configure(EntityTypeBuilder<ProductBatch> b)
    {
        ConfigHelpers.ConfigureEntity(b, "product_batches");
        b.Property(x => x.Number).HasMaxLength(64).IsRequired();
        b.HasIndex(x => x.Number).IsUnique();
        b.HasOne(x => x.Nomenclature)
            .WithMany()
            .HasForeignKey(x => x.NomenclatureId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.ProductionLine)
            .WithMany()
            .HasForeignKey(x => x.ProductionLineId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class ProductUnitConfig : IEntityTypeConfiguration<ProductUnit>
{
    public void Configure(EntityTypeBuilder<ProductUnit> b)
    {
        ConfigHelpers.ConfigureEntity(b, "product_units");
        b.Property(x => x.SerialNumber).HasMaxLength(64).IsRequired();
        b.HasIndex(x => x.SerialNumber).IsUnique();
        b.HasOne(x => x.ProductBatch)
            .WithMany(p => p.Units)
            .HasForeignKey(x => x.ProductBatchId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class ControlOrderConfig : IEntityTypeConfiguration<ControlOrder>
{
    public void Configure(EntityTypeBuilder<ControlOrder> b)
    {
        ConfigHelpers.ConfigureEntity(b, "control_orders");
        b.Property(x => x.Number).HasMaxLength(64).IsRequired();
        b.Property(x => x.Comment).HasMaxLength(1000);
        b.HasIndex(x => x.Number).IsUnique();
        b.HasOne(x => x.MaterialBatch)
            .WithMany(m => m.ControlOrders)
            .HasForeignKey(x => x.MaterialBatchId)
            .OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.ProductBatch)
            .WithMany(p => p.ControlOrders)
            .HasForeignKey(x => x.ProductBatchId)
            .OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.ProductUnit)
            .WithMany()
            .HasForeignKey(x => x.ProductUnitId)
            .OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.ControlProgram)
            .WithMany()
            .HasForeignKey(x => x.ControlProgramId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.ResponsibleEmployee)
            .WithMany()
            .HasForeignKey(x => x.ResponsibleEmployeeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class SampleConfig : IEntityTypeConfiguration<Sample>
{
    public void Configure(EntityTypeBuilder<Sample> b)
    {
        ConfigHelpers.ConfigureEntity(b, "samples");
        b.Property(x => x.Number).HasMaxLength(64).IsRequired();
        b.Property(x => x.Barcode).HasMaxLength(64).IsRequired();
        b.Property(x => x.QuantityUnit).HasMaxLength(32);
        b.HasIndex(x => x.Number).IsUnique();
        b.HasIndex(x => x.Barcode).IsUnique();
        b.HasOne(x => x.ControlOrder)
            .WithMany(o => o.Samples)
            .HasForeignKey(x => x.ControlOrderId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.SampledBy)
            .WithMany()
            .HasForeignKey(x => x.SampledById)
            .OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class SampleStateHistoryConfig : IEntityTypeConfiguration<SampleStateHistory>
{
    public void Configure(EntityTypeBuilder<SampleStateHistory> b)
    {
        ConfigHelpers.ConfigureEntity(b, "sample_state_history");
        b.Property(x => x.Comment).HasMaxLength(500);
        b.HasOne(x => x.Sample)
            .WithMany(s => s.History)
            .HasForeignKey(x => x.SampleId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.ChangedBy)
            .WithMany()
            .HasForeignKey(x => x.ChangedById)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class TestRunConfig : IEntityTypeConfiguration<TestRun>
{
    public void Configure(EntityTypeBuilder<TestRun> b)
    {
        ConfigHelpers.ConfigureEntity(b, "test_runs");
        b.HasOne(x => x.ControlOrder)
            .WithMany(o => o.TestRuns)
            .HasForeignKey(x => x.ControlOrderId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Sample)
            .WithMany(s => s.TestRuns)
            .HasForeignKey(x => x.SampleId)
            .OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.ControlProgramParameter)
            .WithMany()
            .HasForeignKey(x => x.ControlProgramParameterId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Assignee)
            .WithMany()
            .HasForeignKey(x => x.AssigneeId)
            .OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.Equipment)
            .WithMany()
            .HasForeignKey(x => x.EquipmentId)
            .OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.Result)
            .WithOne(r => r.TestRun)
            .HasForeignKey<ParameterResult>(r => r.TestRunId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class MeasurementConfig : IEntityTypeConfiguration<Measurement>
{
    public void Configure(EntityTypeBuilder<Measurement> b)
    {
        ConfigHelpers.ConfigureEntity(b, "measurements");
        b.Property(x => x.TextValue).HasMaxLength(500);
        b.HasOne(x => x.TestRun)
            .WithMany(t => t.Measurements)
            .HasForeignKey(x => x.TestRunId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.MeasuredQuantity)
            .WithMany()
            .HasForeignKey(x => x.MeasuredQuantityId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.RecordedBy)
            .WithMany()
            .HasForeignKey(x => x.RecordedById)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class ParameterResultConfig : IEntityTypeConfiguration<ParameterResult>
{
    public void Configure(EntityTypeBuilder<ParameterResult> b)
    {
        ConfigHelpers.ConfigureEntity(b, "parameter_results");
        b.Property(x => x.TextValue).HasMaxLength(500);
        b.Property(x => x.NormSnapshot).HasMaxLength(500);
        b.HasIndex(x => x.TestRunId).IsUnique();
    }
}

internal sealed class ProtocolConfig : IEntityTypeConfiguration<Protocol>
{
    public void Configure(EntityTypeBuilder<Protocol> b)
    {
        ConfigHelpers.ConfigureEntity(b, "protocols");
        b.Property(x => x.Number).HasMaxLength(64).IsRequired();
        b.Property(x => x.ResultsSnapshot).HasColumnType("text");
        b.Property(x => x.Conclusion).HasMaxLength(2000);
        b.HasIndex(x => x.Number).IsUnique();
        b.HasOne(x => x.Template)
            .WithMany()
            .HasForeignKey(x => x.TemplateId)
            .OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.ControlOrder)
            .WithMany(o => o.Protocols)
            .HasForeignKey(x => x.ControlOrderId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class ProtocolSignatureConfig : IEntityTypeConfiguration<ProtocolSignature>
{
    public void Configure(EntityTypeBuilder<ProtocolSignature> b)
    {
        ConfigHelpers.ConfigureEntity(b, "protocol_signatures");
        b.HasOne(x => x.Protocol)
            .WithMany(p => p.Signatures)
            .HasForeignKey(x => x.ProtocolId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => new
            {
                x.ProtocolId,
                x.Role,
                x.EmployeeId,
            })
            .IsUnique();
    }
}

internal sealed class NonconformanceConfig : IEntityTypeConfiguration<Nonconformance>
{
    public void Configure(EntityTypeBuilder<Nonconformance> b)
    {
        ConfigHelpers.ConfigureEntity(b, "nonconformances");
        b.Property(x => x.Number).HasMaxLength(64).IsRequired();
        b.Property(x => x.ParameterName).HasMaxLength(256);
        b.Property(x => x.ActualValue).HasMaxLength(128);
        b.Property(x => x.NormValue).HasMaxLength(256);
        b.Property(x => x.CauseComment).HasMaxLength(1000);
        b.Property(x => x.DecisionComment).HasMaxLength(1000);
        b.HasIndex(x => x.Number).IsUnique();
        b.HasOne(x => x.ControlOrder)
            .WithMany()
            .HasForeignKey(x => x.ControlOrderId)
            .OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.Protocol)
            .WithMany()
            .HasForeignKey(x => x.ProtocolId)
            .OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.MaterialBatch)
            .WithMany()
            .HasForeignKey(x => x.MaterialBatchId)
            .OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.ProductBatch)
            .WithMany()
            .HasForeignKey(x => x.ProductBatchId)
            .OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.DetectedBy)
            .WithMany()
            .HasForeignKey(x => x.DetectedById)
            .OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.Cause)
            .WithMany()
            .HasForeignKey(x => x.CauseId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class CorrectiveActionConfig : IEntityTypeConfiguration<CorrectiveAction>
{
    public void Configure(EntityTypeBuilder<CorrectiveAction> b)
    {
        ConfigHelpers.ConfigureEntity(b, "corrective_actions");
        b.Property(x => x.Description).HasMaxLength(2000);
        b.Property(x => x.Result).HasMaxLength(1000);
        b.HasOne(x => x.Nonconformance)
            .WithMany(n => n.Actions)
            .HasForeignKey(x => x.NonconformanceId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.AssignedTo)
            .WithMany()
            .HasForeignKey(x => x.AssignedToId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class TraceabilityLinkConfig : IEntityTypeConfiguration<TraceabilityLink>
{
    public void Configure(EntityTypeBuilder<TraceabilityLink> b)
    {
        ConfigHelpers.ConfigureEntity(b, "traceability_links");
        b.HasOne(x => x.ProductBatch)
            .WithMany(p => p.MaterialLinks)
            .HasForeignKey(x => x.ProductBatchId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.MaterialBatch)
            .WithMany(m => m.ProductLinks)
            .HasForeignKey(x => x.MaterialBatchId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => new { x.ProductBatchId, x.MaterialBatchId }).IsUnique();
    }
}

internal sealed class AuditLogConfig : IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> b)
    {
        b.ToTable("audit_log");
        b.HasKey(x => x.Id);
        b.Property(x => x.UserName).HasMaxLength(64);
        b.Property(x => x.EntityType).HasMaxLength(128);
        b.Property(x => x.EntityId).HasMaxLength(64);
        b.Property(x => x.Action).HasMaxLength(16);
        b.Property(x => x.Changes).HasColumnType("text");
        b.HasIndex(x => new { x.EntityType, x.EntityId });
        b.HasIndex(x => x.OccurredAt);
    }
}
