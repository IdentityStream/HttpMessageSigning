# SparebankenVest.HttpMessageSigning

A .NET implementation of "[Signing HTTP Messages](https://datatracker.ietf.org/doc/html/draft-cavage-http-signatures-12)" (Cavage, draft 12) for WCF and HttpClient.

This project is a fork of [IdentityStream.HttpMessageSigning](https://github.com/IdentityStream/HttpMessageSigning).

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
| `RequestTargetUriFormat` | `UriFormat.Unescaped` | Gets or sets the URI format used when constructing the `(request-target)` header. |

When using a certificate for signing, there's a convenience method called `HttpMessageSigningConfiguration.FromCertificate` that can be used to get a configuration with crypto settings based on the certificate.

### WCF

To use HTTP message signing with WCF, call `UseHttpMessageSigning` on your client:

<!-- snippet: WCF_Endpoint_UseHttpMessageSigning -->
<a id='snippet-wcf_endpoint_usehttpmessagesigning'></a>
```cs
var signatureAlgorithm = SignatureAlgorithm.Create(rsaOrECDsaAlgorithm);

var config = new HttpMessageSigningConfiguration("key-id", signatureAlgorithm);

using var client = new TheEndpointClient(binding, endpointAddress);

client.UseHttpMessageSigning(config);

// Make calls using client :)
```
<sup><a href='/test/SparebankenVest.HttpMessageSigning.Tests/Snippets.cs#L15-L25' title='Snippet source file'>snippet source</a> | <a href='#snippet-wcf_endpoint_usehttpmessagesigning' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### HttpClient

To use HTTP message signing with `HttpClient`, create an instance of `SigningHttpMessageHandler` and pass it when creating the `HttpClient` instance:

<!-- snippet: HttpClient_SigningHttpMessageHandler -->
<a id='snippet-httpclient_signinghttpmessagehandler'></a>
```cs
var signatureAlgorithm = SignatureAlgorithm.Create(rsaOrECDsaAlgorithm);

var config = new HttpMessageSigningConfiguration("key-id", signatureAlgorithm);

var handler = new SigningHttpMessageHandler(config);

using var client = new HttpClient(handler);

// Make requests using client :)
```
<sup><a href='/test/SparebankenVest.HttpMessageSigning.Tests/Snippets.cs#L29-L39' title='Snippet source file'>snippet source</a> | <a href='#snippet-httpclient_signinghttpmessagehandler' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->
