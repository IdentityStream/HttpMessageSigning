using System;
using System.Security.Cryptography;

namespace IdentityStream.HttpMessageSigning
{
	internal class HMACSignatureAlgorithm : ISignatureAlgorithm {
		public HMACSignatureAlgorithm(byte[] key, HashAlgorithmName hashAlgorithm) {
			HashAlgorithm = hashAlgorithm;
			_key = key;
		}
		public string Name => "HMAC";

		private byte[] _key;

		public HashAlgorithmName HashAlgorithm { get;}

		public byte[] ComputeHash(byte[] bytes) {
			using var hmac = HMAC.Create(Name + HashAlgorithm.ToString());
			if (hmac is null) {
				throw new InvalidOperationException($"Invalid hash algorithm: {HashAlgorithm.Name}");
			}
			hmac.Key = _key;
			return hmac.ComputeHash(bytes);
		}
	}
}
