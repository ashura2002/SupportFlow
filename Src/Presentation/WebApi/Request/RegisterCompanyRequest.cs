namespace WebApi.Request
{
    public sealed record RegisterCompanyRequest(
        string TenantName,
        string Identifier,
        string FirstName,
        string LastName,
        string Password,
        string Email);
}
