using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace SparebankenVest.HttpMessageSigning.Tests {
    [UsesVerify]
    public class HttpMessageSignerTests {
        private const string KeyId = "d4db0d";

        [Fact]
        public async Task DefaultConfiguration_ProducesCorrectSignatureHeader() {
            var message = new TestHttpMessage(HttpMethod.Post, new Uri("https://www.spv.no/hello"));

            await SignAsync(message, config => {
                config.AddRecommendedHeaders = true;
            });

            await VerifySignatureHeader(message);
        }

        [Fact]
        public async Task HMacSigning_ProducesCorrectSignatureHeader() {
            const string secret = "TopSecret";

            var message = new TestHttpMessage(HttpMethod.Post, new Uri("https://www.spv.no/hello"));

            var hmacAlgo = SignatureAlgorithm.Create(Encoding.UTF8.GetBytes(secret), HashAlgorithmName.SHA256);

            await SignAsync(message, config => config.AddRecommendedHeaders = true, hmacAlgo);

            await VerifySignatureHeader(message);
        }

        [Fact]
        public async Task AddRecommendedHeaders_False_DoesNotIncludeAnyHeaders() {
            var message = new TestHttpMessage(HttpMethod.Post, new Uri("https://www.spv.no/hello"));

            await SignAsync(message);

            await VerifySignatureHeader(message);
        }

        [Fact]
        public async Task CustomHeaders_AreIncludedInSignature() {
            var message = new TestHttpMessage(HttpMethod.Post, new Uri("https://www.spv.no/hello")) {
                Headers = {
                    { "X-Custom-Header", new[] { "Hello" } }
                }
            };

            await SignAsync(message, config => {
                config.HeadersToInclude.Add("X-Custom-Header");
                config.AddHeaderValue("X-Other-Header", "Value");
            });

            Assert.Equal("Value", message.Headers["X-Other-Header"].Single());

            await VerifySignatureHeader(message);
        }

        [Fact]
        public async Task Digest_IsIncludedInSignature() {
            var message = new TestHttpMessage(HttpMethod.Post, new Uri("https://www.spv.no/hello"), new StringContent("hello"));

            await SignAsync(message, config => {
                config.DigestAlgorithm = HashAlgorithmName.SHA512;
            });

            await VerifySignatureHeader(message);
        }

        [Theory]
        [MemberData(nameof(HashAlgorithms))]
        public async Task DigestHeader_IsAddedToRequest(HashAlgorithmName digestAlgorithm) {
            var message = new TestHttpMessage(HttpMethod.Post, new Uri("https://www.spv.no/hello"), new StringContent("hello"));

            await SignAsync(message, config => {
                config.DigestAlgorithm = digestAlgorithm;
            });

            await Verify(message.Headers[HeaderNames.Digest].Single())
                .UseParameters(digestAlgorithm);
        }

        public static IEnumerable<object[]> HashAlgorithms {
            get {
                yield return new object[] { HashAlgorithmName.SHA1 };
                yield return new object[] { HashAlgorithmName.SHA256 };
                yield return new object[] { HashAlgorithmName.SHA512 };
            }
        }

        [Fact]
        public async Task Expires_IsIncluded_WhenEnabled() {
            var message = new TestHttpMessage(HttpMethod.Post, new Uri("https://www.spv.no/hello"));

            await SignAsync(message, config => {
                config.Expires = TimeSpan.FromMinutes(10);
            });

            await VerifySignatureHeader(message);
        }

        [Fact]
        public async Task Created_IsIncluded_WhenEnabled() {
            var message = new TestHttpMessage(HttpMethod.Post, new Uri("https://www.spv.no/hello"));

            await SignAsync(message, config => {
                config.HeadersToInclude.Add(HeaderNames.Created);
            });

            await VerifySignatureHeader(message);
        }

        [Fact]
        public async Task HostHeader_IsAddedToRequest()
        {
            var message = new TestHttpMessage(HttpMethod.Post, new Uri("https://www.spv.no/hello"));

            await SignAsync(message, config => {
                config.HeadersToInclude.Add(HeaderNames.Host);
            });

            Assert.Equal("www.spv.no", message.Headers[HeaderNames.Host].Single());
        }

        [Fact]
        public async Task MissingHeader_Throws() {
            var message = new TestHttpMessage(HttpMethod.Post, new Uri("https://www.spv.no/hello"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => SignAsync(message, config => {
                config.HeadersToInclude.Add("X-Missing-Header");
            }));
        }

        private Task VerifySignatureHeader(IHttpMessage message) {
            if (message.TryGetHeaderValues(HeaderNames.Signature, out var values)) {
                return Verify(values.Single());
            }
            throw new InvalidOperationException("Could not find Signature header on request.");
        }

        private SettingsTask Verify(string value) => Verifier.Verify(value);

        private static Task SignAsync(IHttpMessage message, Action<HttpMessageSigningConfiguration>? configure = null, ISignatureAlgorithm? signingAlgorithm = null) {
            var signatureAlgorithm = signingAlgorithm ?? new TestSignatureAlgorithm(HashAlgorithmName.SHA512);
            var config = new HttpMessageSigningConfiguration(KeyId, signatureAlgorithm) {
                GetCurrentTimestamp = () => new DateTimeOffset(2021, 05, 27, 10, 23, 00, TimeSpan.Zero),
                AddRecommendedHeaders = false,
            };
            configure?.Invoke(config);
            return HttpMessageSigner.SignAsync(message, config);
        }
    }
}
