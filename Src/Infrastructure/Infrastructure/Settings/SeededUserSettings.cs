namespace Infrastructure.Settings
{
    public sealed class SeededUserSettings
    {
        public const string SectionName = "SeededUser";
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
    }
}
