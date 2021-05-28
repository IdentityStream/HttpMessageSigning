# HttpMessageSigning

Implementation of [Signing HTTP Messages](https://datatracker.ietf.org/doc/html/draft-cavage-http-signatures-12) for WCF and HttpClient.

## Usage

When hooking up HTTP message signing, there's a bunch of configuration options available:

| Option | Default | Description |
|--------|-------------|---------|
| `AddRecommendedHeaders` | `true` | Automatically adds recommended headers to `HeadersToInclude` based on the specification and configuration. |
| `DigestAlgorithm` | None | If set, enables digest calculation of the request body. If `AddRecommendedHeaders` has been turned off, you also have to add `Digest` to `HeadersToInclude` in order to enable the digest calculation. |
| `GetUtcNow` | `DateTimeOffset.UtcNow` | Gets the current UTC timestamp. Useful for testing. |
| `HeadersToInclude` | Empty | A set of headers to include in the signature. |
| `Expires` | None | If set, enables signature expiry after the specified amount of time. |

### WCF

To use HTTP message signing with WCF, call `UseHttpMessageSigning` on your client endpoint:

```csharp
var proxy = new /* your client proxy /*;

var signatureAlgorithm = SignatureAlgorithm.Create(/* your RSA or ECDsa algorithm /*);

proxy.Endpoint.UseHttpMessageSigning("key-id", signatureAlgorithm);
```

There's also a bunch of convenience overloads for working with `X509Certificate2`, which will automatically
get the signature algorithm based on the certificate cryptography.

### HttpClient

Coming soon...
