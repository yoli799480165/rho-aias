namespace Chaldea.Fate.RhoAias.Dns.Server;

internal class DnsDataSeeder : IDataSeeder
{
    private readonly IDnsRootServerManager _dnsRootServerManager;

    public DnsDataSeeder(IDnsRootServerManager dnsRootServerManager)
    {
        _dnsRootServerManager = dnsRootServerManager;
    }

    public async Task SeedAsync()
    {
        await CreateHiChinaDnsServerAsync();
    }

    public async Task CreateHiChinaDnsServerAsync()
    {
        await _dnsRootServerManager.CreateDnsRootServerAsync(new DnsRootServer()
        {
            HostName = "dns1.hichina.com",
            Manager = "HiChina",
            IPV4Address = "47.118.199.203",
            IPV6Address = "2408:4009:501::9"
        });
        await _dnsRootServerManager.CreateDnsRootServerAsync(new DnsRootServer()
        {
            HostName = "dns2.hichina.com",
            Manager = "HiChina",
            IPV4Address = "139.224.142.114",
            IPV6Address = "2408:4009:501::10"
        });
    }
}
