namespace WebApi.Request
{
    public sealed record CreateCategoryRequest(
        string Name,
        string? Description);
}
