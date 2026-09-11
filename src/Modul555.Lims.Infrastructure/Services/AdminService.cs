using Microsoft.EntityFrameworkCore;
using Modul555.Lims.Application.Catalogs;
using Modul555.Lims.Application.Common;
using Modul555.Lims.Domain.Common;
using Modul555.Lims.Domain.Enums;
using Modul555.Lims.Domain.Operations;
using Modul555.Lims.Domain.Reference;
using Modul555.Lims.Infrastructure.Persistence;

namespace Modul555.Lims.Infrastructure.Services;

public sealed class AdminService
{
    private readonly LimsDbContext _db;

    public AdminService(LimsDbContext db) => _db = db;

    public async Task<PagedResult<UserAccountDto>> Users(PagedQuery q, CancellationToken ct)
    {
        var query = _db.UserAccounts.Include(a => a.Employee).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim();
            query = query.Where(a =>
                a.UserName.Contains(s)
                || (
                    a.Employee != null
                    && (a.Employee.LastName.Contains(s) || a.Employee.FirstName.Contains(s))
                )
            );
        }
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(a => a.UserName)
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(a => new UserAccountDto
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                EmployeeName =
                    a.Employee != null ? a.Employee.LastName + " " + a.Employee.FirstName : null,
                UserName = a.UserName,
                Roles = a.Roles,
                IsLocked = a.IsLocked,
            })
            .ToListAsync(ct);
        return new PagedResult<UserAccountDto>
        {
            Items = items,
            Total = total,
            Page = q.Page,
            PageSize = q.PageSize,
        };
    }

    public async Task<UserAccountDto> SaveUserAsync(
        Guid? id,
        UserAccountWriteDto dto,
        CancellationToken ct
    )
    {
        var employee =
            await _db.Employees.FindAsync([dto.EmployeeId], ct)
            ?? throw new InvalidOperationException("Сотрудник не найден");

        UserAccount account;
        if (id is null)
        {
            if (await _db.UserAccounts.AnyAsync(a => a.UserName == dto.UserName, ct))
                throw new InvalidOperationException("Логин уже занят");
            account = new UserAccount
            {
                EmployeeId = dto.EmployeeId,
                PasswordHash = "KEYCLOAK",
            };
            _db.UserAccounts.Add(account);
        }
        else
        {
            account =
                await _db.UserAccounts.FindAsync([id.Value], ct)
                ?? throw new InvalidOperationException("Учётная запись не найдена");
            if (
                await _db.UserAccounts.AnyAsync(
                    a => a.UserName == dto.UserName && a.Id != id.Value,
                    ct
                )
            )
                throw new InvalidOperationException("Логин уже занят");
        }

        account.EmployeeId = dto.EmployeeId;
        account.UserName = dto.UserName.Trim();
        account.Roles = string.Join(
            ',',
            dto.Roles.Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
                )
                .Where(r => Roles.All.Contains(r))
        );
        account.IsLocked = dto.IsLocked;
        if (string.IsNullOrWhiteSpace(account.PasswordHash))
            account.PasswordHash = "KEYCLOAK";

        employee.UserName = account.UserName;
        await _db.SaveChangesAsync(ct);

        return new UserAccountDto
        {
            Id = account.Id,
            EmployeeId = account.EmployeeId,
            EmployeeName = employee.FullName,
            UserName = account.UserName,
            Roles = account.Roles,
            IsLocked = account.IsLocked,
        };
    }

    public async Task<CompetencyDto> SaveCompetencyAsync(
        Guid? id,
        CompetencyWriteDto dto,
        CancellationToken ct
    )
    {
        Competency entity;
        if (id is null)
        {
            entity = new Competency();
            _db.Competencies.Add(entity);
        }
        else
        {
            entity =
                await _db.Competencies.FindAsync([id.Value], ct)
                ?? throw new InvalidOperationException("Аттестация не найдена");
        }

        entity.EmployeeId = dto.EmployeeId;
        entity.TestMethodId = dto.TestMethodId;
        entity.IssuedOn = dto.IssuedOn;
        entity.ValidUntil = dto.ValidUntil;
        entity.CertificateNumber = dto.CertificateNumber;
        await _db.SaveChangesAsync(ct);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var loaded = await _db
            .Competencies.Include(c => c.Employee)
            .Include(c => c.TestMethod)
            .FirstAsync(c => c.Id == entity.Id, ct);
        return new CompetencyDto
        {
            Id = loaded.Id,
            EmployeeId = loaded.EmployeeId,
            EmployeeName = loaded.Employee?.FullName,
            TestMethodId = loaded.TestMethodId,
            TestMethodName = loaded.TestMethod?.Name,
            IssuedOn = loaded.IssuedOn,
            ValidUntil = loaded.ValidUntil,
            CertificateNumber = loaded.CertificateNumber,
            IsValid = loaded.IsValidOn(today),
        };
    }

    public async Task<bool> DeleteCompetencyAsync(Guid id, CancellationToken ct)
    {
        var entity = await _db.Competencies.FindAsync([id], ct);
        if (entity is null)
            return false;
        _db.Competencies.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<PagedResult<AuditLogDto>> AuditLog(PagedQuery q, CancellationToken ct)
    {
        var query = _db.AuditLog.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim();
            query = query.Where(a =>
                a.UserName.Contains(s)
                || a.EntityType.Contains(s)
                || a.EntityId.Contains(s)
                || a.Action.Contains(s)
            );
        }
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(a => a.OccurredAt)
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(a => new AuditLogDto
            {
                Id = a.Id,
                OccurredAt = a.OccurredAt,
                UserName = a.UserName,
                EntityType = a.EntityType,
                EntityId = a.EntityId,
                Action = a.Action,
                Changes = a.Changes,
            })
            .ToListAsync(ct);
        return new PagedResult<AuditLogDto>
        {
            Items = items,
            Total = total,
            Page = q.Page,
            PageSize = q.PageSize,
        };
    }

    public async Task<PagedResult<BatchAdminDto>> MaterialBatches(
        PagedQuery q,
        CancellationToken ct
    )
    {
        var query = _db.MaterialBatches.Include(b => b.Nomenclature).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim();
            query = query.Where(b =>
                b.Number.Contains(s) || (b.Nomenclature != null && b.Nomenclature.Name.Contains(s))
            );
        }
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(b => b.ArrivalDate)
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(b => new BatchAdminDto
            {
                Id = b.Id,
                Number = b.Number,
                Kind = "Material",
                NomenclatureName = b.Nomenclature != null ? b.Nomenclature.Name : null,
                Status = b.Status,
                Decision = b.Decision,
                DecisionComment = b.DecisionComment,
            })
            .ToListAsync(ct);
        return new PagedResult<BatchAdminDto>
        {
            Items = items,
            Total = total,
            Page = q.Page,
            PageSize = q.PageSize,
        };
    }

    public async Task<PagedResult<BatchAdminDto>> ProductBatches(PagedQuery q, CancellationToken ct)
    {
        var query = _db.ProductBatches.Include(b => b.Nomenclature).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim();
            query = query.Where(b =>
                b.Number.Contains(s) || (b.Nomenclature != null && b.Nomenclature.Name.Contains(s))
            );
        }
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(b => b.ProductionDate)
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(b => new BatchAdminDto
            {
                Id = b.Id,
                Number = b.Number,
                Kind = "Product",
                NomenclatureName = b.Nomenclature != null ? b.Nomenclature.Name : null,
                Status = b.Status,
                Decision = BatchDecision.None,
                DecisionComment = null,
            })
            .ToListAsync(ct);
        return new PagedResult<BatchAdminDto>
        {
            Items = items,
            Total = total,
            Page = q.Page,
            PageSize = q.PageSize,
        };
    }

    public async Task<BatchAdminDto?> UpdateMaterialBatchAsync(
        Guid id,
        BatchAdminWriteDto dto,
        CancellationToken ct
    )
    {
        var b = await _db
            .MaterialBatches.Include(x => x.Nomenclature)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (b is null)
            return null;
        b.Status = dto.Status;
        b.Decision = dto.Decision;
        b.DecisionComment = dto.DecisionComment;
        if (dto.WarehouseId.HasValue)
            b.WarehouseId = dto.WarehouseId;
        await _db.SaveChangesAsync(ct);
        return new BatchAdminDto
        {
            Id = b.Id,
            Number = b.Number,
            Kind = "Material",
            NomenclatureName = b.Nomenclature?.Name,
            Status = b.Status,
            Decision = b.Decision,
            DecisionComment = b.DecisionComment,
        };
    }

    public async Task<BatchAdminDto?> UpdateProductBatchAsync(
        Guid id,
        BatchAdminWriteDto dto,
        CancellationToken ct
    )
    {
        var b = await _db
            .ProductBatches.Include(x => x.Nomenclature)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (b is null)
            return null;
        b.Status = dto.Status;
        await _db.SaveChangesAsync(ct);
        return new BatchAdminDto
        {
            Id = b.Id,
            Number = b.Number,
            Kind = "Product",
            NomenclatureName = b.Nomenclature?.Name,
            Status = b.Status,
            Decision = BatchDecision.None,
        };
    }

    public async Task EnsureKeycloakEmployeesAsync(CancellationToken ct)
    {
        var lab = await _db.Subdivisions.FirstOrDefaultAsync(s => s.Code == "LAB", ct);
        var seeds = new (string UserName, string LastName, string FirstName, string Position, string Roles)[]
        {
            ("admin", "Администратор", "Системы", "Администратор LIMS", string.Join(',', Roles.All)),
            ("manager", "Менеджер", "Keycloak", "Менеджер", Roles.Manager),
            ("tester", "Тестировщик", "Keycloak", "Тестировщик", Roles.Tester),
            ("developer", "Разработчик", "Keycloak", "Разработчик", Roles.Developer),
        };

        Employee? adminEmp = null;
        foreach (var seed in seeds)
        {
            var emp =
                await _db.Employees.FirstOrDefaultAsync(
                    e => e.UserName == seed.UserName || e.Code == seed.UserName,
                    ct
                );
            if (emp is null)
            {
                emp = new Employee
                {
                    Code = seed.UserName,
                    Name = $"{seed.LastName} {seed.FirstName}".Trim(),
                    UserName = seed.UserName,
                    LastName = seed.LastName,
                    FirstName = seed.FirstName,
                    Position = seed.Position,
                    IsActive = true,
                    SubdivisionId = lab?.Id,
                };
                _db.Employees.Add(emp);
                await _db.SaveChangesAsync(ct);
            }
            else
            {
                emp.UserName = seed.UserName;
                emp.IsActive = true;
                if (string.IsNullOrWhiteSpace(emp.Position))
                    emp.Position = seed.Position;
            }

            if (seed.UserName == "admin")
                adminEmp = emp;

            var account = await _db.UserAccounts.FirstOrDefaultAsync(
                a => a.UserName == seed.UserName,
                ct
            );
            if (account is null)
            {
                _db.UserAccounts.Add(
                    new UserAccount
                    {
                        EmployeeId = emp.Id,
                        UserName = seed.UserName,
                        PasswordHash = "KEYCLOAK",
                        Roles = seed.Roles,
                        IsLocked = false,
                    }
                );
            }
            else
            {
                account.EmployeeId = emp.Id;
                account.UserName = seed.UserName;
                account.PasswordHash = "KEYCLOAK";
                account.Roles = seed.Roles;
                account.IsLocked = false;
            }
        }

        await _db.SaveChangesAsync(ct);

        if (adminEmp is null)
            return;

        var existingMethodIds = await _db
            .Competencies.Where(c => c.EmployeeId == adminEmp.Id)
            .Select(c => c.TestMethodId)
            .ToListAsync(ct);
        var allMethods = await _db
            .TestMethods.Where(m => m.IsActive)
            .Select(m => m.Id)
            .ToListAsync(ct);
        foreach (var mid in allMethods.Except(existingMethodIds))
        {
            _db.Competencies.Add(
                new Competency
                {
                    EmployeeId = adminEmp.Id,
                    TestMethodId = mid,
                    IssuedOn = new DateOnly(2025, 1, 1),
                    ValidUntil = new DateOnly(2030, 12, 31),
                    CertificateNumber = "АТ-admin",
                }
            );
        }

        await _db.SaveChangesAsync(ct);
    }

    public sealed class DemoDataResult
    {
        public int Deleted { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? OrderNumber { get; set; }
        public string? BatchNumber { get; set; }
        public int TestRuns { get; set; }
    }

    public async Task<DemoDataResult> ClearDemoDataAsync(CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        async Task<int> Del<T>()
            where T : class
        {
            var set = _db.Set<T>();
            var n = await set.CountAsync(ct);
            set.RemoveRange(await set.ToListAsync(ct));
            return n;
        }

        var deleted = 0;
        deleted += await Del<Measurement>();
        deleted += await Del<ParameterResult>();
        deleted += await Del<TestRun>();
        deleted += await Del<SampleStateHistory>();
        deleted += await Del<Sample>();
        deleted += await Del<ProtocolSignature>();
        deleted += await Del<Protocol>();
        deleted += await Del<CorrectiveAction>();
        deleted += await Del<Nonconformance>();
        deleted += await Del<TraceabilityLink>();
        deleted += await Del<ControlOrder>();
        deleted += await Del<ProductUnit>();
        deleted += await Del<ProductBatch>();
        deleted += await Del<MaterialBatch>();
        deleted += await Del<EquipmentEvent>();
        deleted += await Del<AuditLogEntry>();

        var adminId = await _db
            .UserAccounts.Where(a => a.UserName == "admin")
            .Select(a => a.EmployeeId)
            .FirstAsync(ct);
        var demoStaff = await _db
            .Employees.Where(e => e.Id != adminId && e.Code.StartsWith("DEMO-"))
            .ToListAsync(ct);
        if (demoStaff.Count > 0)
        {
            var ids = demoStaff.Select(e => e.Id).ToList();
            var comps = await _db
                .Competencies.Where(c => ids.Contains(c.EmployeeId))
                .ToListAsync(ct);
            _db.Competencies.RemoveRange(comps);
            deleted += comps.Count;
            _db.Employees.RemoveRange(demoStaff);
            deleted += demoStaff.Count;
        }

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return new DemoDataResult
        {
            Deleted = deleted,
            Message = $"Удалено записей операционного контура: {deleted}.",
        };
    }

    public async Task<DemoDataResult> GenerateDemoDataAsync(CancellationToken ct)
    {
        if (await _db.MaterialBatches.AnyAsync(ct) || await _db.ControlOrders.AnyAsync(ct))
            throw new InvalidOperationException(
                "Сначала очистите тестовые данные — в базе уже есть партии или распоряжения."
            );

        var admin = await _db.Employees.FirstAsync(
            e => e.UserName == "admin" || e.Code == "admin",
            ct
        );
        var cement =
            await _db.Nomenclatures.FirstOrDefaultAsync(n => n.Code == "CEM-42.5", ct)
            ?? throw new InvalidOperationException(
                "Номенклатура CEM-42.5 не найдена. Проверьте сид НСИ."
            );
        var supplier =
            await _db.Suppliers.FirstOrDefaultAsync(s => s.Code == "CEM-1", ct)
            ?? await _db.Suppliers.FirstAsync(ct);
        var warehouse = await _db.Warehouses.FirstOrDefaultAsync(w => w.Code == "RAW", ct);
        var samplesWh = await _db.Warehouses.FirstOrDefaultAsync(w => w.Code == "SAMPLES", ct);
        var program =
            await _db
                .ControlPrograms.Include(p => p.Parameters)
                .FirstOrDefaultAsync(
                    p =>
                        p.Code == "PRG-CEM"
                        || (p.NomenclatureId == cement.Id && p.ControlKind == ControlKind.Incoming),
                    ct
                )
            ?? throw new InvalidOperationException(
                "Программа входного контроля цемента не найдена."
            );

        var laborant = await _db.Employees.FirstOrDefaultAsync(e => e.Code == "DEMO-LAB", ct);
        if (laborant is null)
        {
            laborant = new Employee
            {
                Code = "DEMO-LAB",
                Name = "Петрова Анна Сергеевна",
                UserName = "DEMO-LAB",
                LastName = "Петрова",
                FirstName = "Анна",
                MiddleName = "Сергеевна",
                Position = "Лаборант",
                SubdivisionId = admin.SubdivisionId,
                WorkSchedule = "пятидневка 08:00–17:00",
                IsActive = true,
            };
            _db.Employees.Add(laborant);
            await _db.SaveChangesAsync(ct);

            var methodIds = await _db
                .TestMethods.Where(m => m.IsActive)
                .Select(m => m.Id)
                .ToListAsync(ct);
            foreach (var mid in methodIds.Take(20))
            {
                _db.Competencies.Add(
                    new Competency
                    {
                        EmployeeId = laborant.Id,
                        TestMethodId = mid,
                        IssuedOn = new DateOnly(2025, 1, 1),
                        ValidUntil = new DateOnly(2030, 12, 31),
                        CertificateNumber = "АТ-DEMO",
                    }
                );
            }
            await _db.SaveChangesAsync(ct);
        }

        var batch = new MaterialBatch
        {
            Number = $"ПТ-DEMO-{DateTime.UtcNow:yyyyMMdd}",
            NomenclatureId = cement.Id,
            SupplierId = supplier.Id,
            ArrivalDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Quantity = 40,
            QuantityUnit = "т",
            SupplierDocumentNumber = "ПС-DEMO-001",
            SupplierDocumentDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-3)),
            ManufacturedOn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
            WarehouseId = warehouse?.Id,
            Status = BatchStatus.InControl,
        };
        _db.MaterialBatches.Add(batch);

        var order = new ControlOrder
        {
            Number = $"ВК-DEMO-{DateTime.UtcNow:yyyyMMdd}",
            ControlKind = ControlKind.Incoming,
            Source = ControlOrderSource.MaterialArrival,
            Status = ControlOrderStatus.InProgress,
            ControlProgramId = program.Id,
            MaterialBatchId = batch.Id,
            PlannedAt = DateTimeOffset.UtcNow,
            ResponsibleEmployeeId = admin.Id,
            Comment = "Демонстрационное распоряжение (сгенерировано из админки)",
        };
        _db.ControlOrders.Add(order);
        await _db.SaveChangesAsync(ct);

        var sample = new Sample
        {
            Number = $"ПРБ-DEMO-{cement.Code}",
            Barcode = $"555{DateTime.UtcNow:yyMMdd}{Random.Shared.Next(1000, 9999)}",
            ControlOrderId = order.Id,
            SampledAt = DateTimeOffset.UtcNow,
            SampledById = laborant.Id,
            WarehouseId = samplesWh?.Id,
            Quantity = 5,
            QuantityUnit = "кг",
            State = SampleState.InTesting,
        };
        sample.History.Add(
            new SampleStateHistory
            {
                FromState = SampleState.Registered,
                ToState = SampleState.InTesting,
                ChangedAt = DateTimeOffset.UtcNow,
                ChangedById = admin.Id,
                Comment = "Демо-регистрация",
            }
        );
        _db.Samples.Add(sample);
        await _db.SaveChangesAsync(ct);

        var parameters = await _db
            .ControlProgramParameters.Where(p => p.ControlProgramId == program.Id)
            .OrderBy(p => p.SortOrder)
            .ToListAsync(ct);

        var press = await _db.Equipment.FirstOrDefaultAsync(
            e => e.Code.Contains("PRESS") || e.Name.Contains("Пресс"),
            ct
        );
        var runs = 0;
        foreach (var p in parameters)
        {
            _db.TestRuns.Add(
                new TestRun
                {
                    ControlOrderId = order.Id,
                    SampleId = sample.Id,
                    ControlProgramParameterId = p.Id,
                    Status = TestRunStatus.Assigned,
                    AssigneeId = laborant.Id,
                    EquipmentId = press?.Id,
                }
            );
            runs++;
        }

        var mix = await _db.Nomenclatures.FirstOrDefaultAsync(
            n => n.Code.Contains("MIX") || n.Name.Contains("смес"),
            ct
        );
        var line = await _db.ProductionLines.FirstOrDefaultAsync(ct);
        var opProgram = mix is null
            ? null
            : await _db.ControlPrograms.FirstOrDefaultAsync(
                p =>
                    p.NomenclatureId == mix.Id
                    && p.ControlKind == ControlKind.Operational
                    && p.IsActive,
                ct
            );

        if (mix is not null && line is not null && opProgram is not null)
        {
            var product = new ProductBatch
            {
                Number = $"ПП-DEMO-{DateTime.UtcNow:yyyyMMdd}",
                Kind = ProductionBatchKind.Mix,
                NomenclatureId = mix.Id,
                ProductionLineId = line.Id,
                ProductionDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Status = BatchStatus.AwaitingControl,
            };
            _db.ProductBatches.Add(product);
            _db.TraceabilityLinks.Add(
                new TraceabilityLink
                {
                    ProductBatchId = product.Id,
                    MaterialBatchId = batch.Id,
                    Quantity = 2,
                }
            );
        }

        await _db.SaveChangesAsync(ct);

        return new DemoDataResult
        {
            Message =
                $"Созданы демо-данные: партия {batch.Number}, распоряжение {order.Number}, {runs} испытаний на АРМ.",
            BatchNumber = batch.Number,
            OrderNumber = order.Number,
            TestRuns = runs,
        };
    }
}
