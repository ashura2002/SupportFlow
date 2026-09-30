namespace Application.Interfaces.Services
{
    public interface IPasswordService
    {
        string HashPassword(string plainText);
        bool VerifyPassword(string password, string hashPassword);
    }
}
