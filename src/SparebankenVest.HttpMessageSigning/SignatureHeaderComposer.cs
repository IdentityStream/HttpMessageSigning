using System;
using System.Linq;
using System.Text;

namespace SparebankenVest.HttpMessageSigning {
    internal static class SignatureHeaderComposer {
        public static string Compose(string signatureString, RequestHttpMessageSigningConfiguration config, DateTimeOffset timestamp) {
            var builder = new StringBuilder();

            builder.Append("keyId=");
            builder.AppendQuoted(config.KeyId);

            builder.Append(",algorithm=");
            builder.AppendQuoted(config.SignatureAlgorithm.GetAlgorithmName());

            if (config.HeadersToInclude.Contains(HeaderNames.Created)) {
                builder.Append(",created=");
                builder.Append(GetTimestampString(timestamp));
            }

            if (config.Expires.HasValue) {
                builder.Append(",expires=");
                builder.Append(GetTimestampString(timestamp.Add(config.Expires.Value)));
            }

            if (config.HeadersToInclude.Count > 0) {
                builder.Append(",headers=");
                builder.AppendQuoted(GetHeaderString(config));
            }

            builder.Append(",signature=");
            builder.AppendQuoted(signatureString);

            return builder.ToString();
        }

        private static string GetTimestampString(DateTimeOffset timestamp) =>
            timestamp.ToUnixTimeSeconds().ToString();

        private static string GetHeaderString(RequestHttpMessageSigningConfiguration config) =>
            string.Join(" ", config.HeadersToInclude.Select(x => x.ToLowerInvariant()));

        private static StringBuilder AppendQuoted(this StringBuilder builder, string value) =>
            builder.Append('"').Append(value).Append('"');
    }
}