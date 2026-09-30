namespace WebApi.Request
{
    public sealed record CreateRequesterRequest(
        string FirstName,
        string LastName,
        string Password,
        string Email);
}
