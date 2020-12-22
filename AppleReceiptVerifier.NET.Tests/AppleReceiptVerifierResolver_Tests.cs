﻿using System;
using AppleReceiptVerifier.NET.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AppleReceiptVerifier.NET.Tests
{
    public class AppleReceiptVerifierResolver_Tests
    {
        [Fact]
        public void Resolving_Default_Name_Throws()
        {
            var services = new ServiceCollection();
            services.AddHttpClient();
            var resolver = new AppleReceiptVerifierResolver(services.BuildServiceProvider());
            Assert.Throws<InvalidOperationException>(() => resolver.Resolve(AppleReceiptVerifierOptions.DefaultVerifierName));
        }

        [Fact]
        public void Resolves_From_Multiple_Instances()
        {
            var services = new ServiceCollection();
            services.AddAppleReceiptVerifier(c => c.AppPassword = "test_password_1", "name1");
            services.AddAppleReceiptVerifier(c => c.AppPassword = "test_password_2", "name2");
            var provider = services.BuildServiceProvider();
            var resolver = provider.GetRequiredService<IAppleReceiptVerifierResolver>();

            AppleReceiptVerifier verifier1 = (AppleReceiptVerifier) resolver.Resolve("name1");
            AppleReceiptVerifier verifier2 = (AppleReceiptVerifier) resolver.Resolve("name2");

            Assert.Equal("test_password_1", verifier1.Options.AppPassword);
            Assert.Equal("test_password_2", verifier2.Options.AppPassword);
        }
    }
}