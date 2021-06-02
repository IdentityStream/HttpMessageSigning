using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace IdentityStream.HttpMessageSigning.Tests {
    public class TestHttpMessage : IHttpMessage {
        public TestHttpMessage(HttpMethod method, Uri requestUri, HttpContent? content = null) {
            Method = method;
            RequestUri = requestUri;
            Content = content;
            Headers = new Dictionary<string, IReadOnlyCollection<string>>(StringComparer.OrdinalIgnoreCase);
        }

        public HttpMethod Method { get; }

        public Uri RequestUri { get; }

        public HttpContent? Content { get; }

        public Dictionary<string, IReadOnlyCollection<string>> Headers { get; }

        public bool TryGetHeaderValues(string name, [NotNullWhen(true)] out IReadOnlyCollection<string>?values) =>
            Headers.TryGetValue(name, out values);

        public void SetHeader(string name, string value) =>
            Headers[name] = new[] { value };
    }
}
