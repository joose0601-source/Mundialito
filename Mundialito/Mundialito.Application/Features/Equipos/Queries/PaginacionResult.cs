
namespace Mundialito.Application.Features.Equipos.Queries;

public sealed record PaginacionResult<T>(
    IReadOnlyList<T> Data,
    int PageNumber,
    int PageSize,
    int TotalRecords,
    int TotalPages);
