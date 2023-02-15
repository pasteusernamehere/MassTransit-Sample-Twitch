namespace Sample.Api;

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public static class ConfigureDependentServices
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder,
        Action<IServiceCollection> configure = null)
    {
        if (configure == null)
        {
            return builder;
        }

        configure.Invoke(builder.Services);

        return builder;
    }
}