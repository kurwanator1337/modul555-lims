using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modul555.Lims.Domain.Common;

namespace Modul555.Lims.Infrastructure.Persistence.Configurations;

internal static class ConfigHelpers
{
    public static void ConfigureReference<T>(EntityTypeBuilder<T> b, string table)
        where T : ReferenceEntity
    {
        b.ToTable(table);
        b.HasKey(x => x.Id);
        b.Property(x => x.Code).HasMaxLength(64).IsRequired();
        b.Property(x => x.Name).HasMaxLength(256).IsRequired();
        b.Property(x => x.Description).HasMaxLength(2000);
        b.HasIndex(x => x.Code).IsUnique();
    }

    public static void ConfigureEntity<T>(EntityTypeBuilder<T> b, string table)
        where T : Entity
    {
        b.ToTable(table);
        b.HasKey(x => x.Id);
    }
}

internal sealed class ProductionLineConfig : IEntityTypeConfiguration<ProductionLine>
{
    public void Configure(EntityTypeBuilder<ProductionLine> b) =>
        ConfigHelpers.ConfigureReference(b, "production_lines");
}

internal sealed class SubdivisionConfig : IEntityTypeConfiguration<Subdivision>
{
    public void Configure(EntityTypeBuilder<Subdivision> b)
    {
        ConfigHelpers.ConfigureReference(b, "subdivisions");
        b.HasOne(x => x.ProductionLine)
            .WithMany()
            .HasForeignKey(x => x.ProductionLineId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class SupplierConfig : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> b)
    {
        ConfigHelpers.ConfigureReference(b, "suppliers");
        b.Property(x => x.Inn).HasMaxLength(12);
        b.Property(x => x.Email).HasMaxLength(200);
        b.Property(x => x.Address).HasMaxLength(500);
    }
}

internal sealed class WarehouseConfig : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> b) =>
        ConfigHelpers.ConfigureReference(b, "warehouses");
}

internal sealed class NomenclatureCategoryConfig : IEntityTypeConfiguration<NomenclatureCategory>
{
    public void Configure(EntityTypeBuilder<NomenclatureCategory> b) =>
        ConfigHelpers.ConfigureReference(b, "nomenclature_categories");
}

internal sealed class NomenclatureConfig : IEntityTypeConfiguration<Nomenclature>
{
    public void Configure(EntityTypeBuilder<Nomenclature> b)
    {
        ConfigHelpers.ConfigureReference(b, "nomenclatures");
        b.Property(x => x.UnitOfMeasure).HasMaxLength(32);
        b.Property(x => x.Grade).HasMaxLength(128);
        b.HasOne(x => x.Category)
            .WithMany(c => c.Items)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.RequirementStandard)
            .WithMany()
            .HasForeignKey(x => x.RequirementStandardId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class StandardDocumentConfig : IEntityTypeConfiguration<StandardDocument>
{
    public void Configure(EntityTypeBuilder<StandardDocument> b) =>
        ConfigHelpers.ConfigureReference(b, "standard_documents");
}

internal sealed class QualityParameterConfig : IEntityTypeConfiguration<QualityParameter>
{
    public void Configure(EntityTypeBuilder<QualityParameter> b)
    {
        ConfigHelpers.ConfigureReference(b, "quality_parameters");
        b.Property(x => x.UnitOfMeasure).HasMaxLength(32);
        b.Property(x => x.AllowedValues).HasMaxLength(1000);
    }
}

internal sealed class MeasuredQuantityConfig : IEntityTypeConfiguration<MeasuredQuantity>
{
    public void Configure(EntityTypeBuilder<MeasuredQuantity> b)
    {
        ConfigHelpers.ConfigureReference(b, "measured_quantities");
        b.Property(x => x.UnitOfMeasure).HasMaxLength(32);
        b.Property(x => x.VariableName).HasMaxLength(64).IsRequired();
    }
}

internal sealed class DefectConfig : IEntityTypeConfiguration<Defect>
{
    public void Configure(EntityTypeBuilder<Defect> b)
    {
        ConfigHelpers.ConfigureReference(b, "defects");
        b.Property(x => x.AppliesTo).HasMaxLength(128);
    }
}

internal sealed class NonconformanceCauseConfig : IEntityTypeConfiguration<NonconformanceCause>
{
    public void Configure(EntityTypeBuilder<NonconformanceCause> b)
    {
        ConfigHelpers.ConfigureReference(b, "nonconformance_causes");
        b.Property(x => x.Group).HasMaxLength(128);
    }
}

internal sealed class TestMethodConfig : IEntityTypeConfiguration<TestMethod>
{
    public void Configure(EntityTypeBuilder<TestMethod> b)
    {
        ConfigHelpers.ConfigureReference(b, "test_methods");
        b.Property(x => x.Formula).HasMaxLength(500);
        b.HasOne(x => x.StandardDocument)
            .WithMany()
            .HasForeignKey(x => x.StandardDocumentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class TestMethodQuantityConfig : IEntityTypeConfiguration<TestMethodQuantity>
{
    public void Configure(EntityTypeBuilder<TestMethodQuantity> b)
    {
        ConfigHelpers.ConfigureEntity(b, "test_method_quantities");
        b.HasOne(x => x.TestMethod)
            .WithMany(m => m.Quantities)
            .HasForeignKey(x => x.TestMethodId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.MeasuredQuantity)
            .WithMany()
            .HasForeignKey(x => x.MeasuredQuantityId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => new { x.TestMethodId, x.MeasuredQuantityId }).IsUnique();
    }
}

internal sealed class TestMethodEquipmentTypeConfig
    : IEntityTypeConfiguration<TestMethodEquipmentType>
{
    public void Configure(EntityTypeBuilder<TestMethodEquipmentType> b)
    {
        ConfigHelpers.ConfigureEntity(b, "test_method_equipment_types");
        b.HasOne(x => x.TestMethod)
            .WithMany(m => m.EquipmentTypes)
            .HasForeignKey(x => x.TestMethodId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.EquipmentType)
            .WithMany(t => t.Methods)
            .HasForeignKey(x => x.EquipmentTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class TestMethodMaterialConfig : IEntityTypeConfiguration<TestMethodMaterial>
{
    public void Configure(EntityTypeBuilder<TestMethodMaterial> b)
    {
        ConfigHelpers.ConfigureEntity(b, "test_method_materials");
        b.HasOne(x => x.TestMethod)
            .WithMany(m => m.Materials)
            .HasForeignKey(x => x.TestMethodId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Nomenclature)
            .WithMany()
            .HasForeignKey(x => x.NomenclatureId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class ControlProgramConfig : IEntityTypeConfiguration<ControlProgram>
{
    public void Configure(EntityTypeBuilder<ControlProgram> b)
    {
        ConfigHelpers.ConfigureReference(b, "control_programs");
        b.HasOne(x => x.Nomenclature)
            .WithMany(n => n.ControlPrograms)
            .HasForeignKey(x => x.NomenclatureId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.ProductionLine)
            .WithMany(l => l.ControlPrograms)
            .HasForeignKey(x => x.ProductionLineId)
            .OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.StandardDocument)
            .WithMany()
            .HasForeignKey(x => x.StandardDocumentId)
            .OnDelete(DeleteBehavior.SetNull);
        b.HasIndex(x => new
            {
                x.NomenclatureId,
                x.ControlKind,
                x.ProductionLineId,
            })
            .IsUnique();
    }
}

internal sealed class ControlProgramParameterConfig
    : IEntityTypeConfiguration<ControlProgramParameter>
{
    public void Configure(EntityTypeBuilder<ControlProgramParameter> b)
    {
        ConfigHelpers.ConfigureEntity(b, "control_program_parameters");
        b.Property(x => x.NormText).HasMaxLength(500);
        b.HasOne(x => x.ControlProgram)
            .WithMany(p => p.Parameters)
            .HasForeignKey(x => x.ControlProgramId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.QualityParameter)
            .WithMany()
            .HasForeignKey(x => x.QualityParameterId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.TestMethod)
            .WithMany()
            .HasForeignKey(x => x.TestMethodId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class EquipmentTypeConfig : IEntityTypeConfiguration<EquipmentType>
{
    public void Configure(EntityTypeBuilder<EquipmentType> b) =>
        ConfigHelpers.ConfigureReference(b, "equipment_types");
}

internal sealed class EquipmentConfig : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> b)
    {
        ConfigHelpers.ConfigureReference(b, "equipment");
        b.Property(x => x.InventoryNumber).HasMaxLength(64);
        b.Property(x => x.SerialNumber).HasMaxLength(64);
        b.Property(x => x.Manufacturer).HasMaxLength(200);
        b.HasOne(x => x.EquipmentType)
            .WithMany(t => t.Items)
            .HasForeignKey(x => x.EquipmentTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.ResponsibleEmployee)
            .WithMany()
            .HasForeignKey(x => x.ResponsibleEmployeeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class EquipmentEventConfig : IEntityTypeConfiguration<EquipmentEvent>
{
    public void Configure(EntityTypeBuilder<EquipmentEvent> b)
    {
        ConfigHelpers.ConfigureEntity(b, "equipment_events");
        b.Property(x => x.CertificateNumber).HasMaxLength(128);
        b.Property(x => x.Organization).HasMaxLength(256);
        b.Property(x => x.Comment).HasMaxLength(1000);
        b.HasOne(x => x.Equipment)
            .WithMany(e => e.Events)
            .HasForeignKey(x => x.EquipmentId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.PerformedBy)
            .WithMany()
            .HasForeignKey(x => x.PerformedById)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class EmployeeConfig : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> b)
    {
        ConfigHelpers.ConfigureReference(b, "employees");
        b.Ignore(x => x.FullName);
        b.Property(x => x.UserName).HasMaxLength(64).IsRequired();
        b.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        b.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        b.Property(x => x.MiddleName).HasMaxLength(100);
        b.Property(x => x.Position).HasMaxLength(200);
        b.HasIndex(x => x.UserName).IsUnique();
        b.HasOne(x => x.Subdivision)
            .WithMany(s => s.Employees)
            .HasForeignKey(x => x.SubdivisionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class CompetencyConfig : IEntityTypeConfiguration<Competency>
{
    public void Configure(EntityTypeBuilder<Competency> b)
    {
        ConfigHelpers.ConfigureEntity(b, "competencies");
        b.Property(x => x.CertificateNumber).HasMaxLength(64);
        b.HasOne(x => x.Employee)
            .WithMany(e => e.Competencies)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.TestMethod)
            .WithMany(m => m.Competencies)
            .HasForeignKey(x => x.TestMethodId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => new { x.EmployeeId, x.TestMethodId }).IsUnique();
    }
}

internal sealed class UserAccountConfig : IEntityTypeConfiguration<UserAccount>
{
    public void Configure(EntityTypeBuilder<UserAccount> b)
    {
        ConfigHelpers.ConfigureEntity(b, "user_accounts");
        b.Property(x => x.UserName).HasMaxLength(64).IsRequired();
        b.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
        b.Property(x => x.Roles).HasMaxLength(400);
        b.HasIndex(x => x.UserName).IsUnique();
        b.HasOne(x => x.Employee)
            .WithMany(e => e.Accounts)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class DocumentTemplateConfig : IEntityTypeConfiguration<DocumentTemplate>
{
    public void Configure(EntityTypeBuilder<DocumentTemplate> b)
    {
        ConfigHelpers.ConfigureReference(b, "document_templates");
        b.Property(x => x.ProductionLineCode).HasMaxLength(16);
        b.Property(x => x.PerformerRole).HasMaxLength(64);
        b.Property(x => x.ApproverRole).HasMaxLength(64);
        b.HasIndex(x => x.Kind).IsUnique();
    }
}
