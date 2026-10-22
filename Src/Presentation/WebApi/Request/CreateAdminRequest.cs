namespace WebApi.Request
{
    public sealed record CreateAdminRequest(
        string FirstName,
        string LastName,
        string Password,
        string Email);
}
