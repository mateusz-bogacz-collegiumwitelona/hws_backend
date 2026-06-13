namespace DTO.Request;

public record PagedRequest
{
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}