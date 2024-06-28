namespace Chaldea.Fate.RhoAias.Dns.Server;

public interface IDnsRequestFilter
{
    Task<bool> IsExcludedAsync(string label);
}

internal class DnsRequestFilter : IDnsRequestFilter
{
    private readonly IDnsZoneManager _dnsZoneManager;
    private List<string>? _excludeList;

    public DnsRequestFilter(IDnsZoneManager dnsZoneManager)
    {
        _dnsZoneManager = dnsZoneManager;
    }

    public async Task<bool> IsExcludedAsync(string label)
    {
        if (_excludeList == null)
        {
            var zone = await _dnsZoneManager.GetDnsZoneListAsync();
            _excludeList = zone.Select(x => x.Domain).ToList();
        }

        return _excludeList.Any(r => label.EndsWith(r));
    }
}
