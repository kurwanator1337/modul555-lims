namespace Modul555.Lims.Domain.Common;

/// <summary>Сравнение фактического значения показателя с нормативом программы испытаний.</summary>
public static class VerdictRules
{
    public static Verdict Evaluate(
        NormKind kind,
        decimal? actual,
        decimal? min,
        decimal? max,
        decimal? tolerance,
        decimal? percentOfDesign,
        decimal? designValue
    )
    {
        if (kind == NormKind.Expert)
            return Verdict.Pending;

        if (kind == NormKind.MustBeTrue)
            return actual is 1m ? Verdict.Conforms : Verdict.NotConforms;

        if (actual is null)
            return Verdict.Pending;

        var value = actual.Value;

        return kind switch
        {
            NormKind.Min => EvaluateMin(value, min),
            NormKind.Max => EvaluateMax(value, max),
            NormKind.Range => EvaluateRange(value, min, max),
            NormKind.Nominal => EvaluateNominal(value, min, tolerance),
            NormKind.PercentOfDesign => EvaluatePercent(value, percentOfDesign, designValue),
            _ => Verdict.Pending,
        };
    }

    private static Verdict EvaluateMin(decimal value, decimal? min) =>
        min is null ? Verdict.Pending
        : value >= min.Value ? Verdict.Conforms
        : Verdict.NotConforms;

    private static Verdict EvaluateMax(decimal value, decimal? max) =>
        max is null ? Verdict.Pending
        : value <= max.Value ? Verdict.Conforms
        : Verdict.NotConforms;

    private static Verdict EvaluateRange(decimal value, decimal? min, decimal? max)
    {
        if (min is null && max is null)
            return Verdict.Pending;
        if (min is not null && value < min.Value)
            return Verdict.NotConforms;
        if (max is not null && value > max.Value)
            return Verdict.NotConforms;
        return Verdict.Conforms;
    }

    private static Verdict EvaluateNominal(decimal value, decimal? nominal, decimal? tolerance)
    {
        if (nominal is null)
            return Verdict.Pending;
        var t = tolerance ?? 0m;
        return value >= nominal.Value - t && value <= nominal.Value + t
            ? Verdict.Conforms
            : Verdict.NotConforms;
    }

    private static Verdict EvaluatePercent(decimal value, decimal? percent, decimal? design)
    {
        if (percent is null || design is null || design.Value == 0)
            return Verdict.Pending;
        var threshold = design.Value * percent.Value / 100m;
        return value >= threshold ? Verdict.Conforms : Verdict.NotConforms;
    }

    public static string FormatNorm(
        NormKind kind,
        decimal? min,
        decimal? max,
        decimal? tolerance,
        decimal? percent,
        string? unit,
        string? text
    )
    {
        var u = string.IsNullOrWhiteSpace(unit) ? string.Empty : " " + unit;
        return kind switch
        {
            NormKind.Min => $"не менее {min}{u}",
            NormKind.Max => $"не более {max}{u}",
            NormKind.Range => $"{min}…{max}{u}",
            NormKind.Nominal => $"{min} ± {tolerance}{u}",
            NormKind.PercentOfDesign => $"не менее {percent}% от проектного",
            NormKind.MustBeTrue => "должно выполняться",
            NormKind.Expert => text ?? "экспертная оценка",
            _ => text ?? string.Empty,
        };
    }
}
