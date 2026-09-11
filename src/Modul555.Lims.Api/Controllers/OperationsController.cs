using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modul555.Lims.Application.Catalogs;
using Modul555.Lims.Application.Common;
using Modul555.Lims.Application.Operations;
using Modul555.Lims.Infrastructure.Services;

namespace Modul555.Lims.Api.Controllers;

[ApiController]
[Authorize]
[Route("api")]
public sealed class OperationsController : ControllerBase
{
    private readonly ProgramService _programs;
    private readonly ControlFlowService _flow;
    private readonly TestRunService _runs;
    private readonly QualityService _quality;
    private readonly MetrologyService _metro;

    public OperationsController(
        ProgramService programs,
        ControlFlowService flow,
        TestRunService runs,
        QualityService quality,
        MetrologyService metro
    )
    {
        _programs = programs;
        _flow = flow;
        _runs = runs;
        _quality = quality;
        _metro = metro;
    }

    [HttpGet("control-programs")]
    public Task<PagedResult<ControlProgramListDto>> Programs(
        [FromQuery] PagedQuery q,
        CancellationToken ct
    ) => _programs.ListAsync(q, ct);

    [HttpGet("control-programs/{id:guid}")]
    public async Task<ActionResult<ControlProgramDto>> Program(Guid id, CancellationToken ct) =>
        await _programs.GetAsync(id, ct) is { } dto ? Ok(dto) : NotFound();

    [HttpPost("control-programs")]
    public Task<ControlProgramDto> CreateProgram(
        [FromBody] ControlProgramWriteDto dto,
        CancellationToken ct
    ) => _programs.UpsertAsync(null, dto, ct);

    [HttpPut("control-programs/{id:guid}")]
    public Task<ControlProgramDto> UpdateProgram(
        Guid id,
        [FromBody] ControlProgramWriteDto dto,
        CancellationToken ct
    ) => _programs.UpsertAsync(id, dto, ct);

    [HttpDelete("control-programs/{id:guid}")]
    public async Task<IActionResult> DeleteProgram(Guid id, CancellationToken ct) =>
        await _programs.DeactivateAsync(id, ct) ? NoContent() : NotFound();

    [HttpGet("material-batches")]
    public Task<PagedResult<MaterialBatchDto>> MaterialBatches(
        [FromQuery] PagedQuery q,
        CancellationToken ct
    ) => _flow.MaterialBatches(q, ct);

    [HttpPost("material-batches")]
    public Task<MaterialBatchDto> CreateMaterialBatch(
        [FromBody] MaterialBatchDto dto,
        CancellationToken ct
    ) => _flow.CreateMaterialBatch(dto, ct);

    [HttpGet("product-batches")]
    public Task<PagedResult<ProductBatchDto>> ProductBatches(
        [FromQuery] PagedQuery q,
        CancellationToken ct
    ) => _flow.ProductBatches(q, ct);

    [HttpPost("product-batches")]
    public Task<ProductBatchDto> CreateProductBatch(
        [FromBody] ProductBatchDto dto,
        CancellationToken ct
    ) => _flow.CreateProductBatch(dto, ct);

    [HttpGet("control-orders")]
    public Task<PagedResult<ControlOrderDto>> Orders(
        [FromQuery] PagedQuery q,
        [FromQuery] ControlKind? kind,
        CancellationToken ct
    ) => _flow.Orders(q, kind, ct);

    [HttpGet("control-orders/{id:guid}")]
    public async Task<ActionResult<ControlOrderDto>> Order(Guid id, CancellationToken ct) =>
        await _flow.GetOrder(id, ct) is { } dto ? Ok(dto) : NotFound();

    [HttpPost("control-orders/incoming")]
    public Task<ControlOrderDto> Incoming(
        [FromBody] CreateIncomingOrderRequest request,
        CancellationToken ct
    ) => _flow.CreateIncoming(request, ct);

    [HttpPost("control-orders/operational")]
    public Task<ControlOrderDto> Operational(
        [FromBody] CreateOperationalOrderRequest request,
        CancellationToken ct
    ) => _flow.CreateOperational(request, ct);

    [HttpGet("control-orders/{id:guid}/samples")]
    public Task<List<SampleDto>> Samples(Guid id, CancellationToken ct) => _flow.Samples(id, ct);

    [HttpPost("control-orders/{id:guid}/samples")]
    public Task<SampleDto> RegisterSample(
        Guid id,
        [FromBody] RegisterSampleRequest request,
        CancellationToken ct
    ) => _flow.RegisterSample(id, request, ct);

    [HttpPost("samples/{id:guid}/state")]
    public async Task<IActionResult> SampleState(
        Guid id,
        [FromBody] SampleState to,
        [FromQuery] string? comment,
        CancellationToken ct
    )
    {
        await _flow.ChangeSampleState(id, to, comment, ct);
        return NoContent();
    }

    [HttpGet("control-orders/{id:guid}/tests")]
    public Task<List<TestRunDto>> OrderTests(Guid id, CancellationToken ct) =>
        _runs.ForOrder(id, ct);

    [HttpGet("workplace")]
    public Task<List<TestRunDto>> Workplace([FromQuery] Guid? assigneeId, CancellationToken ct) =>
        _runs.Workplace(assigneeId, ct);

    [HttpGet("tests/{id:guid}")]
    public async Task<ActionResult<TestRunDto>> Test(Guid id, CancellationToken ct) =>
        await _runs.Get(id, ct) is { } dto ? Ok(dto) : NotFound();

    [HttpPost("tests/{id:guid}/assign")]
    public async Task<IActionResult> Assign(
        Guid id,
        [FromBody] AssignTestRunRequest request,
        CancellationToken ct
    )
    {
        await _flow.Assign(id, request, ct);
        return NoContent();
    }

    [HttpPost("tests/{id:guid}/measurements")]
    public Task<TestRunDto> Save(
        Guid id,
        [FromBody] SaveMeasurementsRequest request,
        CancellationToken ct
    ) => _runs.SaveMeasurements(id, request, ct);

    [HttpPost("tests/{id:guid}/complete")]
    public Task<TestRunDto> Complete(Guid id, CancellationToken ct) => _runs.Complete(id, ct);

    [HttpGet("control-orders/{id:guid}/protocols")]
    public Task<List<ProtocolDto>> Protocols(Guid id, CancellationToken ct) =>
        _quality.ForOrder(id, ct);

    [HttpGet("protocols")]
    public Task<PagedResult<ProtocolDto>> ProtocolList(
        [FromQuery] PagedQuery q,
        CancellationToken ct
    ) => _quality.List(q, ct);

    [HttpPost("control-orders/{id:guid}/protocols")]
    public Task<ProtocolDto> Issue(Guid id, [FromQuery] DocumentKind kind, CancellationToken ct) =>
        _quality.Issue(id, kind, ct);

    [HttpPost("protocols/{id:guid}/sign")]
    public Task<ProtocolDto> Sign(
        Guid id,
        [FromBody] SignProtocolRequest request,
        CancellationToken ct
    ) => _quality.Sign(id, request.Role, ct);

    [HttpGet("nonconformances")]
    public Task<PagedResult<NonconformanceDto>> Nonconformances(
        [FromQuery] PagedQuery q,
        CancellationToken ct
    ) => _quality.Nonconformances(q, ct);

    [HttpPost("nonconformances/{id:guid}/decide")]
    public Task<NonconformanceDto> Decide(
        Guid id,
        [FromBody] DecideNonconformanceRequest request,
        CancellationToken ct
    ) => _quality.Decide(id, request, ct);

    [HttpPost("nonconformances/{id:guid}/actions")]
    public Task<CorrectiveActionDto> AddAction(
        Guid id,
        [FromBody] CreateCorrectiveActionRequest request,
        CancellationToken ct
    ) => _quality.AddAction(id, request, ct);

    [HttpPost("actions/{id:guid}/close")]
    public async Task<IActionResult> CloseAction(
        Guid id,
        [FromBody] string? result,
        CancellationToken ct
    )
    {
        await _quality.CloseAction(id, result, ct);
        return NoContent();
    }

    [HttpGet("passport")]
    public Task<PassportDto> Passport(
        [FromQuery] Guid? productId,
        [FromQuery] Guid? materialId,
        CancellationToken ct
    ) => _quality.Passport(productId, materialId, ct);

    [HttpPost("traceability")]
    public Task<TraceabilityLinkDto> Link(
        [FromQuery] Guid productId,
        [FromQuery] Guid materialId,
        [FromQuery] decimal? quantity,
        CancellationToken ct
    ) => _quality.Link(productId, materialId, quantity, ct);

    [HttpGet("dashboard")]
    public Task<DashboardDto> Dashboard(CancellationToken ct) => _quality.Dashboard(ct);

    [HttpGet("journals/incoming")]
    public Task<List<IncomingJournalRow>> IncomingJournal(CancellationToken ct) =>
        _quality.IncomingJournal(ct);

    [HttpGet("metrology/reminders")]
    public Task<IReadOnlyList<EquipmentDto>> Reminders(CancellationToken ct) =>
        _metro.RefreshAndRemind(ct);

    [HttpGet("equipment/{id:guid}/events")]
    public Task<List<EquipmentEventDto>> Events(Guid id, CancellationToken ct) =>
        _metro.Events(id, ct);

    [HttpPost("equipment/events")]
    public Task<EquipmentEventDto> RegisterEvent(
        [FromBody] EquipmentEventDto dto,
        CancellationToken ct
    ) => _metro.RegisterEvent(dto, ct);
}
