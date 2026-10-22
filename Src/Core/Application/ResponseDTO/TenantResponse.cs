
namespace Application.ResponseDTO
{
    public sealed record TenantResponse(
        Guid Id,
        string Name,
        string Identifier,
        string SchemaName,
        bool IsActive,
        DateTime CreatedAt);
}
