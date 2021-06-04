using System.Net.Http;
using System.Security.Cryptography;
using System.ServiceModel;
using IdentityStream.HttpMessageSigning.ServiceModel;

namespace IdentityStream.HttpMessageSigning.Tests {
    public class Snippets {
        // Fake endpoint for snippet purposes
        public class TheEndpointClient : ClientBase<string> {
            public TheEndpointClient(BasicHttpsBinding binding, EndpointAddress remoteAddress) {
            }
        }

        public void WCF_Client_Setup(BasicHttpsBinding binding, EndpointAddress endpointAddress, RSA rsaOrECDsaAlgorithm) {
            #region WCF_Endpoint_UseHttpMessageSigning
            using var client = new TheEndpointClient(binding, endpointAddress);

            var signatureAlgorithm = SignatureAlgorithm.Create(rsaOrECDsaAlgorithm);

            var config = HttpMessageSigningConfiguration.Create("key-id", signatureAlgorithm);

            client.Endpoint.UseHttpMessageSigning(config);

            // Make calls using client :)
            #endregion
        }

        public void HttpClient_Setup(RSA rsaOrECDsaAlgorithm) {
            #region HttpClient_SigningHttpMessageHandler
            var signatureAlgorithm = SignatureAlgorithm.Create(rsaOrECDsaAlgorithm);

            var config = HttpMessageSigningConfiguration.Create("key-id", signatureAlgorithm);

            var handler = new SigningHttpMessageHandler(config);

            using var client = new HttpClient(handler);

            // Make requests using client :)
            #endregion
        }
    }
}
