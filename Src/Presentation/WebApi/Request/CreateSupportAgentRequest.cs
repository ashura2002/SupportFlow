namespace WebApi.Request
{
    public sealed record CreateSupportAgentRequest(
        string FirstName,
        string LastName,
        string Password,
        string Email);
}
