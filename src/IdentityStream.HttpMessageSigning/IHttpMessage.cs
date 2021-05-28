using System;
using System.Collections.Generic;
using System.Net.Http;

namespace IdentityStream.HttpMessageSigning {
    public interface IHttpMessage {
        HttpContent? Content { get; }

        HttpMethod Method { get; }

        Uri RequestUri { get; }

        void SetHeader(string name, string value);

        bool TryGetHeaderValues(string name, out IReadOnlyCollection<string>? values);
    }
}
