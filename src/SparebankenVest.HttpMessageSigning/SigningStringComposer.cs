using System;
using System.Linq;
using System.Text;

namespace SparebankenVest.HttpMessageSigning {
    internal static class SigningStringComposer {
        private static readonly char[] SplitValues = { '\n' };

        public static string Compose(IHttpMessage message, RequestHttpMessageSigningConfiguration config, DateTimeOffset timestamp) =>
            config.HeadersToInclude
                .Aggregate(new StringBuilder(), (builder, headerName) =>
                    builder.IncludeHeader(headerName, message, config, timestamp))
                        .ToString().Trim();

        private static StringBuilder IncludeHeader(this StringBuilder builder, string headerName, IHttpMessage message, RequestHttpMessageSigningConfiguration config, DateTimeOffset timestamp) {
            if (string.Equals(headerName, HeaderNames.RequestTarget)) {
                return builder.AppendRequestTargetHeader(message, config);
            }

            if (string.Equals(headerName, HeaderNames.Created)) {
                return builder.AppendTimestampHeader(HeaderNames.Created, timestamp);
            }

            if (string.Equals(headerName, HeaderNames.Expires) && config.Expires.HasValue) {
                return builder.AppendTimestampHeader(HeaderNames.Expires, timestamp.Add(config.Expires.Value));
            }

            return builder.AppendDefaultHeader(message, headerName);
        }

        private static StringBuilder AppendRequestTargetHeader(this StringBuilder builder, IHttpMessage message, RequestHttpMessageSigningConfiguration config) {
            var path = message.RequestUri.GetComponents(UriComponents.PathAndQuery, config.RequestTargetUriFormat);
            var method = message.Method.ToString().ToLowerInvariant();
            return builder.AppendHeader(HeaderNames.RequestTarget, $"{method} {path}");
        }

        private static StringBuilder AppendTimestampHeader(this StringBuilder builder, string name, DateTimeOffset timestamp) =>
            builder.AppendHeader(name, timestamp.ToUnixTimeSeconds().ToString());

        private static StringBuilder AppendDefaultHeader(this StringBuilder builder, IHttpMessage message, string name) {
            if (message.TryGetHeaderValues(name, out var values)) {
                // Because each header needs to be on a separate line,
                // we need to normalize line breaks into spaces instead.
                var value = string.Join(", ", values.Select(NormalizeLines).ToArray());
                return builder.AppendHeader(name, value);
            }

            throw new InvalidOperationException($"Request is missing required signature header: {name}");
        }

        private static StringBuilder AppendHeader(this StringBuilder builder, string name, string value) {
            builder.Append(name.ToLowerInvariant());
            builder.Append(": ");
            builder.Append(value);
            builder.Append('\n');
            return builder;
        }

        private static string NormalizeLines(string value) {
            if (string.IsNullOrEmpty(value)) {
                return string.Empty;
            }

            var lines = value.Split(SplitValues, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim());

            return string.Join(" ", lines);
        }
    }
}
