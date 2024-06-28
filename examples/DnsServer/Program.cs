using Chaldea.Fate.RhoAias;
using DnsServer;
using DnsServer.Domains;
using DnsServer.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.AccessControl;
using System.Text.RegularExpressions;

var builder = Host.CreateDefaultBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddRhoAias(hostContext.Configuration);
        services.AddRhoAiasDnsServer();
        services.Configure<DnsServerOptions>(o =>
        {
            o.ExcludeForwardRequests.Add(new Regex("^.*example\\.com$"));
        });
    });
var host = builder.Build();

var dataSeeder = host.Services.GetRequiredService<IDataSeeder>();
await dataSeeder.SeedAsync();

// var manager = host.Services.GetRequiredService<IDnsZoneManager>();
// manager.CreateDnsZoneAsync(new DnsZone
// {
//     Domain = "example.com",
//     ResourceRecords = new List<DnsRecord>
//     {
//         new DnsRecord
//         {
//             ResourceType = DnsRecordType.A,
//             Address = "127.0.0.1"
//         },
//         new DnsRecord
//         {
//             ResourceType = DnsRecordType.A,
//             SubZoneName = "www",
//             Address = "127.0.0.1"
//         },
//     }
// });
var manager = host.Services.GetRequiredService<IDnsZoneRepository>();
manager.AddZone("example.com", CancellationToken.None);
manager.UpdateZone(new DNSZone("example.com")
{
    ResourceRecords = new List<ResourceRecord>
    {
        new AResourceRecord(3600)
        {
            Address = "127.0.0.1"
        },
        new AResourceRecord(3600, "www")
        {
            Address = "127.0.0.1"
        }
    }
});
host.Run();
