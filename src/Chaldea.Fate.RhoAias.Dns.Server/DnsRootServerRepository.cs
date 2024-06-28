using DnsServer.Domains;
using DnsServer.Persistence;

namespace Chaldea.Fate.RhoAias.Dns.Server;

internal class DnsRootServerRepository : IDnsRootServerRepository
{
    private readonly IDnsRootServerManager _dnsRootServerManager;

    public DnsRootServerRepository(IDnsRootServerManager dnsRootServerManager)
    {
        _dnsRootServerManager = dnsRootServerManager;
    }

    public async Task<IEnumerable<DNSRootServer>> FindAll(CancellationToken token = new())
    {
        return (await _dnsRootServerManager.GetDnsRootServerListAsync())
            .Select(x => new DNSRootServer(x.HostName, x.Manager, x.IPV4Address, x.IPV6Address))
            .ToArray();
    }
}
