using Chaldea.Fate.RhoAias;
using Chaldea.Fate.RhoAias.Dns.Server;
using DnsServer;
using DnsServer.Domains;
using DnsServer.Persistence;
using DnsServer.Persistence.InMemory;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IRhoAiasConfigurationBuilder AddRhoAiasDnsServer(this IRhoAiasConfigurationBuilder builder)
    {
        builder.Services.AddRhoAiasDnsServer();
        return builder;
    }

    public static IServiceCollection AddRhoAiasDnsServer(this IServiceCollection services)
    {
        services.AddOptions<DnsServerOptions>("RhoAias:Dns:Server");
        services.AddOptions<RhoAiasDnsServerOptions>("RhoAias:Dns:Server");
        services.AddHostedService<DnsHostedService>();
        services.AddTransient<IDnsAuthoritativeHandler, DnsAuthoritativeHandler>();
        services.AddTransient<IDnsRecursiveHandler, DnsRecursiveHandler>();
        services.AddTransient<IDnsResolver, DnsResolver>();
        services.AddSingleton<IDnsRootServerRepository, DnsRootServerRepository>();
        services.AddSingleton<IDnsZoneRepository>(new InMemoryDnsZoneRepository(new List<DNSZone>()));
        services.AddDistributedMemoryCache();
        services.AddSingleton<IDataSeeder, DnsDataSeeder>();
        services.AddSingleton<IDnsRequestFilter, DnsRequestFilter>();
        return services;
    }
}
