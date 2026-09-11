using System.Security.Cryptography;
using System.Text;

namespace Modul555.Lims.Infrastructure.Persistence.Seeding;

/// <summary>
/// Детерминированные GUID из кода сущности.
/// Повторный запуск сида не создаёт дубликаты: записи находятся по Id.
/// </summary>
internal static class SeedIds
{
    public static Guid Of(string key)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes("modul555.lims:" + key));
        Span<byte> guid = stackalloc byte[16];
        bytes.AsSpan(0, 16).CopyTo(guid);
        guid[6] = (byte)((guid[6] & 0x0F) | 0x40);
        guid[8] = (byte)((guid[8] & 0x3F) | 0x80);
        return new Guid(guid);
    }
}
