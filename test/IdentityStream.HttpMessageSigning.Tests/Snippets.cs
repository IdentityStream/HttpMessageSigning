using System.Security.Cryptography;
using System.ServiceModel;
using IdentityStream.HttpMessageSigning.ServiceModel;

namespace IdentityStream.HttpMessageSigning.Tests {
    public class Snippets {
        //Fake endpoint for snippet purposes
        public class TheEndpointClient : ClientBase<string> {
            public TheEndpointClient(BasicHttpsBinding binding, EndpointAddress remoteAddress) {
            }
        }

        public void MissingHeader_Throws(BasicHttpsBinding binding,EndpointAddress endpointAddress, RSA rsaOrECDsaAlgorithm) {
            #region UseHttpMessageSigning
            var client = new TheEndpointClient(binding, endpointAddress);

            var signatureAlgorithm = SignatureAlgorithm.Create(rsaOrECDsaAlgorithm);

            client.Endpoint.UseHttpMessageSigning("key-id", signatureAlgorithm);
            #endregion
        }
    }
}
