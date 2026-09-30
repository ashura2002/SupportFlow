using Application.Interfaces.Services;


namespace Infrastructure.Services
{
    internal sealed class BcryptService : IPasswordService
    {
        public string HashPassword(string plainText)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainText);
        }

        public bool VerifyPassword(string password, string hashPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashPassword);
        }
    }
}
