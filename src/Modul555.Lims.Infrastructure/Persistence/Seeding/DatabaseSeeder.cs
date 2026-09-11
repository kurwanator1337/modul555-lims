using Microsoft.EntityFrameworkCore;

namespace Modul555.Lims.Infrastructure.Persistence.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(LimsDbContext db, CancellationToken ct = default)
    {
        if (await db.ProductionLines.AnyAsync(ct))
            return;

        SeedOrganization(db);
        SeedStandards(db);
        SeedCatalogs(db);
        SeedNomenclature(db);
        SeedMethods(db);
        SeedIncomingPrograms(db);
        SeedOperationalPrograms(db);
        SeedEquipment(db);
        SeedPersonnel(db);
        SeedDocumentTemplates(db);

        await db.SaveChangesAsync(ct);
    }

    private static Guid Id(string key) => SeedIds.Of(key);

    private static T Ref<T>(string code, string name, Action<T>? configure = null, int sort = 0)
        where T : ReferenceEntity, new()
    {
        var entity = new T
        {
            Id = Id(typeof(T).Name + ":" + code),
            Code = code,
            Name = name,
            SortOrder = sort,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = "seed",
        };
        configure?.Invoke(entity);
        return entity;
    }

    private static void SeedOrganization(LimsDbContext db)
    {
        db.ProductionLines.AddRange(
            Ref<ProductionLine>("FSM", "ФСМ — фасадные стеновые панели", sort: 1),
            Ref<ProductionLine>("STM", "СТМ — санитарно-технические модули", sort: 2),
            Ref<ProductionLine>("IM", "ИМ — инженерные модули", sort: 3),
            Ref<ProductionLine>("RC", "ЖБИ — железобетонные изделия", sort: 4)
        );

        db.Subdivisions.AddRange(
            Ref<Subdivision>("LAB", "Производственная лаборатория"),
            Ref<Subdivision>("QMS", "Отдел технического контроля"),
            Ref<Subdivision>("TECH", "Технологическая служба")
        );

        db.Warehouses.AddRange(
            Ref<Warehouse>("RAW", "Склад сырья"),
            Ref<Warehouse>("QUAR", "Зона карантина", w => w.IsQuarantineZone = true),
            Ref<Warehouse>("FG", "Склад готовой продукции"),
            Ref<Warehouse>("SAMPLES", "Хранение проб")
        );

        db.Suppliers.AddRange(
            Ref<Supplier>(
                "CEM-1",
                "Цементный завод «Восток»",
                s =>
                {
                    s.Inn = "7701000001";
                    s.ContactPerson = "Иванов П.С.";
                }
            ),
            Ref<Supplier>(
                "AGG-1",
                "Карьер «Северный»",
                s =>
                {
                    s.Inn = "7701000002";
                    s.ContactPerson = "Петрова А.В.";
                }
            ),
            Ref<Supplier>(
                "ADM-1",
                "Химпром-добавки",
                s =>
                {
                    s.Inn = "7701000003";
                }
            ),
            Ref<Supplier>(
                "REB-1",
                "Металлопрокат-Центр",
                s =>
                {
                    s.Inn = "7701000004";
                }
            )
        );
    }

    private static void SeedStandards(LimsDbContext db)
    {
        StandardDocument Std(string code, string name, StandardKind kind, int year) =>
            Ref<StandardDocument>(
                code,
                name,
                s =>
                {
                    s.Kind = kind;
                    s.Year = year;
                    s.IsCurrent = true;
                }
            );

        db.StandardDocuments.AddRange(
            Std(
                "ГОСТ 31108",
                "Цементы общестроительные. Технические условия",
                StandardKind.Requirement,
                2020
            ),
            Std("ГОСТ 30515", "Цементы. Общие технические условия", StandardKind.Requirement, 2013),
            Std("ГОСТ 30744", "Цементы. Методы испытаний", StandardKind.TestMethod, 2001),
            Std(
                "ГОСТ 310.3",
                "Цементы. Методы определения нормальной густоты, сроков схватывания и равномерности изменения объёма",
                StandardKind.TestMethod,
                1976
            ),
            Std(
                "ГОСТ 310.2",
                "Цементы. Методы определения тонкости помола",
                StandardKind.TestMethod,
                1976
            ),
            Std(
                "ГОСТ 8736",
                "Песок для строительных работ. Технические условия",
                StandardKind.Requirement,
                2014
            ),
            Std(
                "ГОСТ 8735",
                "Песок для строительных работ. Методы испытаний",
                StandardKind.TestMethod,
                1988
            ),
            Std(
                "ГОСТ 8267",
                "Щебень и гравий из плотных горных пород для строительных работ",
                StandardKind.Requirement,
                1993
            ),
            Std(
                "ГОСТ 8269.0",
                "Щебень и гравий из плотных горных пород и отходов промышленного производства. Методы физических и механических испытаний",
                StandardKind.TestMethod,
                1997
            ),
            Std(
                "ГОСТ 23732",
                "Вода для бетонов и строительных растворов. Технические условия",
                StandardKind.Requirement,
                2011
            ),
            Std(
                "ГОСТ 24211",
                "Добавки для бетонов и строительных растворов. Общие технические условия",
                StandardKind.Requirement,
                2008
            ),
            Std(
                "ГОСТ 30459",
                "Добавки для бетонов и строительных растворов. Методы определения эффективности",
                StandardKind.TestMethod,
                2008
            ),
            Std(
                "ГОСТ 34028",
                "Прокат арматурный для железобетонных конструкций. Технические условия",
                StandardKind.Requirement,
                2016
            ),
            Std(
                "ГОСТ 5781",
                "Сталь горячекатаная для армирования железобетонных конструкций",
                StandardKind.Requirement,
                1982
            ),
            Std(
                "ГОСТ Р 52544",
                "Прокат арматурный свариваемый периодического профиля классов А500С и В500С",
                StandardKind.Requirement,
                2006
            ),
            Std(
                "ГОСТ 12004",
                "Сталь арматурная. Методы испытания на растяжение",
                StandardKind.TestMethod,
                1981
            ),
            Std(
                "ГОСТ 7565",
                "Чугун, сталь и сплавы. Метод отбора проб",
                StandardKind.TestMethod,
                1981
            ),
            Std(
                "ГОСТ 25485",
                "Бетоны ячеистые. Технические условия",
                StandardKind.Requirement,
                2019
            ),
            Std(
                "ГОСТ 31359",
                "Бетоны ячеистые автоклавного твердения. Технические условия",
                StandardKind.Requirement,
                2007
            ),
            Std(
                "ГОСТ 31360",
                "Изделия стеновые неармированные из ячеистого бетона автоклавного твердения",
                StandardKind.Requirement,
                2024
            ),
            Std(
                "ГОСТ 10180",
                "Бетоны. Методы определения прочности по контрольным образцам",
                StandardKind.TestMethod,
                2012
            ),
            Std(
                "ГОСТ 17624",
                "Бетоны. Ультразвуковой метод определения прочности",
                StandardKind.TestMethod,
                2021
            ),
            Std(
                "ГОСТ 22690",
                "Бетоны. Определение прочности механическими методами неразрушающего контроля",
                StandardKind.TestMethod,
                2015
            ),
            Std("ГОСТ 10181", "Смеси бетонные. Методы испытаний", StandardKind.TestMethod, 2014),
            Std(
                "ГОСТ 13015",
                "Изделия бетонные и железобетонные для строительства. Общие технические требования",
                StandardKind.Requirement,
                2012
            ),
            Std(
                "ГОСТ 427",
                "Линейки измерительные металлические. Технические условия",
                StandardKind.TestMethod,
                1975
            ),
            Std(
                "СП 73.13330",
                "Внутренние санитарно-технические системы зданий",
                StandardKind.CodeOfPractice,
                2016
            ),
            Std("ПУЭ", "Правила устройства электроустановок", StandardKind.CodeOfPractice, 2003),
            Std("ГОСТ Р 50571", "Электроустановки низковольтные", StandardKind.Requirement, 2010),
            Std(
                "ГОСТ Р 57997",
                "Арматурные и закладные изделия сварные, соединения сварные арматуры и закладных изделий железобетонных конструкций",
                StandardKind.Requirement,
                2017
            ),
            Std(
                "ГОСТ 26633",
                "Бетоны тяжёлые и мелкозернистые. Технические условия",
                StandardKind.Requirement,
                2015
            ),
            Std(
                "ГОСТ 18105",
                "Бетоны. Правила контроля и оценки прочности",
                StandardKind.EvaluationRules,
                2018
            ),
            Std(
                "ГОСТ 10060",
                "Бетоны. Методы определения морозостойкости",
                StandardKind.TestMethod,
                2012
            ),
            Std(
                "ГОСТ 12730.5",
                "Бетоны. Методы определения водонепроницаемости",
                StandardKind.TestMethod,
                2018
            ),
            Std(
                "ГОСТ 22904",
                "Конструкции железобетонные. Магнитный метод определения толщины защитного слоя бетона",
                StandardKind.TestMethod,
                2023
            ),
            Std(
                "ISO/IEC 17025",
                "Общие требования к компетентности испытательных и калибровочных лабораторий",
                StandardKind.CodeOfPractice,
                2019
            )
        );
    }

    private static void SeedCatalogs(LimsDbContext db)
    {
        db.QualityParameters.AddRange(
            P("CEM-ACT", "Активность (прочность на сжатие в возрасте 28 сут.)", "МПа"),
            P("CEM-SET-S", "Начало схватывания", "мин", precision: 0),
            P("CEM-SET-E", "Конец схватывания", "мин", precision: 0),
            P("CEM-FIN", "Тонкость помола (остаток на сите)", "%"),
            P("CEM-SSA", "Удельная поверхность", "м2/кг", precision: 0),
            P("CEM-NC", "Нормальная густота цементного теста", "%"),
            P("CEM-SHELF", "Срок годности соблюдён", null, ValueKind.Boolean),
            P("SND-GM", "Модуль крупности", null, precision: 2),
            P("SND-GRAD", "Гранулометрический состав соответствует", null, ValueKind.Boolean),
            P("SND-MOIST", "Влажность", "%"),
            P("SND-CLAY", "Содержание глины в комках", "%"),
            P("SND-DUST", "Содержание пылевидных и глинистых частиц", "%"),
            P("GRV-FRACT", "Соответствие фракции", null, ValueKind.Boolean),
            P("GRV-MOIST", "Влажность", "%"),
            P("GRV-CLAY", "Содержание глины в комках", "%"),
            P("GRV-DUST", "Содержание пылевидных и глинистых частиц", "%"),
            P("GRV-CRUSH", "Марка по дробимости", null, precision: 0),
            P("GRV-FLAKY", "Содержание игольчатых и пластинчатых частиц", "%"),
            P("WAT-SALT", "Содержание растворимых солей", "мг/л", precision: 0),
            P("WAT-SO4", "Содержание сульфат-ионов", "мг/л", precision: 0),
            P("WAT-CL", "Содержание хлорид-ионов", "мг/л", precision: 0),
            P("WAT-SS", "Содержание взвешенных частиц", "мг/л", precision: 0),
            P("WAT-OX", "Окисляемость", "мг/л"),
            P("WAT-COL", "Цветность", "град.", precision: 0),
            P("ADM-DENS", "Плотность", "г/см3", precision: 3),
            P("ADM-SHELF", "Срок годности соблюдён", null, ValueKind.Boolean),
            P("REB-MARK", "Наличие маркировки", null, ValueKind.Boolean),
            P("REB-DIA", "Диаметр", "мм"),
            P("REB-LEN", "Длина", "мм", precision: 0),
            P(
                "REB-SURF",
                "Состояние поверхности",
                null,
                ValueKind.Enumerated,
                "без дефектов;раковины;трещины;коррозия"
            ),
            P("REB-UTS", "Временное сопротивление", "МПа", precision: 0),
            P("REB-YS", "Предел текучести", "МПа", precision: 0),
            P("REB-EL", "Относительное удлинение", "%"),
            P("MIX-DOSE", "Точность дозирования", "%"),
            P("MIX-TEMP", "Температура смеси", "°C"),
            P("MIX-FOAM", "Средняя плотность свежеприготовленной смеси", "кг/м3", precision: 0),
            P("MIX-AIR", "Содержание вовлечённого воздуха / газа", "%"),
            P("MIX-SLUMP", "Подвижность (осадка конуса)", "см"),
            P("STEAM-HEAT", "Скорость подъёма температуры", "°C/ч"),
            P("STEAM-ISO", "Температура изотермической выдержки", "°C"),
            P("STEAM-HOLD", "Продолжительность изотермической выдержки", "ч"),
            P("STEAM-COOL", "Скорость остывания", "°C/ч"),
            P("STR-DEMOULD", "Распалубочная прочность", "МПа"),
            P("STR-RELEASE", "Отпускная прочность", "МПа"),
            P("STR-28", "Проектная прочность (28 сут.)", "МПа"),
            P("DENS-AAC", "Средняя плотность газобетона", "кг/м3", precision: 0),
            P("GEO-L", "Отклонение длины", "мм"),
            P("GEO-W", "Отклонение ширины", "мм"),
            P("GEO-H", "Отклонение высоты", "мм"),
            P("MOIST-REL", "Отпускная влажность", "%"),
            P("STM-PRES", "Падение давления при опрессовке", "МПа", precision: 3),
            P("STM-LEAK", "Отсутствие протечек при проливе", null, ValueKind.Boolean),
            P("EL-INS", "Сопротивление изоляции кабеля", "МОм"),
            P("EL-CONT", "Целостность жил соответствует схеме", null, ValueKind.Boolean),
            P("EL-CB", "Корректное срабатывание автоматов, отсутствие КЗ", null, ValueKind.Boolean),
            P("EL-PH", "Корректность подключения освещения и розеток", null, ValueKind.Boolean),
            P("WELD-STR", "Прочность сварного соединения", "кН"),
            P(
                "WELD-VIS",
                "Визуальный контроль сварных швов",
                null,
                ValueKind.Enumerated,
                "без дефектов;трещины;подрезы;непровары"
            ),
            P("FROST", "Марка по морозостойкости", null, precision: 0),
            P("WATERTIGHT", "Марка по водонепроницаемости", null, precision: 0),
            P("COVER", "Толщина защитного слоя бетона", "мм")
        );

        db.MeasuredQuantities.AddRange(
            Q("MASS", "Масса", "г", "mass"),
            Q("VOLUME", "Объём", "см3", "volume"),
            Q("FORCE", "Разрушающее усилие", "кН", "force"),
            Q("AREA", "Площадь поперечного сечения", "мм2", "area"),
            Q("TIME", "Время", "с", "time"),
            Q("TEMP", "Температура", "°C", "temp"),
            Q("PRESSURE", "Давление", "МПа", "pressure"),
            Q("LENGTH", "Линейный размер", "мм", "length"),
            Q("SIEVE", "Остаток на сите", "г", "sieve"),
            Q("SLUMP", "Осадка конуса", "см", "slump"),
            Q("REBOUND", "Показание склерометра", null, "rebound"),
            Q("RESIST", "Сопротивление", "МОм", "resist"),
            Q("DENSITY", "Плотность", "г/см3", "density"),
            Q("MASS_DRY", "Масса после сушки", "г", "massDry"),
            Q("YESNO", "Результат визуального контроля", null, "pass")
        );

        db.Defects.AddRange(
            Ref<Defect>(
                "REB-PIT",
                "Раковины",
                d =>
                {
                    d.AppliesTo = "арматура";
                }
            ),
            Ref<Defect>(
                "REB-CRACK",
                "Трещины",
                d =>
                {
                    d.AppliesTo = "арматура";
                    d.IsCritical = true;
                }
            ),
            Ref<Defect>(
                "REB-COR",
                "Следы коррозии",
                d =>
                {
                    d.AppliesTo = "арматура";
                }
            ),
            Ref<Defect>(
                "WELD-CRACK",
                "Трещины шва",
                d =>
                {
                    d.AppliesTo = "сварные швы";
                    d.IsCritical = true;
                }
            ),
            Ref<Defect>(
                "WELD-UNDERCUT",
                "Подрезы",
                d =>
                {
                    d.AppliesTo = "сварные швы";
                    d.IsCritical = true;
                }
            ),
            Ref<Defect>(
                "WELD-LACK",
                "Непровары",
                d =>
                {
                    d.AppliesTo = "сварные швы";
                    d.IsCritical = true;
                }
            )
        );

        db.NonconformanceCauses.AddRange(
            Ref<NonconformanceCause>("RAW", "Несоответствие сырья", c => c.Group = "Сырьё"),
            Ref<NonconformanceCause>("DOSE", "Отклонение дозирования", c => c.Group = "Технология"),
            Ref<NonconformanceCause>(
                "STEAM",
                "Отклонение режима пропарки",
                c => c.Group = "Технология"
            ),
            Ref<NonconformanceCause>("CURE", "Недостаточная выдержка", c => c.Group = "Технология"),
            Ref<NonconformanceCause>(
                "EQUIP",
                "Неисправность оборудования",
                c => c.Group = "Оборудование"
            ),
            Ref<NonconformanceCause>("HUMAN", "Ошибка персонала", c => c.Group = "Персонал"),
            Ref<NonconformanceCause>("OTHER", "Иное", c => c.Group = "Прочее")
        );
    }

    private static QualityParameter P(
        string code,
        string name,
        string? unit,
        ValueKind kind = ValueKind.Numeric,
        string? allowed = null,
        int precision = 2
    ) =>
        Ref<QualityParameter>(
            code,
            name,
            p =>
            {
                p.UnitOfMeasure = unit;
                p.ValueKind = kind;
                p.AllowedValues = allowed;
                p.Precision = precision;
            }
        );

    private static MeasuredQuantity Q(string code, string name, string? unit, string variable) =>
        Ref<MeasuredQuantity>(
            code,
            name,
            q =>
            {
                q.UnitOfMeasure = unit;
                q.VariableName = variable;
            }
        );

    private static void SeedNomenclature(LimsDbContext db)
    {
        db.NomenclatureCategories.AddRange(
            Ref<NomenclatureCategory>("CEMENT", "Цемент", c => c.IsRawMaterial = true, 1),
            Ref<NomenclatureCategory>("SAND", "Песок", c => c.IsRawMaterial = true, 2),
            Ref<NomenclatureCategory>("GRAVEL", "Щебень", c => c.IsRawMaterial = true, 3),
            Ref<NomenclatureCategory>("WATER", "Вода для бетона", c => c.IsRawMaterial = true, 4),
            Ref<NomenclatureCategory>("ADM", "Добавки", c => c.IsRawMaterial = true, 5),
            Ref<NomenclatureCategory>(
                "REBAR",
                "Металлопрокат и арматура",
                c => c.IsRawMaterial = true,
                6
            ),
            Ref<NomenclatureCategory>("MIX", "Бетонные и ячеистые смеси", sort: 7),
            Ref<NomenclatureCategory>("PANEL", "Стеновые панели", sort: 8),
            Ref<NomenclatureCategory>("MODULE", "Модули СТМ / ИМ", sort: 9),
            Ref<NomenclatureCategory>("RC", "Железобетонные изделия", sort: 10),
            Ref<NomenclatureCategory>(
                "CONSUMABLE",
                "Расходные материалы лаборатории",
                c => c.IsRawMaterial = true,
                11
            )
        );

        Nomenclature N(
            string code,
            string name,
            string category,
            string uom,
            string? grade = null,
            string? std = null,
            int? shelf = null,
            decimal? design = null
        ) =>
            Ref<Nomenclature>(
                code,
                name,
                n =>
                {
                    n.CategoryId = Id("NomenclatureCategory:" + category);
                    n.UnitOfMeasure = uom;
                    n.Grade = grade;
                    n.RequirementStandardId = std is null ? null : Id("StandardDocument:" + std);
                    n.ShelfLifeDays = shelf;
                    n.DesignValue = design;
                }
            );

        db.Nomenclatures.AddRange(
            N("CEM-42.5", "Цемент ЦЕМ I 42,5Н", "CEMENT", "т", "ЦЕМ I 42,5Н", "ГОСТ 31108", 60),
            N("SND-M2", "Песок строительный Мк 2,0–2,5", "SAND", "м3", "Мк 2,0–2,5", "ГОСТ 8736"),
            N("GRV-5-20", "Щебень фракции 5–20", "GRAVEL", "м3", "5–20", "ГОСТ 8267"),
            N(
                "WAT-SRC",
                "Вода для бетона (постоянный источник)",
                "WATER",
                "м3",
                null,
                "ГОСТ 23732"
            ),
            N("ADM-SP", "Суперпластификатор", "ADM", "кг", null, "ГОСТ 24211", 365),
            N("REB-A500", "Арматура А500С Ø12", "REBAR", "т", "А500С Ø12", "ГОСТ Р 52544"),
            N(
                "MIX-AAC-D600",
                "Смесь ячеистого бетона D600",
                "MIX",
                "м3",
                "D600",
                "ГОСТ 25485",
                design: 600
            ),
            N("MIX-B25", "Бетонная смесь B25", "MIX", "м3", "B25", "ГОСТ 26633", design: 32.7m),
            N(
                "PNL-FSM",
                "Фасадная стеновая панель",
                "PANEL",
                "шт",
                "D600",
                "ГОСТ 31360",
                design: 2.5m
            ),
            N("MOD-STM", "Санитарно-технический модуль", "MODULE", "шт"),
            N("MOD-IM", "Инженерный модуль", "MODULE", "шт"),
            N("RC-BEAM", "Железобетонное изделие", "RC", "шт", "B25", "ГОСТ 26633", design: 32.7m),
            N("RC-CAGE", "Арматурный каркас", "RC", "шт", null, "ГОСТ Р 57997"),
            N("LAB-PAPER", "Фильтровальная бумага", "CONSUMABLE", "шт")
        );
    }

    private static void SeedMethods(LimsDbContext db)
    {
        TestMethod M(
            string code,
            string name,
            string? std,
            string? formula,
            bool destructive = false,
            int replicates = 1,
            int minutes = 30
        ) =>
            Ref<TestMethod>(
                code,
                name,
                m =>
                {
                    m.StandardDocumentId = std is null ? null : Id("StandardDocument:" + std);
                    m.Formula = formula;
                    m.IsDestructive = destructive;
                    m.DefaultReplicates = replicates;
                    m.LaborMinutes = minutes;
                }
            );

        var methods = new[]
        {
            M(
                "M-COMP",
                "Испытание на сжатие контрольных образцов",
                "ГОСТ 10180",
                "force * 1000 / area",
                destructive: true,
                replicates: 3,
                minutes: 40
            ),
            M("M-VICAT", "Сроки схватывания (прибор Вика)", "ГОСТ 310.3", null, minutes: 90),
            M("M-FIN", "Тонкость помола (остаток на сите)", "ГОСТ 310.2", "sieve / mass * 100"),
            M("M-SIEVE", "Ситовой анализ", "ГОСТ 8735", null, minutes: 60),
            M(
                "M-MOIST",
                "Влажность весовым методом",
                "ГОСТ 8735",
                "(mass - massDry) / massDry * 100"
            ),
            M("M-CRUSH", "Дробимость щебня", "ГОСТ 8269.0", null, destructive: true),
            M("M-CHEM", "Химический анализ воды", "ГОСТ 23732", null, minutes: 120),
            M("M-DENS", "Плотность ареометром", "ГОСТ 24211", null),
            M(
                "M-TENSILE",
                "Испытание арматуры на растяжение",
                "ГОСТ 12004",
                "force * 1000 / area",
                destructive: true,
                replicates: 2,
                minutes: 45
            ),
            M("M-VIS", "Визуально-измерительный контроль", null, null, minutes: 15),
            M("M-SLUMP", "Осадка конуса", "ГОСТ 10181", null),
            M("M-TEMP", "Измерение температуры", null, null, minutes: 5),
            M(
                "M-REBOUND",
                "Неразрушающий контроль прочности (склерометр)",
                "ГОСТ 22690",
                null,
                minutes: 20
            ),
            M(
                "M-US",
                "Ультразвуковой метод определения прочности",
                "ГОСТ 17624",
                null,
                minutes: 20
            ),
            M("M-PRESS", "Опрессовка системы", "СП 73.13330", null, minutes: 25),
            M("M-FLOOD", "Пролив канализации", "СП 73.13330", null, minutes: 20),
            M("M-MEG", "Измерение сопротивления изоляции", "ПУЭ", null, minutes: 20),
            M(
                "M-WELD",
                "Испытание сварного соединения",
                "ГОСТ Р 57997",
                null,
                destructive: true,
                minutes: 40
            ),
            M("M-FROST", "Морозостойкость", "ГОСТ 10060", null, destructive: true, minutes: 240),
            M("M-WT", "Водонепроницаемость", "ГОСТ 12730.5", null, minutes: 180),
            M(
                "M-COVER",
                "Толщина защитного слоя (магнитный метод)",
                "ГОСТ 22904",
                null,
                minutes: 15
            ),
            M("M-GEO", "Измерение геометрических размеров", "ГОСТ 427", null, minutes: 15),
            M("M-BOOL", "Фиксация соответствия / несоответствия", null, null, minutes: 5),
        };
        db.TestMethods.AddRange(methods);

        void Link(string method, params string[] quantities)
        {
            for (var i = 0; i < quantities.Length; i++)
            {
                db.TestMethodQuantities.Add(
                    new TestMethodQuantity
                    {
                        Id = Id($"TMQ:{method}:{quantities[i]}"),
                        TestMethodId = Id("TestMethod:" + method),
                        MeasuredQuantityId = Id("MeasuredQuantity:" + quantities[i]),
                        SortOrder = i,
                        CreatedAt = DateTimeOffset.UtcNow,
                        CreatedBy = "seed",
                    }
                );
            }
        }

        Link("M-COMP", "FORCE", "AREA");
        Link("M-VICAT", "TIME");
        Link("M-FIN", "SIEVE", "MASS");
        Link("M-SIEVE", "MASS", "SIEVE");
        Link("M-MOIST", "MASS", "MASS_DRY");
        Link("M-CRUSH", "FORCE", "MASS");
        Link("M-CHEM", "DENSITY");
        Link("M-DENS", "DENSITY");
        Link("M-TENSILE", "FORCE", "AREA", "LENGTH");
        Link("M-VIS", "YESNO", "LENGTH");
        Link("M-SLUMP", "SLUMP");
        Link("M-TEMP", "TEMP");
        Link("M-REBOUND", "REBOUND");
        Link("M-US", "TIME");
        Link("M-PRESS", "PRESSURE", "TIME");
        Link("M-FLOOD", "YESNO", "TIME");
        Link("M-MEG", "RESIST");
        Link("M-WELD", "FORCE");
        Link("M-GEO", "LENGTH");
        Link("M-BOOL", "YESNO");
        Link("M-COVER", "LENGTH");
        Link("M-FROST", "FORCE");
        Link("M-WT", "PRESSURE");
    }

    private static void SeedIncomingPrograms(LimsDbContext db)
    {
        Program(
            "PRG-CEM",
            "Входной контроль цемента",
            "CEM-42.5",
            ControlKind.Incoming,
            "ГОСТ 31108",
            ("CEM-ACT", "M-COMP", NormKind.Min, 42.5m, null, ControlFrequency.EveryBatch, 3),
            ("CEM-SET-S", "M-VICAT", NormKind.Min, 45m, null, ControlFrequency.EveryBatch, 1),
            ("CEM-SET-E", "M-VICAT", NormKind.Max, 600m, null, ControlFrequency.EveryBatch, 1),
            ("CEM-FIN", "M-FIN", NormKind.Max, 15m, null, ControlFrequency.EveryBatch, 1),
            ("CEM-NC", "M-VICAT", NormKind.Range, 24m, 30m, ControlFrequency.EveryBatch, 1),
            ("CEM-SHELF", "M-BOOL", NormKind.MustBeTrue, null, null, ControlFrequency.EveryBatch, 1)
        );

        Program(
            "PRG-SND",
            "Входной контроль песка",
            "SND-M2",
            ControlKind.Incoming,
            "ГОСТ 8736",
            ("SND-GM", "M-SIEVE", NormKind.Range, 2.0m, 2.5m, ControlFrequency.EveryBatch, 1),
            (
                "SND-GRAD",
                "M-SIEVE",
                NormKind.MustBeTrue,
                null,
                null,
                ControlFrequency.EveryBatch,
                1
            ),
            ("SND-MOIST", "M-MOIST", NormKind.Max, 10m, null, ControlFrequency.EveryBatch, 1),
            ("SND-CLAY", "M-SIEVE", NormKind.Max, 0.5m, null, ControlFrequency.EveryBatch, 1),
            ("SND-DUST", "M-SIEVE", NormKind.Max, 3m, null, ControlFrequency.EveryBatch, 1)
        );

        Program(
            "PRG-GRV",
            "Входной контроль щебня",
            "GRV-5-20",
            ControlKind.Incoming,
            "ГОСТ 8267",
            (
                "GRV-FRACT",
                "M-SIEVE",
                NormKind.MustBeTrue,
                null,
                null,
                ControlFrequency.EveryBatch,
                1
            ),
            ("GRV-MOIST", "M-MOIST", NormKind.Max, 5m, null, ControlFrequency.EveryBatch, 1),
            ("GRV-CLAY", "M-SIEVE", NormKind.Max, 0.25m, null, ControlFrequency.EveryBatch, 1),
            ("GRV-DUST", "M-SIEVE", NormKind.Max, 2m, null, ControlFrequency.EveryBatch, 1),
            ("GRV-CRUSH", "M-CRUSH", NormKind.Min, 800m, null, ControlFrequency.EveryBatch, 1),
            ("GRV-FLAKY", "M-SIEVE", NormKind.Max, 15m, null, ControlFrequency.EveryBatch, 1)
        );

        Program(
            "PRG-WAT",
            "Входной контроль воды",
            "WAT-SRC",
            ControlKind.Incoming,
            "ГОСТ 23732",
            ("WAT-OX", "M-CHEM", NormKind.Max, 15m, null, ControlFrequency.Quarterly, 1),
            ("WAT-COL", "M-CHEM", NormKind.Max, 70m, null, ControlFrequency.Quarterly, 1),
            ("WAT-SS", "M-CHEM", NormKind.Max, 200m, null, ControlFrequency.Quarterly, 1),
            ("WAT-CL", "M-CHEM", NormKind.Max, 350m, null, ControlFrequency.Quarterly, 1),
            ("WAT-SO4", "M-CHEM", NormKind.Max, 500m, null, ControlFrequency.Quarterly, 1)
        );

        Program(
            "PRG-ADM",
            "Входной контроль добавок",
            "ADM-SP",
            ControlKind.Incoming,
            "ГОСТ 24211",
            ("ADM-DENS", "M-DENS", NormKind.Range, 1.05m, 1.20m, ControlFrequency.EveryBatch, 1),
            ("ADM-SHELF", "M-BOOL", NormKind.MustBeTrue, null, null, ControlFrequency.EveryBatch, 1)
        );

        Program(
            "PRG-REB",
            "Входной контроль арматуры",
            "REB-A500",
            ControlKind.Incoming,
            "ГОСТ Р 52544",
            ("REB-MARK", "M-VIS", NormKind.MustBeTrue, null, null, ControlFrequency.Full, 1),
            ("REB-DIA", "M-VIS", NormKind.Nominal, 12m, 0.4m, ControlFrequency.Full, 3),
            ("REB-SURF", "M-VIS", NormKind.Expert, null, null, ControlFrequency.Full, 1),
            ("REB-UTS", "M-TENSILE", NormKind.Min, 600m, null, ControlFrequency.Sampled, 2),
            ("REB-YS", "M-TENSILE", NormKind.Min, 500m, null, ControlFrequency.Sampled, 2),
            ("REB-EL", "M-TENSILE", NormKind.Min, 14m, null, ControlFrequency.Sampled, 2)
        );

        void Program(
            string code,
            string name,
            string nom,
            ControlKind kind,
            string std,
            params (
                string p,
                string m,
                NormKind nk,
                decimal? min,
                decimal? max,
                ControlFrequency f,
                int r
            )[] rows
        )
        {
            var program = Ref<ControlProgram>(
                code,
                name,
                p =>
                {
                    p.NomenclatureId = Id("Nomenclature:" + nom);
                    p.ControlKind = kind;
                    p.StandardDocumentId = Id("StandardDocument:" + std);
                    p.IsApproved = true;
                    p.ApprovedAt = DateTimeOffset.UtcNow;
                    p.ApprovedBy = "seed";
                }
            );
            db.ControlPrograms.Add(program);
            for (var i = 0; i < rows.Length; i++)
            {
                var row = rows[i];
                db.ControlProgramParameters.Add(
                    new ControlProgramParameter
                    {
                        Id = Id($"CPP:{code}:{row.p}"),
                        ControlProgramId = program.Id,
                        QualityParameterId = Id("QualityParameter:" + row.p),
                        TestMethodId = Id("TestMethod:" + row.m),
                        NormKind = row.nk,
                        NormMin = row.nk is NormKind.Min or NormKind.Range or NormKind.Nominal
                            ? row.min
                            : null,
                        NormMax = row.nk is NormKind.Max ? row.min : row.max,
                        Tolerance = row.nk == NormKind.Nominal ? row.max : null,
                        Frequency = row.f,
                        Replicates = row.r,
                        Aggregation = AggregationKind.Average,
                        IsMandatory = true,
                        SortOrder = i,
                        CreatedAt = DateTimeOffset.UtcNow,
                        CreatedBy = "seed",
                    }
                );
            }
        }
    }

    private static void SeedOperationalPrograms(LimsDbContext db)
    {
        void Add(
            string code,
            string name,
            string nom,
            string line,
            string std,
            params (
                string p,
                string m,
                NormKind nk,
                decimal? a,
                decimal? b,
                ControlFrequency f,
                int r,
                decimal? pct
            )[] rows
        )
        {
            var program = Ref<ControlProgram>(
                code,
                name,
                p =>
                {
                    p.NomenclatureId = Id("Nomenclature:" + nom);
                    p.ControlKind = ControlKind.Operational;
                    p.ProductionLineId = Id("ProductionLine:" + line);
                    p.StandardDocumentId = Id("StandardDocument:" + std);
                    p.IsApproved = true;
                    p.ApprovedAt = DateTimeOffset.UtcNow;
                    p.ApprovedBy = "seed";
                }
            );
            db.ControlPrograms.Add(program);
            for (var i = 0; i < rows.Length; i++)
            {
                var row = rows[i];
                db.ControlProgramParameters.Add(
                    new ControlProgramParameter
                    {
                        Id = Id($"CPP:{code}:{row.p}"),
                        ControlProgramId = program.Id,
                        QualityParameterId = Id("QualityParameter:" + row.p),
                        TestMethodId = Id("TestMethod:" + row.m),
                        NormKind = row.nk,
                        NormMin = row.nk is NormKind.Min or NormKind.Range or NormKind.Nominal
                            ? row.a
                            : null,
                        NormMax = row.nk is NormKind.Max or NormKind.Range ? row.b : null,
                        Tolerance = row.nk == NormKind.Nominal ? row.b : null,
                        PercentOfDesign = row.pct,
                        Frequency = row.f,
                        Replicates = row.r,
                        Aggregation = AggregationKind.Average,
                        IsMandatory = true,
                        SortOrder = i,
                        CreatedAt = DateTimeOffset.UtcNow,
                        CreatedBy = "seed",
                    }
                );
            }
        }

        Add(
            "PRG-FSM-MIX",
            "Контроль смеси ячеистого бетона",
            "MIX-AAC-D600",
            "FSM",
            "ГОСТ 25485",
            ("MIX-DOSE", "M-BOOL", NormKind.Nominal, 0m, 2m, ControlFrequency.EveryBatch, 1, null),
            ("MIX-TEMP", "M-TEMP", NormKind.Range, 15m, 35m, ControlFrequency.EveryBatch, 1, null),
            (
                "MIX-FOAM",
                "M-DENS",
                NormKind.Range,
                550m,
                650m,
                ControlFrequency.EveryBatch,
                1,
                null
            ),
            ("MIX-AIR", "M-DENS", NormKind.Range, 20m, 30m, ControlFrequency.EveryBatch, 1, null)
        );

        Add(
            "PRG-FSM-PNL",
            "Операционный контроль панелей ФСМ",
            "PNL-FSM",
            "FSM",
            "ГОСТ 31360",
            (
                "STR-DEMOULD",
                "M-COMP",
                NormKind.PercentOfDesign,
                null,
                null,
                ControlFrequency.EveryBatch,
                3,
                75m
            ),
            ("STR-28", "M-COMP", NormKind.Min, 2.5m, null, ControlFrequency.EveryBatch, 3, null),
            (
                "DENS-AAC",
                "M-DENS",
                NormKind.Range,
                550m,
                650m,
                ControlFrequency.EveryBatch,
                3,
                null
            ),
            ("GEO-L", "M-GEO", NormKind.Max, null, 5m, ControlFrequency.Sampled, 3, null),
            ("GEO-W", "M-GEO", NormKind.Max, null, 3m, ControlFrequency.Sampled, 3, null),
            ("GEO-H", "M-GEO", NormKind.Max, null, 3m, ControlFrequency.Sampled, 3, null),
            ("MOIST-REL", "M-MOIST", NormKind.Max, null, 25m, ControlFrequency.Sampled, 3, null)
        );

        Add(
            "PRG-STM",
            "Операционный контроль модуля СТМ",
            "MOD-STM",
            "STM",
            "СП 73.13330",
            ("STM-PRES", "M-PRESS", NormKind.Max, null, 0.05m, ControlFrequency.Full, 1, null),
            (
                "STM-LEAK",
                "M-FLOOD",
                NormKind.MustBeTrue,
                null,
                null,
                ControlFrequency.Full,
                1,
                null
            ),
            ("EL-INS", "M-MEG", NormKind.Min, 0.5m, null, ControlFrequency.Full, 1, null),
            ("EL-CONT", "M-BOOL", NormKind.MustBeTrue, null, null, ControlFrequency.Full, 1, null),
            ("EL-CB", "M-BOOL", NormKind.MustBeTrue, null, null, ControlFrequency.Full, 1, null),
            ("EL-PH", "M-BOOL", NormKind.MustBeTrue, null, null, ControlFrequency.Full, 1, null)
        );

        Add(
            "PRG-IM",
            "Операционный контроль модуля ИМ",
            "MOD-IM",
            "IM",
            "ПУЭ",
            ("EL-INS", "M-MEG", NormKind.Min, 0.5m, null, ControlFrequency.Full, 1, null),
            ("EL-CONT", "M-BOOL", NormKind.MustBeTrue, null, null, ControlFrequency.Full, 1, null),
            ("EL-CB", "M-BOOL", NormKind.MustBeTrue, null, null, ControlFrequency.Full, 1, null),
            ("EL-PH", "M-BOOL", NormKind.MustBeTrue, null, null, ControlFrequency.Full, 1, null)
        );

        Add(
            "PRG-RC-MIX",
            "Контроль бетонной смеси ЖБИ",
            "MIX-B25",
            "RC",
            "ГОСТ 26633",
            (
                "MIX-SLUMP",
                "M-SLUMP",
                NormKind.Range,
                10m,
                16m,
                ControlFrequency.EveryBatch,
                1,
                null
            ),
            ("MIX-TEMP", "M-TEMP", NormKind.Min, 5m, null, ControlFrequency.EveryBatch, 1, null)
        );

        Add(
            "PRG-RC",
            "Операционный контроль изделий ЖБИ",
            "RC-BEAM",
            "RC",
            "ГОСТ 18105",
            (
                "STR-DEMOULD",
                "M-REBOUND",
                NormKind.PercentOfDesign,
                null,
                null,
                ControlFrequency.EveryBatch,
                3,
                50m
            ),
            (
                "STR-RELEASE",
                "M-REBOUND",
                NormKind.PercentOfDesign,
                null,
                null,
                ControlFrequency.EveryBatch,
                3,
                70m
            ),
            ("STR-28", "M-COMP", NormKind.Min, 32.7m, null, ControlFrequency.EveryBatch, 3, null),
            ("COVER", "M-COVER", NormKind.Min, 20m, null, ControlFrequency.Sampled, 3, null)
        );

        Add(
            "PRG-RC-CAGE",
            "Контроль арматурных каркасов",
            "RC-CAGE",
            "RC",
            "ГОСТ Р 57997",
            ("WELD-STR", "M-WELD", NormKind.Min, 20m, null, ControlFrequency.Sampled, 2, null),
            ("WELD-VIS", "M-VIS", NormKind.Expert, null, null, ControlFrequency.Full, 1, null)
        );
    }

    private static void SeedEquipment(LimsDbContext db)
    {
        db.EquipmentTypes.AddRange(
            Ref<EquipmentType>("PRESS", "Пресс испытательный"),
            Ref<EquipmentType>("SCALE", "Весы лабораторные"),
            Ref<EquipmentType>("VICAT", "Прибор Вика"),
            Ref<EquipmentType>("SIEVE", "Набор сит"),
            Ref<EquipmentType>("TENSILE", "Разрывная машина"),
            Ref<EquipmentType>("CALIPER", "Штангенциркуль"),
            Ref<EquipmentType>("THERM", "Термометр / пирометр"),
            Ref<EquipmentType>("REBOUND", "Склерометр"),
            Ref<EquipmentType>("US", "Ультразвуковой прибор"),
            Ref<EquipmentType>("MEGOHM", "Мегаомметр"),
            Ref<EquipmentType>("PRESS-ST", "Стенд опрессовки"),
            Ref<EquipmentType>("COVER", "Прибор магнитного контроля защитного слоя")
        );

        void Equip(
            string code,
            string name,
            string type,
            string inv,
            DateOnly until,
            EquipmentStatus status = EquipmentStatus.Operational
        )
        {
            db.Equipment.Add(
                Ref<Equipment>(
                    code,
                    name,
                    e =>
                    {
                        e.EquipmentTypeId = Id("EquipmentType:" + type);
                        e.InventoryNumber = inv;
                        e.SerialNumber = "SN-" + code;
                        e.Manufacturer = "Лабприбор";
                        e.YearOfManufacture = 2022;
                        e.Status = status;
                        e.VerificationValidUntil = until;
                        e.VerificationIntervalMonths = 12;
                    }
                )
            );
        }

        var ok = new DateOnly(2027, 3, 1);
        var soon = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(20));
        var expired = new DateOnly(2025, 12, 1);

        Equip("PRESS-01", "Пресс П-125", "PRESS", "ИНВ-1001", ok);
        Equip("SCALE-01", "Весы ВЛ-200", "SCALE", "ИНВ-1002", ok);
        Equip("VICAT-01", "Прибор Вика", "VICAT", "ИНВ-1003", ok);
        Equip("SIEVE-01", "Набор сит строительный", "SIEVE", "ИНВ-1004", ok);
        Equip("TENSILE-01", "Разрывная машина Р-50", "TENSILE", "ИНВ-1005", soon);
        Equip("CALIPER-01", "Штангенциркуль ШЦ-I-250", "CALIPER", "ИНВ-1006", ok);
        Equip("THERM-01", "Пирометр ПМ-1", "THERM", "ИНВ-1007", ok);
        Equip("REBOUND-01", "Склерометр ИПС-МГ4", "REBOUND", "ИНВ-1008", ok);
        Equip("MEG-01", "Мегаомметр Е6-32", "MEGOHM", "ИНВ-1009", ok);
        Equip("PRESS-ST-01", "Стенд опрессовки ХГВС", "PRESS-ST", "ИНВ-1010", ok);
        Equip(
            "US-01",
            "Ультразвуковой прибор УК-14П",
            "US",
            "ИНВ-1011",
            expired,
            EquipmentStatus.VerificationExpired
        );
        Equip("COVER-01", "Измеритель защитного слоя ИПА-МГ4", "COVER", "ИНВ-1012", ok);

        void LinkType(string method, string type)
        {
            db.TestMethodEquipmentTypes.Add(
                new TestMethodEquipmentType
                {
                    Id = Id($"TME:{method}:{type}"),
                    TestMethodId = Id("TestMethod:" + method),
                    EquipmentTypeId = Id("EquipmentType:" + type),
                    IsRequired = true,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = "seed",
                }
            );
        }

        LinkType("M-COMP", "PRESS");
        LinkType("M-VICAT", "VICAT");
        LinkType("M-FIN", "SIEVE");
        LinkType("M-SIEVE", "SIEVE");
        LinkType("M-MOIST", "SCALE");
        LinkType("M-TENSILE", "TENSILE");
        LinkType("M-VIS", "CALIPER");
        LinkType("M-TEMP", "THERM");
        LinkType("M-REBOUND", "REBOUND");
        LinkType("M-US", "US");
        LinkType("M-MEG", "MEGOHM");
        LinkType("M-PRESS", "PRESS-ST");
        LinkType("M-COVER", "COVER");
        LinkType("M-GEO", "CALIPER");
        LinkType("M-DENS", "SCALE");
        LinkType("M-CRUSH", "PRESS");
        LinkType("M-WELD", "TENSILE");
    }

    private static void SeedPersonnel(LimsDbContext db)
    {
        var admin = Ref<Employee>(
            "admin",
            "Администратор системы",
            e =>
            {
                e.UserName = "admin";
                e.LastName = "Администратор";
                e.FirstName = "Системы";
                e.Position = "Администратор LIMS";
                e.SubdivisionId = Id("Subdivision:LAB");
                e.WorkSchedule = "пятидневка 08:00–17:00";
            }
        );
        db.Employees.Add(admin);
        db.UserAccounts.Add(
            new UserAccount
            {
                Id = Id("UserAccount:admin"),
                EmployeeId = admin.Id,
                UserName = "admin",
                PasswordHash = "KEYCLOAK",
                Roles = string.Join(',', Roles.All),
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = "seed",
            }
        );

        var methods = new[]
        {
            "M-COMP",
            "M-VICAT",
            "M-FIN",
            "M-SIEVE",
            "M-MOIST",
            "M-TENSILE",
            "M-VIS",
            "M-SLUMP",
            "M-TEMP",
            "M-REBOUND",
            "M-GEO",
            "M-DENS",
            "M-BOOL",
            "M-CHEM",
            "M-CRUSH",
            "M-MEG",
            "M-PRESS",
            "M-FLOOD",
            "M-US",
            "M-COVER",
            "M-WELD",
        };
        foreach (var method in methods)
        {
            db.Competencies.Add(
                new Competency
                {
                    Id = Id($"CMP:admin:{method}"),
                    EmployeeId = admin.Id,
                    TestMethodId = Id("TestMethod:" + method),
                    IssuedOn = new DateOnly(2025, 2, 1),
                    ValidUntil = new DateOnly(2030, 12, 31),
                    CertificateNumber = $"АТ-admin-{method}",
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = "seed",
                }
            );
        }
    }

    private static void SeedDocumentTemplates(LimsDbContext db)
    {
        void T(
            DocumentKind kind,
            string name,
            ControlStage stage,
            string? line = null,
            bool otk = false,
            bool onNc = false,
            string performer = Roles.Laborant
        )
        {
            db.DocumentTemplates.Add(
                Ref<DocumentTemplate>(
                    kind.ToString(),
                    name,
                    t =>
                    {
                        t.Kind = kind;
                        t.Stage = stage;
                        t.ProductionLineCode = line;
                        t.PerformerRole = performer;
                        t.ApproverRole = Roles.LabHead;
                        t.RequiresQualityInspector = otk;
                        t.CreatedOnNonconformance = onNc;
                    }
                )
            );
        }

        T(DocumentKind.IncomingControlJournal, "Журнал входного контроля", ControlStage.Incoming);
        T(
            DocumentKind.IncomingProtocolCement,
            "Протокол входного контроля (цемент)",
            ControlStage.Incoming
        );
        T(
            DocumentKind.IncomingProtocolSand,
            "Протокол входного контроля (песок)",
            ControlStage.Incoming
        );
        T(
            DocumentKind.IncomingProtocolGravel,
            "Протокол входного контроля (щебень)",
            ControlStage.Incoming
        );
        T(
            DocumentKind.IncomingProtocolWater,
            "Протокол входного контроля (вода)",
            ControlStage.Incoming
        );
        T(
            DocumentKind.IncomingProtocolAdmixture,
            "Протокол входного контроля (добавки)",
            ControlStage.Incoming
        );
        T(
            DocumentKind.RebarMechanicalTestProtocol,
            "Протокол механических испытаний арматуры",
            ControlStage.Incoming
        );
        T(
            DocumentKind.RebarVisualInspectionReport,
            "Акт осмотра арматуры",
            ControlStage.Incoming,
            otk: true
        );
        T(
            DocumentKind.IncomingNonconformanceReport,
            "Акт о несоответствии (входной контроль)",
            ControlStage.Incoming,
            otk: true,
            onNc: true
        );
        T(DocumentKind.SortingReport, "Акт разбраковки", ControlStage.Incoming, otk: true);

        T(
            DocumentKind.MixParametersProtocol,
            "Протокол контроля параметров смеси",
            ControlStage.Operational,
            "FSM"
        );
        T(
            DocumentKind.ScadaVerificationReport,
            "Акт верификации данных SCADA",
            ControlStage.Operational,
            "FSM"
        );
        T(
            DocumentKind.HeatMoistureTreatmentProtocol,
            "Протокол контроля тепловлажностной обработки",
            ControlStage.Operational,
            "FSM"
        );
        T(
            DocumentKind.ControlSpecimenProtocolAfterSteaming,
            "Протокол испытания контрольных образцов (после пропарки)",
            ControlStage.Operational,
            "FSM"
        );
        T(
            DocumentKind.ControlSpecimenProtocol28Days,
            "Протокол испытания контрольных образцов (28 сут.)",
            ControlStage.Operational,
            "FSM"
        );
        T(
            DocumentKind.AeratedConcreteDensityProtocol,
            "Протокол определения средней плотности газобетона",
            ControlStage.Operational,
            "FSM"
        );
        T(
            DocumentKind.FinishedGoodsGeometryReport,
            "Акт контроля геометрии готовых изделий",
            ControlStage.Operational,
            "FSM",
            otk: true
        );
        T(
            DocumentKind.ReleaseMoistureProtocol,
            "Протокол определения отпускной влажности",
            ControlStage.Operational,
            "FSM"
        );
        T(
            DocumentKind.BatchConformityConclusion,
            "Заключение о годности партии",
            ControlStage.Operational,
            "FSM",
            otk: true
        );

        T(
            DocumentKind.PressureTestProtocol,
            "Протокол опрессовки системы ХГВС, ОМ",
            ControlStage.Operational,
            "STM",
            otk: true
        );
        T(
            DocumentKind.DrainageFloodingReport,
            "Акт пролива канализации",
            ControlStage.Operational,
            "STM",
            otk: true
        );
        T(
            DocumentKind.ElectricalMeasurementProtocol,
            "Протокол электроизмерений",
            ControlStage.Operational,
            "STM",
            otk: true,
            performer: Roles.ElectricalLaborant
        );
        T(
            DocumentKind.ModuleSystemsAcceptanceReport,
            "Акт приёмки инженерных систем модуля",
            ControlStage.Operational,
            "STM",
            otk: true
        );
        T(
            DocumentKind.ModuleConformityConclusion,
            "Заключение о годности модуля",
            ControlStage.Operational,
            "STM",
            otk: true
        );

        T(
            DocumentKind.WeldedJointTestProtocol,
            "Протокол испытаний сварных соединений",
            ControlStage.Operational,
            "RC"
        );
        T(
            DocumentKind.ConcreteMixControlProtocol,
            "Протокол контроля бетонной смеси",
            ControlStage.Operational,
            "RC"
        );
        T(
            DocumentKind.RcHeatTreatmentProtocol,
            "Протокол контроля тепловлажностной обработки ЖБИ",
            ControlStage.Operational,
            "RC"
        );
        T(
            DocumentKind.NondestructiveStrengthProtocolDemoulding,
            "Протокол неразрушающего контроля (распалубочная)",
            ControlStage.Operational,
            "RC"
        );
        T(
            DocumentKind.NondestructiveStrengthProtocolRelease,
            "Протокол неразрушающего контроля (отпускная)",
            ControlStage.Operational,
            "RC"
        );
        T(
            DocumentKind.CubeTestProtocol28Days,
            "Протокол испытания кубов (28 сут.)",
            ControlStage.Operational,
            "RC"
        );
        T(
            DocumentKind.FrostResistanceProtocol,
            "Протокол испытаний на морозостойкость",
            ControlStage.Operational,
            "RC"
        );
        T(
            DocumentKind.WaterTightnessProtocol,
            "Протокол испытаний на водонепроницаемость",
            ControlStage.Operational,
            "RC"
        );
        T(
            DocumentKind.ConcreteCoverProtocol,
            "Протокол контроля защитного слоя бетона",
            ControlStage.Operational,
            "RC"
        );
        T(
            DocumentKind.RcBatchConformityConclusion,
            "Заключение о годности партии ЖБИ",
            ControlStage.Operational,
            "RC",
            otk: true
        );

        T(
            DocumentKind.OperationalNonconformanceReport,
            "Акт о несоответствии (операционный контроль)",
            ControlStage.Operational,
            onNc: true
        );
        T(
            DocumentKind.CorrectiveActionCard,
            "Карта корректирующих действий",
            ControlStage.Operational
        );
        T(DocumentKind.NonconformanceJournal, "Журнал несоответствий", ControlStage.Operational);
        T(DocumentKind.WriteOffReport, "Акт списания", ControlStage.Operational, otk: true);
        T(
            DocumentKind.VerificationSchedule,
            "График поверки средств измерений",
            ControlStage.Incoming
        );
    }
}
