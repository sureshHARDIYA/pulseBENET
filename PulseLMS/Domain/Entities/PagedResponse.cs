namespace PulseLMS.Domain.Entities;

public sealed class PagedResponse<T>
{
	public required IReadOnlyList<T> Items { get; init; }
	public required int Page { get; init; }
	public required int PageSize { get; init; }
	public required int TotalItems { get; init; }
	public required int TotalPages { get; init; }
	public bool HasNext => Page + 1 < TotalPages;
	public bool HasPrevious => Page > 0;
	public int? NextPage => HasNext ? Page + 1 : null;
	public int? PreviousPage => HasPrevious ? Page - 1 : null;
}