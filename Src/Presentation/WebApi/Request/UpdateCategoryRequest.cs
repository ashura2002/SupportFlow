namespace WebApi.Request
{
    public sealed record UpdateCategoryRequest(
        string Name,
        string? Description);
}
