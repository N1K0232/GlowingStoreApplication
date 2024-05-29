namespace GlowingStoreApplication.Shared.Collections;

public record class ListResult<T>(IEnumerable<T> Content, long TotalCount, bool HasNextPage = false);