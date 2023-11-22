using System;
using System.Security.Cryptography;

namespace IdentityStream.HttpMessageSigning {
	// ReSharper disable once InconsistentNaming
	internal class HMACSignatureAlgorithm : ISignatureAlgorithm {
		public HMACSignatureAlgorithm(byte[] key, HashAlgorithmName hashAlgorithm) {
			Key = key ?? throw new ArgumentNullException(nameof(key));
			HashAlgorithm = hashAlgorithm;
		}

		public byte[] Key { get; }

		public HashAlgorithmName HashAlgorithm { get;}

		public string Name => "HMAC";

		public byte[] ComputeHash(byte[] bytes) {
			using var hmac = Hasher.GetHmac(HashAlgorithm, Key);
			return hmac.ComputeHash(bytes);
		}
	}
}
