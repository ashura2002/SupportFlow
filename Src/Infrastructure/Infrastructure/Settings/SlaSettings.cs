
namespace Infrastructure.Settings;

public sealed class SlaSettings
{
    public const string SectionName = "Sla";
    public int ResolutionHours { get; init; } = 24;
}
