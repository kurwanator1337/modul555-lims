using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Modul555.Lims.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase().Annotation("Npgsql:PostgresExtension:pgcrypto", ",,");

            migrationBuilder.CreateTable(
                name: "audit_log",
                columns: table => new
                {
                    Id = table
                        .Column<long>(type: "bigint", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    OccurredAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    UserName = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    EntityType = table.Column<string>(
                        type: "character varying(128)",
                        maxLength: 128,
                        nullable: false
                    ),
                    EntityId = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Action = table.Column<string>(
                        type: "character varying(16)",
                        maxLength: 16,
                        nullable: false
                    ),
                    Changes = table.Column<string>(type: "text", maxLength: 400, nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_log", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "defects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsCritical = table.Column<bool>(type: "boolean", nullable: false),
                    AppliesTo = table.Column<string>(
                        type: "character varying(128)",
                        maxLength: 128,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    Code = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Name = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "character varying(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_defects", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "document_templates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    Stage = table.Column<int>(type: "integer", nullable: false),
                    ProductionLineCode = table.Column<string>(
                        type: "character varying(16)",
                        maxLength: 16,
                        nullable: true
                    ),
                    PerformerRole = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    ApproverRole = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: true
                    ),
                    RequiresQualityInspector = table.Column<bool>(type: "boolean", nullable: false),
                    SignatureKind = table.Column<int>(type: "integer", nullable: false),
                    CreatedOnNonconformance = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    Code = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Name = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "character varying(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document_templates", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "equipment_types",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    Code = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Name = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "character varying(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipment_types", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "measured_quantities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UnitOfMeasure = table.Column<string>(
                        type: "character varying(32)",
                        maxLength: 32,
                        nullable: true
                    ),
                    VariableName = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Precision = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    Code = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Name = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "character varying(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_measured_quantities", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "nomenclature_categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsRawMaterial = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    Code = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Name = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "character varying(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nomenclature_categories", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "nonconformance_causes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Group = table.Column<string>(
                        type: "character varying(128)",
                        maxLength: 128,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    Code = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Name = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "character varying(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nonconformance_causes", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "production_lines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    Code = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Name = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "character varying(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_production_lines", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "quality_parameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ValueKind = table.Column<int>(type: "integer", nullable: false),
                    UnitOfMeasure = table.Column<string>(
                        type: "character varying(32)",
                        maxLength: 32,
                        nullable: true
                    ),
                    Precision = table.Column<int>(type: "integer", nullable: false),
                    AllowedValues = table.Column<string>(
                        type: "character varying(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    Code = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Name = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "character varying(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quality_parameters", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "standard_documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    Code = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Name = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "character varying(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_standard_documents", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "suppliers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Inn = table.Column<string>(
                        type: "character varying(12)",
                        maxLength: 12,
                        nullable: true
                    ),
                    ContactPerson = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    Phone = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    Email = table.Column<string>(
                        type: "character varying(200)",
                        maxLength: 200,
                        nullable: true
                    ),
                    Address = table.Column<string>(
                        type: "character varying(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    Code = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Name = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "character varying(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suppliers", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "warehouses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsQuarantineZone = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    Code = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Name = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "character varying(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehouses", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "subdivisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionLineId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    Code = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Name = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "character varying(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subdivisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_subdivisions_production_lines_ProductionLineId",
                        column: x => x.ProductionLineId,
                        principalTable: "production_lines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "nomenclatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    UnitOfMeasure = table.Column<string>(
                        type: "character varying(32)",
                        maxLength: 32,
                        nullable: false
                    ),
                    Grade = table.Column<string>(
                        type: "character varying(128)",
                        maxLength: 128,
                        nullable: true
                    ),
                    RequirementStandardId = table.Column<Guid>(type: "uuid", nullable: true),
                    TrackBatches = table.Column<bool>(type: "boolean", nullable: false),
                    ShelfLifeDays = table.Column<int>(type: "integer", nullable: true),
                    DesignValue = table.Column<decimal>(
                        type: "numeric(18,4)",
                        precision: 18,
                        scale: 4,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    Code = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Name = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "character varying(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nomenclatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_nomenclatures_nomenclature_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "nomenclature_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_nomenclatures_standard_documents_RequirementStandardId",
                        column: x => x.RequirementStandardId,
                        principalTable: "standard_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "test_methods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StandardDocumentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Formula = table.Column<string>(
                        type: "character varying(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    IsDestructive = table.Column<bool>(type: "boolean", nullable: false),
                    LaborMinutes = table.Column<int>(type: "integer", nullable: false),
                    DefaultReplicates = table.Column<int>(type: "integer", nullable: false),
                    RequiresCompetency = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    Code = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Name = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "character varying(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test_methods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_test_methods_standard_documents_StandardDocumentId",
                        column: x => x.StandardDocumentId,
                        principalTable: "standard_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    LastName = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    FirstName = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    MiddleName = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    Position = table.Column<string>(
                        type: "character varying(200)",
                        maxLength: 200,
                        nullable: false
                    ),
                    SubdivisionId = table.Column<Guid>(type: "uuid", nullable: true),
                    WorkSchedule = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    Code = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Name = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "character varying(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employees_subdivisions_SubdivisionId",
                        column: x => x.SubdivisionId,
                        principalTable: "subdivisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "control_programs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NomenclatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    ControlKind = table.Column<int>(type: "integer", nullable: false),
                    ProductionLineId = table.Column<Guid>(type: "uuid", nullable: true),
                    StandardDocumentId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    ApprovedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    ApprovedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    Code = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Name = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "character varying(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_control_programs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_control_programs_nomenclatures_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "nomenclatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_control_programs_production_lines_ProductionLineId",
                        column: x => x.ProductionLineId,
                        principalTable: "production_lines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_control_programs_standard_documents_StandardDocumentId",
                        column: x => x.StandardDocumentId,
                        principalTable: "standard_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "material_batches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    NomenclatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uuid", nullable: false),
                    ArrivalDate = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    Quantity = table.Column<decimal>(
                        type: "numeric(18,4)",
                        precision: 18,
                        scale: 4,
                        nullable: false
                    ),
                    QuantityUnit = table.Column<string>(
                        type: "character varying(32)",
                        maxLength: 32,
                        nullable: true
                    ),
                    SupplierDocumentNumber = table.Column<string>(
                        type: "character varying(128)",
                        maxLength: 128,
                        nullable: true
                    ),
                    SupplierDocumentDate = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    ManufacturedOn = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Decision = table.Column<int>(type: "integer", nullable: false),
                    DecisionComment = table.Column<string>(
                        type: "character varying(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_material_batches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_material_batches_nomenclatures_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "nomenclatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_material_batches_suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_material_batches_warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "product_batches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    NomenclatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionLineId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductionDate = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    DesignValue = table.Column<decimal>(
                        type: "numeric(18,4)",
                        precision: 18,
                        scale: 4,
                        nullable: true
                    ),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_batches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_product_batches_nomenclatures_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "nomenclatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_product_batches_production_lines_ProductionLineId",
                        column: x => x.ProductionLineId,
                        principalTable: "production_lines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "test_method_equipment_types",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TestMethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test_method_equipment_types", x => x.Id);
                    table.ForeignKey(
                        name: "FK_test_method_equipment_types_equipment_types_EquipmentTypeId",
                        column: x => x.EquipmentTypeId,
                        principalTable: "equipment_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_test_method_equipment_types_test_methods_TestMethodId",
                        column: x => x.TestMethodId,
                        principalTable: "test_methods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "test_method_materials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TestMethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    NomenclatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlannedQuantity = table.Column<decimal>(
                        type: "numeric(18,4)",
                        precision: 18,
                        scale: 4,
                        nullable: false
                    ),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test_method_materials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_test_method_materials_nomenclatures_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "nomenclatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_test_method_materials_test_methods_TestMethodId",
                        column: x => x.TestMethodId,
                        principalTable: "test_methods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "test_method_quantities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TestMethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    MeasuredQuantityId = table.Column<Guid>(type: "uuid", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test_method_quantities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_test_method_quantities_measured_quantities_MeasuredQuantity~",
                        column: x => x.MeasuredQuantityId,
                        principalTable: "measured_quantities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_test_method_quantities_test_methods_TestMethodId",
                        column: x => x.TestMethodId,
                        principalTable: "test_methods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "competencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    TestMethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    IssuedOn = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    ValidUntil = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CertificateNumber = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_competencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_competencies_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_competencies_test_methods_TestMethodId",
                        column: x => x.TestMethodId,
                        principalTable: "test_methods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "equipment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryNumber = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: true
                    ),
                    SerialNumber = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: true
                    ),
                    Manufacturer = table.Column<string>(
                        type: "character varying(200)",
                        maxLength: 200,
                        nullable: true
                    ),
                    YearOfManufacture = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    VerificationValidUntil = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    VerificationIntervalMonths = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                    ResponsibleEmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    Code = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Name = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "character varying(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_equipment_employees_ResponsibleEmployeeId",
                        column: x => x.ResponsibleEmployeeId,
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_equipment_equipment_types_EquipmentTypeId",
                        column: x => x.EquipmentTypeId,
                        principalTable: "equipment_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "user_accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    PasswordHash = table.Column<string>(
                        type: "character varying(500)",
                        maxLength: 500,
                        nullable: false
                    ),
                    Roles = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: false
                    ),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_accounts_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "control_program_parameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ControlProgramId = table.Column<Guid>(type: "uuid", nullable: false),
                    QualityParameterId = table.Column<Guid>(type: "uuid", nullable: false),
                    TestMethodId = table.Column<Guid>(type: "uuid", nullable: true),
                    NormKind = table.Column<int>(type: "integer", nullable: false),
                    NormMin = table.Column<decimal>(
                        type: "numeric(18,4)",
                        precision: 18,
                        scale: 4,
                        nullable: true
                    ),
                    NormMax = table.Column<decimal>(
                        type: "numeric(18,4)",
                        precision: 18,
                        scale: 4,
                        nullable: true
                    ),
                    Tolerance = table.Column<decimal>(
                        type: "numeric(18,4)",
                        precision: 18,
                        scale: 4,
                        nullable: true
                    ),
                    PercentOfDesign = table.Column<decimal>(
                        type: "numeric(18,4)",
                        precision: 18,
                        scale: 4,
                        nullable: true
                    ),
                    NormText = table.Column<string>(
                        type: "character varying(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    Frequency = table.Column<int>(type: "integer", nullable: false),
                    Replicates = table.Column<int>(type: "integer", nullable: false),
                    Aggregation = table.Column<int>(type: "integer", nullable: false),
                    IsMandatory = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_control_program_parameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_control_program_parameters_control_programs_ControlProgramId",
                        column: x => x.ControlProgramId,
                        principalTable: "control_programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_control_program_parameters_quality_parameters_QualityParame~",
                        column: x => x.QualityParameterId,
                        principalTable: "quality_parameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_control_program_parameters_test_methods_TestMethodId",
                        column: x => x.TestMethodId,
                        principalTable: "test_methods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "product_units",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductBatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    SerialNumber = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_product_units_product_batches_ProductBatchId",
                        column: x => x.ProductBatchId,
                        principalTable: "product_batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "traceability_links",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductBatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialBatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<decimal>(
                        type: "numeric(18,4)",
                        precision: 18,
                        scale: 4,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_traceability_links", x => x.Id);
                    table.ForeignKey(
                        name: "FK_traceability_links_material_batches_MaterialBatchId",
                        column: x => x.MaterialBatchId,
                        principalTable: "material_batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_traceability_links_product_batches_ProductBatchId",
                        column: x => x.ProductBatchId,
                        principalTable: "product_batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "equipment_events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    EventDate = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    ValidUntil = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    CertificateNumber = table.Column<string>(
                        type: "character varying(128)",
                        maxLength: 128,
                        nullable: true
                    ),
                    Organization = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: true
                    ),
                    Comment = table.Column<string>(
                        type: "character varying(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    PerformedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipment_events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_equipment_events_employees_PerformedById",
                        column: x => x.PerformedById,
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_equipment_events_equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "control_orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    ControlKind = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    MaterialBatchId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductBatchId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductUnitId = table.Column<Guid>(type: "uuid", nullable: true),
                    ControlProgramId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlannedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CompletedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    ResponsibleEmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    Comment = table.Column<string>(
                        type: "character varying(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_control_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_control_orders_control_programs_ControlProgramId",
                        column: x => x.ControlProgramId,
                        principalTable: "control_programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_control_orders_employees_ResponsibleEmployeeId",
                        column: x => x.ResponsibleEmployeeId,
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_control_orders_material_batches_MaterialBatchId",
                        column: x => x.MaterialBatchId,
                        principalTable: "material_batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_control_orders_product_batches_ProductBatchId",
                        column: x => x.ProductBatchId,
                        principalTable: "product_batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_control_orders_product_units_ProductUnitId",
                        column: x => x.ProductUnitId,
                        principalTable: "product_units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "protocols",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: true),
                    ControlOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    IssuedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    OverallVerdict = table.Column<int>(type: "integer", nullable: false),
                    ResultsSnapshot = table.Column<string>(
                        type: "text",
                        maxLength: 400,
                        nullable: true
                    ),
                    Conclusion = table.Column<string>(
                        type: "character varying(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_protocols", x => x.Id);
                    table.ForeignKey(
                        name: "FK_protocols_control_orders_ControlOrderId",
                        column: x => x.ControlOrderId,
                        principalTable: "control_orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_protocols_document_templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "document_templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "samples",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Barcode = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    ControlOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    SampledAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    SampledById = table.Column<Guid>(type: "uuid", nullable: true),
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: true),
                    State = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<decimal>(
                        type: "numeric(18,4)",
                        precision: 18,
                        scale: 4,
                        nullable: true
                    ),
                    QuantityUnit = table.Column<string>(
                        type: "character varying(32)",
                        maxLength: 32,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_samples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_samples_control_orders_ControlOrderId",
                        column: x => x.ControlOrderId,
                        principalTable: "control_orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_samples_employees_SampledById",
                        column: x => x.SampledById,
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_samples_warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "nonconformances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    Stage = table.Column<int>(type: "integer", nullable: false),
                    ControlOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProtocolId = table.Column<Guid>(type: "uuid", nullable: true),
                    MaterialBatchId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductBatchId = table.Column<Guid>(type: "uuid", nullable: true),
                    DetectedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    DetectedById = table.Column<Guid>(type: "uuid", nullable: true),
                    ParameterName = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    ActualValue = table.Column<string>(
                        type: "character varying(128)",
                        maxLength: 128,
                        nullable: true
                    ),
                    NormValue = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: true
                    ),
                    CauseId = table.Column<Guid>(type: "uuid", nullable: true),
                    CauseComment = table.Column<string>(
                        type: "character varying(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Decision = table.Column<int>(type: "integer", nullable: false),
                    DecisionComment = table.Column<string>(
                        type: "character varying(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nonconformances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_nonconformances_control_orders_ControlOrderId",
                        column: x => x.ControlOrderId,
                        principalTable: "control_orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_nonconformances_employees_DetectedById",
                        column: x => x.DetectedById,
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_nonconformances_material_batches_MaterialBatchId",
                        column: x => x.MaterialBatchId,
                        principalTable: "material_batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_nonconformances_nonconformance_causes_CauseId",
                        column: x => x.CauseId,
                        principalTable: "nonconformance_causes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_nonconformances_product_batches_ProductBatchId",
                        column: x => x.ProductBatchId,
                        principalTable: "product_batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_nonconformances_protocols_ProtocolId",
                        column: x => x.ProtocolId,
                        principalTable: "protocols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "protocol_signatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProtocolId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    SignedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_protocol_signatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_protocol_signatures_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_protocol_signatures_protocols_ProtocolId",
                        column: x => x.ProtocolId,
                        principalTable: "protocols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "sample_state_history",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SampleId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromState = table.Column<int>(type: "integer", nullable: false),
                    ToState = table.Column<int>(type: "integer", nullable: false),
                    ChangedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    ChangedById = table.Column<Guid>(type: "uuid", nullable: true),
                    Comment = table.Column<string>(
                        type: "character varying(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sample_state_history", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sample_state_history_employees_ChangedById",
                        column: x => x.ChangedById,
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_sample_state_history_samples_SampleId",
                        column: x => x.SampleId,
                        principalTable: "samples",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "test_runs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ControlOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    SampleId = table.Column<Guid>(type: "uuid", nullable: true),
                    ControlProgramParameterId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssigneeId = table.Column<Guid>(type: "uuid", nullable: true),
                    EquipmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    CompletedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test_runs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_test_runs_control_orders_ControlOrderId",
                        column: x => x.ControlOrderId,
                        principalTable: "control_orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_test_runs_control_program_parameters_ControlProgramParamete~",
                        column: x => x.ControlProgramParameterId,
                        principalTable: "control_program_parameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_test_runs_employees_AssigneeId",
                        column: x => x.AssigneeId,
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_test_runs_equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_test_runs_samples_SampleId",
                        column: x => x.SampleId,
                        principalTable: "samples",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "corrective_actions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NonconformanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(
                        type: "character varying(2000)",
                        maxLength: 2000,
                        nullable: false
                    ),
                    AssignedToId = table.Column<Guid>(type: "uuid", nullable: true),
                    DueDate = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    CompletedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Result = table.Column<string>(
                        type: "character varying(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_corrective_actions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_corrective_actions_employees_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_corrective_actions_nonconformances_NonconformanceId",
                        column: x => x.NonconformanceId,
                        principalTable: "nonconformances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "measurements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TestRunId = table.Column<Guid>(type: "uuid", nullable: false),
                    MeasuredQuantityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Replicate = table.Column<int>(type: "integer", nullable: false),
                    NumericValue = table.Column<decimal>(
                        type: "numeric(18,4)",
                        precision: 18,
                        scale: 4,
                        nullable: true
                    ),
                    BooleanValue = table.Column<bool>(type: "boolean", nullable: true),
                    TextValue = table.Column<string>(
                        type: "character varying(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    RecordedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    RecordedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_measurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_measurements_employees_RecordedById",
                        column: x => x.RecordedById,
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_measurements_measured_quantities_MeasuredQuantityId",
                        column: x => x.MeasuredQuantityId,
                        principalTable: "measured_quantities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_measurements_test_runs_TestRunId",
                        column: x => x.TestRunId,
                        principalTable: "test_runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "parameter_results",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TestRunId = table.Column<Guid>(type: "uuid", nullable: false),
                    NumericValue = table.Column<decimal>(
                        type: "numeric(18,4)",
                        precision: 18,
                        scale: 4,
                        nullable: true
                    ),
                    BooleanValue = table.Column<bool>(type: "boolean", nullable: true),
                    TextValue = table.Column<string>(
                        type: "character varying(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    NormSnapshot = table.Column<string>(
                        type: "character varying(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    Verdict = table.Column<int>(type: "integer", nullable: false),
                    CalculatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parameter_results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parameter_results_test_runs_TestRunId",
                        column: x => x.TestRunId,
                        principalTable: "test_runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_audit_log_EntityType_EntityId",
                table: "audit_log",
                columns: new[] { "EntityType", "EntityId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_audit_log_OccurredAt",
                table: "audit_log",
                column: "OccurredAt"
            );

            migrationBuilder.CreateIndex(
                name: "IX_competencies_EmployeeId_TestMethodId",
                table: "competencies",
                columns: new[] { "EmployeeId", "TestMethodId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_competencies_TestMethodId",
                table: "competencies",
                column: "TestMethodId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_control_orders_ControlProgramId",
                table: "control_orders",
                column: "ControlProgramId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_control_orders_MaterialBatchId",
                table: "control_orders",
                column: "MaterialBatchId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_control_orders_Number",
                table: "control_orders",
                column: "Number",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_control_orders_ProductBatchId",
                table: "control_orders",
                column: "ProductBatchId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_control_orders_ProductUnitId",
                table: "control_orders",
                column: "ProductUnitId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_control_orders_ResponsibleEmployeeId",
                table: "control_orders",
                column: "ResponsibleEmployeeId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_control_program_parameters_ControlProgramId",
                table: "control_program_parameters",
                column: "ControlProgramId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_control_program_parameters_QualityParameterId",
                table: "control_program_parameters",
                column: "QualityParameterId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_control_program_parameters_TestMethodId",
                table: "control_program_parameters",
                column: "TestMethodId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_control_programs_Code",
                table: "control_programs",
                column: "Code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_control_programs_NomenclatureId_ControlKind_ProductionLineId",
                table: "control_programs",
                columns: new[] { "NomenclatureId", "ControlKind", "ProductionLineId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_control_programs_ProductionLineId",
                table: "control_programs",
                column: "ProductionLineId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_control_programs_StandardDocumentId",
                table: "control_programs",
                column: "StandardDocumentId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_corrective_actions_AssignedToId",
                table: "corrective_actions",
                column: "AssignedToId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_corrective_actions_NonconformanceId",
                table: "corrective_actions",
                column: "NonconformanceId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_defects_Code",
                table: "defects",
                column: "Code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_document_templates_Code",
                table: "document_templates",
                column: "Code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_document_templates_Kind",
                table: "document_templates",
                column: "Kind",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_employees_Code",
                table: "employees",
                column: "Code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_employees_SubdivisionId",
                table: "employees",
                column: "SubdivisionId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_employees_UserName",
                table: "employees",
                column: "UserName",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_equipment_Code",
                table: "equipment",
                column: "Code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_equipment_EquipmentTypeId",
                table: "equipment",
                column: "EquipmentTypeId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_equipment_ResponsibleEmployeeId",
                table: "equipment",
                column: "ResponsibleEmployeeId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_equipment_events_EquipmentId",
                table: "equipment_events",
                column: "EquipmentId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_equipment_events_PerformedById",
                table: "equipment_events",
                column: "PerformedById"
            );

            migrationBuilder.CreateIndex(
                name: "IX_equipment_types_Code",
                table: "equipment_types",
                column: "Code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_material_batches_NomenclatureId",
                table: "material_batches",
                column: "NomenclatureId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_material_batches_Number",
                table: "material_batches",
                column: "Number",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_material_batches_SupplierId",
                table: "material_batches",
                column: "SupplierId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_material_batches_WarehouseId",
                table: "material_batches",
                column: "WarehouseId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_measured_quantities_Code",
                table: "measured_quantities",
                column: "Code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_measurements_MeasuredQuantityId",
                table: "measurements",
                column: "MeasuredQuantityId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_measurements_RecordedById",
                table: "measurements",
                column: "RecordedById"
            );

            migrationBuilder.CreateIndex(
                name: "IX_measurements_TestRunId",
                table: "measurements",
                column: "TestRunId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_nomenclature_categories_Code",
                table: "nomenclature_categories",
                column: "Code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_nomenclatures_CategoryId",
                table: "nomenclatures",
                column: "CategoryId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_nomenclatures_Code",
                table: "nomenclatures",
                column: "Code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_nomenclatures_RequirementStandardId",
                table: "nomenclatures",
                column: "RequirementStandardId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_nonconformance_causes_Code",
                table: "nonconformance_causes",
                column: "Code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_nonconformances_CauseId",
                table: "nonconformances",
                column: "CauseId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_nonconformances_ControlOrderId",
                table: "nonconformances",
                column: "ControlOrderId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_nonconformances_DetectedById",
                table: "nonconformances",
                column: "DetectedById"
            );

            migrationBuilder.CreateIndex(
                name: "IX_nonconformances_MaterialBatchId",
                table: "nonconformances",
                column: "MaterialBatchId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_nonconformances_Number",
                table: "nonconformances",
                column: "Number",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_nonconformances_ProductBatchId",
                table: "nonconformances",
                column: "ProductBatchId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_nonconformances_ProtocolId",
                table: "nonconformances",
                column: "ProtocolId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_parameter_results_TestRunId",
                table: "parameter_results",
                column: "TestRunId",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_product_batches_NomenclatureId",
                table: "product_batches",
                column: "NomenclatureId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_product_batches_Number",
                table: "product_batches",
                column: "Number",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_product_batches_ProductionLineId",
                table: "product_batches",
                column: "ProductionLineId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_product_units_ProductBatchId",
                table: "product_units",
                column: "ProductBatchId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_product_units_SerialNumber",
                table: "product_units",
                column: "SerialNumber",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_production_lines_Code",
                table: "production_lines",
                column: "Code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_protocol_signatures_EmployeeId",
                table: "protocol_signatures",
                column: "EmployeeId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_protocol_signatures_ProtocolId_Role_EmployeeId",
                table: "protocol_signatures",
                columns: new[] { "ProtocolId", "Role", "EmployeeId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_protocols_ControlOrderId",
                table: "protocols",
                column: "ControlOrderId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_protocols_Number",
                table: "protocols",
                column: "Number",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_protocols_TemplateId",
                table: "protocols",
                column: "TemplateId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_quality_parameters_Code",
                table: "quality_parameters",
                column: "Code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_sample_state_history_ChangedById",
                table: "sample_state_history",
                column: "ChangedById"
            );

            migrationBuilder.CreateIndex(
                name: "IX_sample_state_history_SampleId",
                table: "sample_state_history",
                column: "SampleId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_samples_Barcode",
                table: "samples",
                column: "Barcode",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_samples_ControlOrderId",
                table: "samples",
                column: "ControlOrderId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_samples_Number",
                table: "samples",
                column: "Number",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_samples_SampledById",
                table: "samples",
                column: "SampledById"
            );

            migrationBuilder.CreateIndex(
                name: "IX_samples_WarehouseId",
                table: "samples",
                column: "WarehouseId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_standard_documents_Code",
                table: "standard_documents",
                column: "Code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_subdivisions_Code",
                table: "subdivisions",
                column: "Code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_subdivisions_ProductionLineId",
                table: "subdivisions",
                column: "ProductionLineId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_suppliers_Code",
                table: "suppliers",
                column: "Code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_test_method_equipment_types_EquipmentTypeId",
                table: "test_method_equipment_types",
                column: "EquipmentTypeId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_test_method_equipment_types_TestMethodId",
                table: "test_method_equipment_types",
                column: "TestMethodId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_test_method_materials_NomenclatureId",
                table: "test_method_materials",
                column: "NomenclatureId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_test_method_materials_TestMethodId",
                table: "test_method_materials",
                column: "TestMethodId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_test_method_quantities_MeasuredQuantityId",
                table: "test_method_quantities",
                column: "MeasuredQuantityId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_test_method_quantities_TestMethodId_MeasuredQuantityId",
                table: "test_method_quantities",
                columns: new[] { "TestMethodId", "MeasuredQuantityId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_test_methods_Code",
                table: "test_methods",
                column: "Code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_test_methods_StandardDocumentId",
                table: "test_methods",
                column: "StandardDocumentId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_test_runs_AssigneeId",
                table: "test_runs",
                column: "AssigneeId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_test_runs_ControlOrderId",
                table: "test_runs",
                column: "ControlOrderId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_test_runs_ControlProgramParameterId",
                table: "test_runs",
                column: "ControlProgramParameterId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_test_runs_EquipmentId",
                table: "test_runs",
                column: "EquipmentId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_test_runs_SampleId",
                table: "test_runs",
                column: "SampleId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_traceability_links_MaterialBatchId",
                table: "traceability_links",
                column: "MaterialBatchId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_traceability_links_ProductBatchId_MaterialBatchId",
                table: "traceability_links",
                columns: new[] { "ProductBatchId", "MaterialBatchId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_user_accounts_EmployeeId",
                table: "user_accounts",
                column: "EmployeeId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_user_accounts_UserName",
                table: "user_accounts",
                column: "UserName",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_warehouses_Code",
                table: "warehouses",
                column: "Code",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "audit_log");

            migrationBuilder.DropTable(name: "competencies");

            migrationBuilder.DropTable(name: "corrective_actions");

            migrationBuilder.DropTable(name: "defects");

            migrationBuilder.DropTable(name: "equipment_events");

            migrationBuilder.DropTable(name: "measurements");

            migrationBuilder.DropTable(name: "parameter_results");

            migrationBuilder.DropTable(name: "protocol_signatures");

            migrationBuilder.DropTable(name: "sample_state_history");

            migrationBuilder.DropTable(name: "test_method_equipment_types");

            migrationBuilder.DropTable(name: "test_method_materials");

            migrationBuilder.DropTable(name: "test_method_quantities");

            migrationBuilder.DropTable(name: "traceability_links");

            migrationBuilder.DropTable(name: "user_accounts");

            migrationBuilder.DropTable(name: "nonconformances");

            migrationBuilder.DropTable(name: "test_runs");

            migrationBuilder.DropTable(name: "measured_quantities");

            migrationBuilder.DropTable(name: "nonconformance_causes");

            migrationBuilder.DropTable(name: "protocols");

            migrationBuilder.DropTable(name: "control_program_parameters");

            migrationBuilder.DropTable(name: "equipment");

            migrationBuilder.DropTable(name: "samples");

            migrationBuilder.DropTable(name: "document_templates");

            migrationBuilder.DropTable(name: "quality_parameters");

            migrationBuilder.DropTable(name: "test_methods");

            migrationBuilder.DropTable(name: "equipment_types");

            migrationBuilder.DropTable(name: "control_orders");

            migrationBuilder.DropTable(name: "control_programs");

            migrationBuilder.DropTable(name: "employees");

            migrationBuilder.DropTable(name: "material_batches");

            migrationBuilder.DropTable(name: "product_units");

            migrationBuilder.DropTable(name: "subdivisions");

            migrationBuilder.DropTable(name: "suppliers");

            migrationBuilder.DropTable(name: "warehouses");

            migrationBuilder.DropTable(name: "product_batches");

            migrationBuilder.DropTable(name: "nomenclatures");

            migrationBuilder.DropTable(name: "production_lines");

            migrationBuilder.DropTable(name: "nomenclature_categories");

            migrationBuilder.DropTable(name: "standard_documents");
        }
    }
}
