using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdentityStream.HttpMessageSigning {
    internal static class SignatureHeaderComposer {
        public static string Compose(string signatureString, RequestHttpMessageSigningConfiguration config, DateTimeOffset timestamp) {
            var builder = new StringBuilder();

            builder.Append("keyId=");
            builder.Append(config.KeyId);

            builder.Append(",algorithm=");
            builder.Append(config.SignatureAlgorithm.GetAlgorithmName());

            builder.Append(",created=");
            builder.Append(timestamp.ToUnixTimeSeconds().ToString());

            if (config.Expires.HasValue) {
                builder.Append(",expires=");
                builder.Append(timestamp.Add(config.Expires.Value).ToUnixTimeSeconds().ToString());
            }

            if (config.HeadersToInclude.Count > 0) {
                builder.Append(",headers=");
                builder.AppendJoin(" ", config.HeadersToInclude.Select(x => x.ToLowerInvariant()));
            }

            builder.Append(",signature=");
            builder.Append(signatureString);

            return builder.ToString();
        }

        private static StringBuilder AppendJoin<T>(this StringBuilder builder, string separator, IEnumerable<T> values) =>
            builder.Append(string.Join(separator, values));
    }
}