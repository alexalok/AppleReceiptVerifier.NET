using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AppleReceiptVerifier.NET
{
    public interface IAppleReceiptVerifierResolver
    {
        AppleReceiptVerifier Resolve(string verifierName);
    }

    public class AppleReceiptVerifierResolver : IAppleReceiptVerifierResolver
    {
        readonly IServiceProvider _services;
        readonly IHttpClientFactory _httpClientFactory;

        public AppleReceiptVerifierResolver(IServiceProvider services)
        {
            _services = services;
            _httpClientFactory = _services.GetRequiredService<IHttpClientFactory>();
        }

        public AppleReceiptVerifier Resolve(string verifierName)
        {
            if (verifierName == AppleReceiptVerifierOptions.DefaultVerifierName)
                throw new InvalidOperationException("Resolver can only be used when registering service implementations with non-default names.");
            verifierName = AppleReceiptVerifierOptions.ServicesPrefix + verifierName;
            var namedOptionsResolver = _services.GetRequiredService<IOptionsMonitor<AppleReceiptVerifierOptions>>();
            var namedOptions = namedOptionsResolver.Get(verifierName);
            var options = Options.Create<AppleReceiptVerifierOptions>(namedOptions);
            var httpClient = _httpClientFactory.CreateClient(verifierName);
            var instance = ActivatorUtilities.CreateInstance<AppleReceiptVerifier>(_services, options, httpClient);
            return instance;
        }
    }
}