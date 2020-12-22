using System;
using System.Collections.Generic;
using AppleReceiptVerifier.NET.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace AppleReceiptVerifier.NET.Tests
{
    public class ContainerExtensions_Tests
    {
        const string TestAppPassword = "test_app_password";

        [Fact]
        public void AddAppleReceiptVerifier_Reads_From_ConfigSection()
        {
            var srv = new ServiceCollection();
            var configSection = new ConfigurationBuilder()
                .AddInMemoryCollection(new[] { new KeyValuePair<string, string>($"{nameof(AppleReceiptVerifierOptions)}:{nameof(AppleReceiptVerifierOptions.AppPassword)}", TestAppPassword), })
                .Build()
                .GetSection(nameof(AppleReceiptVerifierOptions));

            srv.AddAppleReceiptVerifier(configSection);
            var provider = srv.BuildServiceProvider();

            var options = provider.GetRequiredService<IOptions<AppleReceiptVerifierOptions>>();
            Assert.Equal(TestAppPassword, options.Value.AppPassword);
        }

        [Fact]
        public void AddAppleReceiptVerifier_Reads_From_Explicit_Configuration_Delegate()
        {
            var srv = new ServiceCollection();

            srv.AddAppleReceiptVerifier(c => c.AppPassword = "test_app_password");
            var provider = srv.BuildServiceProvider();

            var options = provider.GetRequiredService<IOptions<AppleReceiptVerifierOptions>>();
            Assert.Equal(TestAppPassword, options.Value.AppPassword);
        }

        [Fact]
        public void AddAppleReceiptVerifier_Throws_When_No_Config_Sources_Provided()
        {
            var srv = new ServiceCollection();

            Assert.Throws<InvalidOperationException>(() => srv.AddAppleReceiptVerifier(null, null, AppleReceiptVerifierOptions.DefaultVerifierName));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData(null)]
        public void AddAppleReceiptVerifier_Throws_When_Empty_AppPassword_Provided(string invalidAppPassword)
        {
            var srv = new ServiceCollection();

            srv.AddAppleReceiptVerifier(c => c.AppPassword = invalidAppPassword);
            var provider = srv.BuildServiceProvider();

            Assert.Throws<OptionsValidationException>(() => provider.GetRequiredService<IOptions<AppleReceiptVerifierOptions>>().Value);
        }

        [Fact]
        public void Adding_Single_Default_Implementation_Does_Not_Add_Resolver()
        {
            var services = new ServiceCollection();
            services.AddAppleReceiptVerifier(c => c.AppPassword = "test_password");
            var provider = services.BuildServiceProvider();

            var resolver = provider.GetService<IAppleReceiptVerifierResolver>();

            Assert.Null(resolver);
        }

        [Fact]
        public void Adding_Multiple_Implementations_Adds_Resolver()
        {
            var services = new ServiceCollection();
            services.AddAppleReceiptVerifier(c => c.AppPassword = "test_password", "name1");
            services.AddAppleReceiptVerifier(c => c.AppPassword = "test_password", "name2");
            var provider = services.BuildServiceProvider();

            var resolver = provider.GetService<IAppleReceiptVerifierResolver>();

            Assert.NotNull(resolver);
        }
    }
}