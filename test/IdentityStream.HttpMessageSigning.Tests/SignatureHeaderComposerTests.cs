using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace IdentityStream.HttpMessageSigning.Tests {
    [UsesVerify]
    public class SignerTests {
        private const string KeyId = "d4db0d";

        public SignerTests() {
            Settings = new VerifySettings();
            Settings.UseDirectory("Snapshots");
            Settings.UniqueForRuntime();
        }

        private VerifySettings Settings { get; }

        [Fact]
        public async Task DefaultConfiguration_ProducesCorrectSignatureHeader() {
            var message = new TestHttpMessage(HttpMethod.Post, new Uri("https://identitystream.com/hello"));

            await SignAsync(message);

            await VerifySignatureHeader(message);
        }

        [Fact]
        public async Task AddRecommendedHeaders_False_DoesNotIncludeAnyHeaders() {
            var message = new TestHttpMessage(HttpMethod.Post, new Uri("https://identitystream.com/hello"));

            await SignAsync(message, config => {
                config.AddRecommendedHeaders = false;
            });

            await VerifySignatureHeader(message);
        }

        [Fact]
        public async Task CustomHeaders_AreIncludedInSignature() {
            var message = new TestHttpMessage(HttpMethod.Post, new Uri("https://identitystream.com/hello")) {
                Headers = {
                    { "X-Custom-Header", new[] { "Hello" } }
                }
            };

            await SignAsync(message, config => {
                config.HeadersToInclude.Add("X-Custom-Header");
                config.AddRecommendedHeaders = false;
            });

            await VerifySignatureHeader(message);
        }

        [Fact]
        public async Task Digest_IsIncludedInSignature() {
            var message = new TestHttpMessage(HttpMethod.Post, new Uri("https://identitystream.com/hello"), new StringContent("hello"));

            await SignAsync(message, config => {
                config.DigestAlgorithm = HashAlgorithmName.SHA512;
                config.HeadersToInclude.Add(HeaderNames.Digest);
                config.AddRecommendedHeaders = false;
            });

            await VerifySignatureHeader(message);
        }

        [Theory]
        [MemberData(nameof(HashAlgorigthms))]
        public async Task DigestHeader_IsAddedToRequest(HashAlgorithmName digestAlgorithm) {
            var message = new TestHttpMessage(HttpMethod.Post, new Uri("https://identitystream.com/hello"), new StringContent("hello"));

            await SignAsync(message, config => {
                config.DigestAlgorithm = digestAlgorithm;
                config.HeadersToInclude.Add(HeaderNames.Digest);
                config.AddRecommendedHeaders = false;
            });

            Settings.UseParameters(digestAlgorithm);

            await Verify(message.Headers[HeaderNames.Digest].Single());
        }

        public static IEnumerable<object[]> HashAlgorigthms {
            get {
                yield return new object[] { HashAlgorithmName.SHA1 };
                yield return new object[] { HashAlgorithmName.SHA256 };
                yield return new object[] { HashAlgorithmName.SHA384 };
                yield return new object[] { HashAlgorithmName.SHA512 };
            }
        }

        [Fact]
        public async Task Expires_IsIncluded_WhenEnabled() {
            var message = new TestHttpMessage(HttpMethod.Post, new Uri("https://identitystream.com/hello"));

            await SignAsync(message, config => {
                config.HeadersToInclude.Add(HeaderNames.Expires);
                config.Expires = TimeSpan.FromMinutes(10);
                config.AddRecommendedHeaders = false;
            });

            await VerifySignatureHeader(message);
        }

        [Fact]
        public async Task MissingHeader_Throws() {
            var message = new TestHttpMessage(HttpMethod.Post, new Uri("https://identitystream.com/hello"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => SignAsync(message, config => {
                config.HeadersToInclude.Add("X-Missing-Header");
                config.AddRecommendedHeaders = false;
            }));
        }

        private Task VerifySignatureHeader(IHttpMessage message) {
            if (message.TryGetHeaderValues(HeaderNames.Signature, out var values)) {
                return Verify(values!.Single());
            }
            throw new InvalidOperationException("Could not find Signature header on request.");
        }

        private Task Verify(string value) => Verifier.Verify(value, Settings);

        private static Task SignAsync(IHttpMessage message, Action<HttpMessageSigningConfiguration>? configure = null) {
            var signatureAlgorithm = new TestSignatureAlgorithm(HashAlgorithmName.SHA512);
            var config = HttpMessageSigningConfiguration.Create(KeyId, signatureAlgorithm, config => {
                config.GetUtcNow = () => new DateTimeOffset(2021, 05, 27, 10, 23, 00, TimeSpan.Zero);
                configure?.Invoke(config);
            });
            return Signer.SignAsync(message, config);
        }
    }
}
