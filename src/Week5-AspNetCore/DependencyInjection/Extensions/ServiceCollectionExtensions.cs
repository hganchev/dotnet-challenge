using DependencyInjection.Options;
using DependencyInjection.Services;

namespace DependencyInjection.Extensions;

/// <summary>
/// Encapsulates all registrations for "MyFeature" behind a single extension method,
/// keeping <c>Program.cs</c> declarative instead of a long list of raw <c>Add*</c> calls.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMyFeature(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MyFeatureOptions>(configuration.GetSection(MyFeatureOptions.SectionName));

        services.AddTransient<ITransientService, TransientService>();
        services.AddScoped<IScopedService, ScopedService>();
        services.AddSingleton<ISingletonService, SingletonService>();

        services.AddSingleton<IBuggyCaptiveDependencyService, BuggyCaptiveDependencyService>();
        services.AddSingleton<IFixedCaptiveDependencyService, FixedCaptiveDependencyService>();

        services.AddKeyedSingleton<IGreeter, EnglishGreeter>("en");
        services.AddKeyedSingleton<IGreeter, BulgarianGreeter>("bg");

        return services;
    }
}
