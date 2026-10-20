
namespace Mundialito.Infrastructure.Persistence;

public sealed class IdempotencyRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Key { get; set; } = string.Empty;

    public string Operation { get; set; } = string.Empty;

    public string RequestHash { get; set; } = string.Empty;

    public int StatusCode { get; set; }

    public string ResponseBody { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
