using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace SparebankenVest.HttpMessageSigning.Tests {
    public class TestHttpMessage : IHttpMessage {
        public TestHttpMessage(HttpMethod method, Uri requestUri, HttpContent? content = null) {
            Method = method;
            RequestUri = requestUri;
            Content = content;
            Headers = new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase);
            Properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public HttpMethod Method { get; }

        public Uri RequestUri { get; }

        public HttpContent? Content { get; }

        public Dictionary<string, IEnumerable<string>> Headers { get; }

        public Dictionary<string, string> Properties { get; }

        public bool TryGetHeaderValues(string name, [NotNullWhen(true)] out IEnumerable<string>? values) =>
            Headers.TryGetValue(name, out values);

        public void SetHeader(string name, string value) =>
            Headers[name] = new[] { value };

        public void SetProperty(string name, string value) =>
            Properties[name] = value;
    }
}
