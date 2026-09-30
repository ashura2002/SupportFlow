namespace WebApi.Request
{
    public sealed record PaginatedRequest(int Page = 1, int PageSize = 10);
}
