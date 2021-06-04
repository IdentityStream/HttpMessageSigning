using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IdentityStream.HttpMessageSigning.Tests {
    public class HttpMessageSigningHandlerTests {
        [Fact]
        public async Task IncludedHeaders_AreAddedToRequest() {
            var signatureAlgorithm = new TestSignatureAlgorithm(HashAlgorithmName.SHA512);

            var config = HttpMessageSigningConfiguration.Create("d4db0d", signatureAlgorithm, config => {
                config.GetCurrentTimestamp = () => new DateTimeOffset(2021, 05, 27, 10, 23, 00, TimeSpan.Zero);
                config.DigestAlgorithm = HashAlgorithmName.SHA256;
                config.HeadersToInclude.Add(HeaderNames.Date);
                config.HeadersToInclude.Add(HeaderNames.Host);
            });

            var recorder = new RecordingHttpRequestHandler();

            var handler = new SigningHttpMessageHandler(config, recorder);

            using var client = new HttpClient(handler);

            await client.GetAsync("https://identitystream.com/hello");

            var request = recorder.Requests.Single();

            Assert.True(request.Headers.Contains(HeaderNames.Signature));
            Assert.True(request.Headers.Contains(HeaderNames.Digest));
            Assert.True(request.Headers.Contains(HeaderNames.Host));
            Assert.True(request.Headers.Contains(HeaderNames.Date));
        }

        /// <summary>
        /// A request handler that records all requests and sends back an empty 200 OK response.
        /// </summary>
        private class RecordingHttpRequestHandler : DelegatingHandler {
            public RecordingHttpRequestHandler() {
                Requests = new Queue<HttpRequestMessage>();
            }

            public Queue<HttpRequestMessage> Requests { get; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
                Requests.Enqueue(request);
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }
        }
    }
}
