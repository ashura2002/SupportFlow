namespace WebApi.Request
{
    public sealed record UpdatePasswordRequest(string NewPassword, string ConfirmNewPassword);
}
