using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace SparebankenVest.HttpMessageSigning {
    /// <summary>
    /// Represents a HTTP message for signing.
    /// </summary>
    public interface IHttpMessage {
        /// <summary>
        /// The body of the HTTP message.
        /// </summary>
        HttpContent? Content { get; }

        /// <summary>
        /// The method of the HTTP message.
        /// </summary>
        HttpMethod Method { get; }

        /// <summary>
        /// The HTTP message's full URI.
        /// </summary>
        Uri RequestUri { get; }

        /// <summary>
        /// Sets the header with the specified <paramref name="name"/> and <paramref name="value"/> on the HTTP message.
        /// </summary>
        /// <param name="name">The header name.</param>
        /// <param name="value">The header value.</param>
        void SetHeader(string name, string value);

        /// <summary>
        /// Tries to get the values of a header with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The header name.</param>
        /// <param name="values">The header values.</param>
        /// <returns><c>true</c> if the header exists; otherwise <c>false</c>.</returns>
        bool TryGetHeaderValues(string name, [NotNullWhen(true)] out IEnumerable<string>? values);

        /// <summary>
        /// Sets a property on the HTTP request. Useful for debugging purposes.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        void SetProperty(string name, string value);
    }
}
