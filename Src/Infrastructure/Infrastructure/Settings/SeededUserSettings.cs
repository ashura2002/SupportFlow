namespace Infrastructure.Settings
{
    public sealed class SeededUserSettings
    {
        public const string SectionName = "SeededUser";
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string Password { get; init; }
        public required string Email { get; init; }
    }
}
