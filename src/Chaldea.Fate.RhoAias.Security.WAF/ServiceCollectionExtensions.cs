using Chaldea.Fate.RhoAias;
using Chaldea.Fate.RhoAias.Security.WAF;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IRhoAiasConfigurationBuilder AddRhoAiasWAF(this IRhoAiasConfigurationBuilder builder)
    {
        builder.Services.AddSingleton<IDataSeeder, WAFDataSeeder>();
        return builder;
    }

    public static IRhoAiasApplicationBuilder UseRhoAiasWAF(this IRhoAiasApplicationBuilder app)
    {
        app.ApplicationBuilder.UseMiddleware<WAFMiddleware>();
        return app;
    }
}