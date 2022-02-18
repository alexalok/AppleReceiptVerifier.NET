using System;
using AppleReceiptVerifierNET;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection.Extensions
{
    public static class ContainerExtensions
    {
        public static IServiceCollection AddAppleReceiptVerifier(this IServiceCollection services, IConfigurationSection configSection)
        {
            if (configSection == null)
                throw new ArgumentNullException(nameof(configSection));

            return services.AddAppleReceiptVerifier(configSection, AppleReceiptVerifierOptions.DefaultVerifierName);
        }

        public static IServiceCollection AddAppleReceiptVerifier(this IServiceCollection services, IConfigurationSection configSection, string name)
        {
            if (configSection == null)
                throw new ArgumentNullException(nameof(configSection));

            return services.AddAppleReceiptVerifier(configSection, null, name);
        }

        public static IServiceCollection AddAppleReceiptVerifier(this IServiceCollection services, Action<AppleReceiptVerifierOptions> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            return services.AddAppleReceiptVerifier(configure, AppleReceiptVerifierOptions.DefaultVerifierName);
        }
        
        public static IServiceCollection AddAppleReceiptVerifier(this IServiceCollection services, Action<IServiceProvider, AppleReceiptVerifierOptions> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            return services.AddAppleReceiptVerifier(configure, AppleReceiptVerifierOptions.DefaultVerifierName);
        }

        public static IServiceCollection AddAppleReceiptVerifier(this IServiceCollection services, Action<AppleReceiptVerifierOptions> configure, string name)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var configureProxy = new Action<IServiceProvider, AppleReceiptVerifierOptions>((_, opt) => configure(opt)); 

            return services.AddAppleReceiptVerifier(configureProxy, name);
        }
        
        public static IServiceCollection AddAppleReceiptVerifier(this IServiceCollection services, Action<IServiceProvider, AppleReceiptVerifierOptions> configure, string name)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            return services.AddAppleReceiptVerifier(null, configure, name);
        }

        public static IServiceCollection AddAppleReceiptVerifier(this IServiceCollection services, IConfigurationSection? configSection, 
            Action<IServiceProvider, AppleReceiptVerifierOptions>? configure, 
            string name = AppleReceiptVerifierOptions.DefaultVerifierName)
        {
            if (configSection == null && configure == null)
                throw new InvalidOperationException($"At least {nameof(configSection)} or {nameof(configure)} must be provided.");

            bool isDefaultName = name == AppleReceiptVerifierOptions.DefaultVerifierName;
            var optionsBuilder = services.AddOptions<AppleReceiptVerifierOptions>(isDefaultName ? Options.Options.DefaultName : AppleReceiptVerifierOptions.ServicesPrefix + name);
            if (configSection != null)
                optionsBuilder.Bind(configSection); // first config
            if (configure != null)
                optionsBuilder.Configure((AppleReceiptVerifierOptions opt, IServiceProvider s) => configure(s, opt)); // then explicit options
            optionsBuilder.Validate(o => !string.IsNullOrWhiteSpace(o.AppSecret),
                $"{nameof(AppleReceiptVerifierOptions.AppSecret)} must have a non-empty value.");

            if (isDefaultName)
                services.AddHttpClient<IAppleReceiptVerifier, AppleReceiptVerifier>();
            else
            {
                services.AddHttpClient(AppleReceiptVerifierOptions.ServicesPrefix + name);
                services.TryAddScoped<IAppleReceiptVerifierResolver, AppleReceiptVerifierResolver>(); // don't use Singleton here or else we'd exhaust HttpClients' pool
            }

            return services;
        }
    }
}