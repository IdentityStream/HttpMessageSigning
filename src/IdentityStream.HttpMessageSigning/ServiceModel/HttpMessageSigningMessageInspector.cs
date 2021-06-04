using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;

namespace IdentityStream.HttpMessageSigning.ServiceModel {
    /// <summary>
    /// A WCF message inspector that signs HTTP requests before they're sent.
    /// </summary>
    public class HttpMessageSigningMessageInspector : IClientMessageInspector {
        private static readonly XmlWriterSettings Settings = new() {
            Encoding = Encoding.UTF8
        };

        /// <summary>
        /// Creates a new inspector instance using the specified <paramref name="config"/>.
        /// </summary>
        /// <param name="config">The configuration to use when signing HTTP messages.</param>
        public HttpMessageSigningMessageInspector(HttpMessageSigningConfiguration config) {
            Config = config;
        }

        private HttpMessageSigningConfiguration Config { get; }

        /// <inheritdoc/>
        public void AfterReceiveReply(ref Message reply, object correlationState) {
            // No implementation necessary.
        }

        /// <inheritdoc/>
        public object? BeforeSendRequest(ref Message request, IClientChannel channel) {
            var httpRequest = GetHttpRequestProperty(request);

            var httpMessage = CreateHttpMessage(httpRequest, channel, Config, ref request);

            HttpMessageSigner.SignAsync(httpMessage, Config).GetAwaiter().GetResult(); // Ugh :(

            return null;
        }

        private static IHttpMessage CreateHttpMessage(HttpRequestMessageProperty httpRequest, IClientChannel channel, HttpMessageSigningConfiguration config, ref Message request) {
            var method = new HttpMethod(httpRequest.Method);
            var requestUri = channel.RemoteAddress.Uri;
            var content = default(HttpContent?);

            if (config.DigestAlgorithm.HasValue) {
                var requestBody = ReadRequestBody(ref request);
                content = new ByteArrayContent(requestBody);
            }

            return new WcfHttpRequestMessage(method, requestUri, content, httpRequest.Headers, request.Properties);
        }

        private static byte[] ReadRequestBody(ref Message message) {
            var stream = new MemoryStream();
            var writer = XmlWriter.Create(stream, Settings);

            var buffer = message.CreateBufferedCopy(int.MaxValue);

            message = buffer.CreateMessage();

            message.WriteMessage(writer);

            message = buffer.CreateMessage();

            writer.Flush();
            stream.Flush();

            return stream.ToArray();
        }

        private static HttpRequestMessageProperty GetHttpRequestProperty(Message request) {
            if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out var property) && property is HttpRequestMessageProperty httpRequest) {
                return httpRequest;
            }

            httpRequest = new HttpRequestMessageProperty();
            request.Properties[HttpRequestMessageProperty.Name] = httpRequest;
            return httpRequest;
        }

        private class WcfHttpRequestMessage : IHttpMessage {
            private static readonly char[] SplitValues = { ',' };

            public WcfHttpRequestMessage(
                HttpMethod method,
                Uri requestUri,
                HttpContent? content,
                WebHeaderCollection headers,
                MessageProperties properties) {
                Method = method;
                RequestUri = requestUri;
                Content = content;
                Headers = headers;
                Properties = properties;
            }

            public HttpMethod Method { get; }

            public Uri RequestUri { get; }

            public HttpContent? Content { get; }

            public WebHeaderCollection Headers { get; }

            public MessageProperties Properties { get; }

            public void SetHeader(string name, string value) =>
                Headers.Set(name, value);

            public bool TryGetHeaderValues(string name, [NotNullWhen(true)] out IEnumerable<string> values) {
                var value = Headers.Get(name);

                if (value is null) {
                    values = Array.Empty<string>();
                    return false;
                }

                values = NormalizeHeaderValue(value);
                return true;
            }

            public void SetProperty(string name, string value) =>
                Properties[name] = value;

            private static IEnumerable<string> NormalizeHeaderValue(string value) =>
                value.Split(SplitValues, StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim());
        }
    }
}
