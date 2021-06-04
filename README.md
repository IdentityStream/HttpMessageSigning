# HttpMessageSigning

A .NET implementation of "[Signing HTTP Messages](https://datatracker.ietf.org/doc/html/draft-cavage-http-signatures-12)" (Cavage, draft 12) for WCF and HttpClient.

## Usage

When hooking up HTTP message signing, there's a bunch of configuration options available:

| Option | Default | Description |
|--------|-------------|---------|
| `AddRecommendedHeaders` | `true` | Automatically adds recommended headers, such as `(request-target)`, `(created)`, `(expires)`, `Date` and `Digest` to `HeadersToInclude` based on the specification and configuration. |
| `DigestAlgorithm` | None | If set, enables digest calculation of the request body. If `AddRecommendedHeaders` has been turned off, you also have to add `Digest` to `HeadersToInclude` in order to enable the digest calculation. |
| `GetCurrentTimestamp` | `DateTimeOffset.UtcNow` | Gets the current UTC timestamp. Useful for testing. |
| `HeadersToInclude` | Empty | A set of headers to include in the signature. |
| `Expires` | None | If set, enables signature expiry after the specified amount of time. |
| `AddHeaderValue` | N/A | Adds a header with a value to all signed requests and their signatures. |
| `AddHeaderValues` | N/A | Adds a collection of headers to all signed requests and their signatures. |

### WCF

To use HTTP message signing with WCF, call `UseHttpMessageSigning` on your client endpoint:

<!-- snippet: UseHttpMessageSigning -->
<a id='snippet-usehttpmessagesigning'></a>
```cs
var client = new TheEndpointClient(binding, endpointAddress);

var signatureAlgorithm = SignatureAlgorithm.Create(rsaOrECDsaAlgorithm);

client.Endpoint.UseHttpMessageSigning("key-id", signatureAlgorithm);
```
<sup><a href='/test/IdentityStream.HttpMessageSigning.Tests/Snippets.cs#L14-L20' title='Snippet source file'>snippet source</a> | <a href='#snippet-usehttpmessagesigning' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

There's also a bunch of convenience overloads for working with `X509Certificate2`, which will automatically
get the signature algorithm based on the certificate cryptography.

### HttpClient

Coming soon...
