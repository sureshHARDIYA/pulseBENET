namespace PulseLMS.Domain.Entities;

public sealed class SearchRequest
{
	public int Page { get; init; } = 0;
	public int PageSize { get; init; } = 10;
	public string? Search { get; init; }
}


