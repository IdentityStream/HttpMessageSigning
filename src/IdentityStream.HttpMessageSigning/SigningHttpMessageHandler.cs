using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityStream.HttpMessageSigning {
    /// <summary>
    /// A delegating handler to be used with <see cref="HttpClient"/> for signing HTTP requests.
    /// </summary>
    public class SigningHttpMessageHandler : DelegatingHandler {
        /// <summary>
        /// Creates an instance of <see cref="SigningHttpMessageHandler"/> with the specified <paramref name="config"/>.
        /// </summary>
        /// <param name="config">The configuration to use when signing HTTP requests.</param>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="config"/> is invalid.</exception>
        public SigningHttpMessageHandler(HttpMessageSigningConfiguration config) {
            Config = config?.Validate() ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Creates an instance of <see cref="SigningHttpMessageHandler"/> with the specified <paramref name="config"/>
        /// and <paramref name="innerHandler"/>.
        /// </summary>
        /// <param name="config">The configuration to use when signing HTTP requests.</param>
        /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
        public SigningHttpMessageHandler(HttpMessageSigningConfiguration config, HttpMessageHandler innerHandler)
            : base(innerHandler) {
            Config = config;
        }

        private HttpMessageSigningConfiguration Config { get; }

        /// <inheritdoc/>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            var message = new HttpMessage(request);

            await HttpMessageSigner.SignAsync(message, Config).ConfigureAwait(false);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        private class HttpMessage : IHttpMessage {
            public HttpMessage(HttpRequestMessage request) {
                Request = request;
            }

            public HttpContent? Content => Request.Content;

            public HttpMethod Method => Request.Method;

            public Uri RequestUri => Request.RequestUri;

            private HttpRequestMessage Request { get; }

            public void SetHeader(string name, string value) =>
                Request.Headers.TryAddWithoutValidation(name, value);

            public bool TryGetHeaderValues(string name, [NotNullWhen(true)] out IEnumerable<string>? values) =>
                Request.Headers.TryGetValues(name, out values);

            public void SetProperty(string name, string value) =>
                Request.Properties[name] = value;
        }
    }
}
