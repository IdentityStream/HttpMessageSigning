using SparebankenVest.HttpMessageSigning;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace System.Net.Http {
    /// <summary>
    /// Extensions to sign a <see cref="HttpRequestMessage"/>.
    /// </summary>
    public static class HttpRequestMessageExtensions {
        /// <summary>
        /// Signs the specified <paramref name="message"/> using the specified <paramref name="config"/>.
        /// </summary>
        /// <param name="message">The message to sign.</param>
        /// <param name="config">The configuration to use when signing.</param>
        public static Task SignAsync(this HttpRequestMessage message, HttpMessageSigningConfiguration config) =>
            HttpMessageSigner.SignAsync(new HttpMessage(message), config);

        private class HttpMessage : IHttpMessage {
            public HttpMessage(HttpRequestMessage request) {
                Request = request;
            }

            public HttpContent? Content => Request.Content;

            public HttpMethod Method => Request.Method;

            public Uri RequestUri => Request.RequestUri!;

            private HttpRequestMessage Request { get; }

            public void SetHeader(string name, string value) =>
                Request.Headers.TryAddWithoutValidation(name, value);

            public bool TryGetHeaderValues(string name, [NotNullWhen(true)] out IEnumerable<string>? values) =>
                Request.Headers.TryGetValues(name, out values);

            public void SetProperty(string name, string value) =>
#if NETSTANDARD2_0
                Request.Properties[name] = value;
#else
                Request.Options.Set(new HttpRequestOptionsKey<string>(name), value);
#endif
        }
    }
}
