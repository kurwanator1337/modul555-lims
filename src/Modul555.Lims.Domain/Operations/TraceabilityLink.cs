using Modul555.Lims.Domain.Common;

namespace Modul555.Lims.Domain.Operations;

/// <summary>
/// Связь партии изделия с партией сырья.
/// Обеспечивает двустороннюю прослеживаемость:
/// от изделия к сырью и от сырья к перечню изделий.
/// </summary>
public class TraceabilityLink : Entity
{
    public Guid ProductBatchId { get; set; }
    public ProductBatch? ProductBatch { get; set; }

    public Guid MaterialBatchId { get; set; }
    public MaterialBatch? MaterialBatch { get; set; }

    /// <summary>Количество сырья, израсходованного на данную производственную партию.</summary>
    public decimal? Quantity { get; set; }
}
