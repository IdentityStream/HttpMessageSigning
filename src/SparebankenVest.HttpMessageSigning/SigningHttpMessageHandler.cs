using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SparebankenVest.HttpMessageSigning {
    /// <summary>
    /// A delegating handler to be used with <see cref="HttpClient"/> for signing HTTP requests.
    /// </summary>
    public class SigningHttpMessageHandler : DelegatingHandler {
        /// <summary>
        /// Creates an instance of <see cref="SigningHttpMessageHandler"/> with the specified <paramref name="config"/>.
        /// </summary>
        /// <param name="config">The configuration to use when signing HTTP requests.</param>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="config"/> is invalid.</exception>
        public SigningHttpMessageHandler(HttpMessageSigningConfiguration config)
            : this(config, new HttpClientHandler()) {
        }

        /// <summary>
        /// Creates an instance of <see cref="SigningHttpMessageHandler"/> with the specified <paramref name="config"/>
        /// and <paramref name="innerHandler"/>.
        /// </summary>
        /// <param name="config">The configuration to use when signing HTTP requests.</param>
        /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
        public SigningHttpMessageHandler(HttpMessageSigningConfiguration config, HttpMessageHandler innerHandler)
            : base(innerHandler) {
            Config = config?.Validate() ?? throw new ArgumentNullException(nameof(config));
        }

        private HttpMessageSigningConfiguration Config { get; }

        /// <inheritdoc/>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            await request.SignAsync(Config).ConfigureAwait(false);
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
