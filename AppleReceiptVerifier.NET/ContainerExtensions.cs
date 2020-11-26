using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace AppleReceiptVerifier.NET
{
    public static class ContainerExtensions
    {
        public static IServiceCollection AddAppleReceiptVerifier(this IServiceCollection services, IConfigurationSection configSection)
        {
            if (configSection == null)
                throw new ArgumentNullException(nameof(configSection));

            return AddAppleReceiptVerifier(services, configSection, AppleReceiptVerifierOptions.DefaultVerifierName);
        }

        public static IServiceCollection AddAppleReceiptVerifier(this IServiceCollection services, IConfigurationSection configSection, string name)
        {
            if (configSection == null)
                throw new ArgumentNullException(nameof(configSection));

            return AddAppleReceiptVerifier(services, configSection, null, name);
        }

        public static IServiceCollection AddAppleReceiptVerifier(this IServiceCollection services, Action<AppleReceiptVerifierOptions> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            return AddAppleReceiptVerifier(services, configure, AppleReceiptVerifierOptions.DefaultVerifierName);
        }

        public static IServiceCollection AddAppleReceiptVerifier(this IServiceCollection services, Action<AppleReceiptVerifierOptions> configure, string name)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            return AddAppleReceiptVerifier(services, null, configure, name);
        }

        public static IServiceCollection AddAppleReceiptVerifier(this IServiceCollection services, IConfigurationSection? configSection, Action<AppleReceiptVerifierOptions>? configure, string name = AppleReceiptVerifierOptions.DefaultVerifierName)
        {
            if (configSection == null && configure == null)
                throw new InvalidOperationException($"At least {nameof(configSection)} or {nameof(configure)} must be provided.");

            bool isDefaultName = name == AppleReceiptVerifierOptions.DefaultVerifierName;
            var optionsBuilder = services.AddOptions<AppleReceiptVerifierOptions>(isDefaultName ? Options.DefaultName : AppleReceiptVerifierOptions.ServicesPrefix + name);
            if (configSection != null)
                optionsBuilder.Bind(configSection); // first config
            if (configure != null)
                optionsBuilder.Configure(configure); // then explicit options
            optionsBuilder.Validate(o => !string.IsNullOrWhiteSpace(o.AppPassword),
                $"{nameof(AppleReceiptVerifierOptions.AppPassword)} must have a non-empty value.");

            if (isDefaultName)
            {
                services.AddHttpClient<IAppleReceiptVerifier, AppleReceiptVerifier>();
            }
            else
            {
                services.AddHttpClient(AppleReceiptVerifierOptions.ServicesPrefix + name);
                services.TryAddScoped<IAppleReceiptVerifierResolver, AppleReceiptVerifierResolver>(); // don't use Singleton here or else we'd exhaust HttpClients' pool
            }

            return services;
        }
    }
}