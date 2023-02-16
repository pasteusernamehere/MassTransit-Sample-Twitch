namespace Sample.Api;

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