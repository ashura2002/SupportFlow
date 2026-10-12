using Application.Interfaces.Services;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public sealed class SlaPolicyService : ISlaPolicy
{
    private readonly SlaSettings _slaSettings;
    public SlaPolicyService(IOptions<SlaSettings> options)
    {
        _slaSettings = options.Value;
    }

    public int ResolutionHours => _slaSettings.ResolutionHours;
}
