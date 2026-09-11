using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Modul555.Lims.Infrastructure.Persistence;

public sealed class LimsDbContextFactory : IDesignTimeDbContextFactory<LimsDbContext>
{
    public LimsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<LimsDbContext>()
            .UseNpgsql(
                "Host=localhost;Port=5433;Database=modul555_lims;Username=lims;Password=lims"
            )
            .Options;
        return new LimsDbContext(options);
    }
}
