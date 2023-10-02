using System.Net.Http;
using System.Security.Cryptography;
using System.ServiceModel;

namespace IdentityStream.HttpMessageSigning.Tests {
    public class Snippets {
        // Fake endpoint for snippet purposes
        public class TheEndpointClient : ClientBase<string> {
#pragma warning disable IDE0060 // Remove unused parameter
            public TheEndpointClient(BasicHttpsBinding binding, EndpointAddress remoteAddress) {
            }
#pragma warning restore IDE0060 // Remove unused parameter
        }

        public static void WCF_Client_Setup(BasicHttpsBinding binding, EndpointAddress endpointAddress, RSA rsaOrECDsaAlgorithm) {
            #region WCF_Endpoint_UseHttpMessageSigning
            var signatureAlgorithm = SignatureAlgorithm.Create(rsaOrECDsaAlgorithm);

            var config = new HttpMessageSigningConfiguration("key-id", signatureAlgorithm);

            using var client = new TheEndpointClient(binding, endpointAddress);

            client.UseHttpMessageSigning(config);

            // Make calls using client :)
            #endregion
        }

        public static void HttpClient_Setup(RSA rsaOrECDsaAlgorithm) {
            #region HttpClient_SigningHttpMessageHandler
            var signatureAlgorithm = SignatureAlgorithm.Create(rsaOrECDsaAlgorithm);

            var config = new HttpMessageSigningConfiguration("key-id", signatureAlgorithm);

            var handler = new SigningHttpMessageHandler(config);

            using var client = new HttpClient(handler);

            // Make requests using client :)
            #endregion
        }
    }
}
