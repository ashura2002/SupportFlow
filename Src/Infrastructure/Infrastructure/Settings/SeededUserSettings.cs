namespace Infrastructure.Settings
{
    public sealed class SeededUserSettings
    {
        public const string SectionName = "SeededUser";
        public required string SEEDED_USER_FIRST_NAME { get; init; }
        public required string SEEDED_USER_LAST_NAME { get; init; }
        public required string SEEDED_USER_PASSWORD { get; init; }
        public required string SEEDED_USER_EMAIL { get; init; }
    }
}
