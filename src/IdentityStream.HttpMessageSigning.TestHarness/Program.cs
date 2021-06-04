using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using IdentityStream.HttpMessageSigning;
using IdentityStream.HttpMessageSigning.ServiceModel;

var binding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
var remoteAddress = new EndpointAddress("https://httpdump.app/dumps/419e3bbd-1300-46ed-9cc2-f581062ca8fd");

var client = new HelloEndpointClient(binding, remoteAddress);

var certificate = GetCertificate("CN=IdentityStreamServiceManager");

var config = HttpMessageSigningConfiguration.Create(certificate, HashAlgorithmName.SHA256, config => {
    config.DigestAlgorithm = HashAlgorithmName.SHA256;
    config.HeadersToInclude.Add(HeaderNames.RequestTarget);
    config.HeadersToInclude.Add(HeaderNames.Expires);
    config.HeadersToInclude.Add(HeaderNames.Created);
    config.HeadersToInclude.Add(HeaderNames.Digest);
    config.AddRecommendedHeaders = false;
    config.Expires = TimeSpan.FromMinutes(2);
});

client.Endpoint.UseHttpMessageSigning(config);

await client.OpenAsync();

await client.SayHelloAsync(new helloRequest { Name = "Kristian" });


static X509Certificate2 GetCertificate(string subject) {
    using var store = new X509Store(StoreLocation.LocalMachine);

    store.Open(OpenFlags.ReadOnly);

    foreach (var certificate in store.Certificates) {
        if (certificate.Subject.Equals(subject, StringComparison.OrdinalIgnoreCase)) {
            return certificate;
        }
    }

    throw new CertificateNotFoundException(subject);
}

class CertificateNotFoundException : Exception {
    public CertificateNotFoundException(string name)
        : base($"Could not find certificate with friendly name '{name}' in store.") {
    }

    public CertificateNotFoundException() : base() {
    }

    public CertificateNotFoundException(string message, Exception innerException)
        : base(message, innerException) {
    }
}