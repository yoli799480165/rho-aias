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
        // await CreateDefaultDnsRootServerAsync();
        await CreateHiChinaDnsServerAsync();
    }

    public async Task CreateDefaultDnsRootServerAsync()
    {
        await _dnsRootServerManager.CreateDnsRootServerAsync(new DnsRootServer
        {
            HostName = "a.root-servers.net",
            Manager = "VeriSign, Inc.",
            IPV4Address = "198.41.0.4",
            IPV6Address = "2001:503:ba3e::2:30"
        });
        await _dnsRootServerManager.CreateDnsRootServerAsync(new DnsRootServer
        {
            HostName = "b.root-servers.net",
            Manager = "University of Southern California (ISI)",
            IPV4Address = "199.9.14.201",
            IPV6Address = "2001:500:200::b"
        });
        await _dnsRootServerManager.CreateDnsRootServerAsync(new DnsRootServer
        {
            HostName = "c.root-servers.net", 
            Manager = "Cogent Communications", 
            IPV4Address = "192.33.4.12", 
            IPV6Address = "2001:500:2::c"
        });
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
