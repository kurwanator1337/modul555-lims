using Microsoft.EntityFrameworkCore;
using Modul555.Lims.Infrastructure.Persistence;

namespace Modul555.Lims.Infrastructure.Services;

internal static class Numbering
{
    public static async Task<string> NextAsync(
        LimsDbContext db,
        string prefix,
        Func<LimsDbContext, IQueryable<string>> selector,
        CancellationToken ct
    )
    {
        var year = DateTime.UtcNow.Year;
        var stamp = $"{prefix}-{year}-";
        var last = await selector(db)
            .Where(n => n.StartsWith(stamp))
            .OrderByDescending(n => n)
            .FirstOrDefaultAsync(ct);
        var next = 1;
        if (last is not null)
        {
            var tail = last[stamp.Length..];
            if (int.TryParse(tail, out var n))
                next = n + 1;
        }
        return $"{stamp}{next:0000}";
    }
}
