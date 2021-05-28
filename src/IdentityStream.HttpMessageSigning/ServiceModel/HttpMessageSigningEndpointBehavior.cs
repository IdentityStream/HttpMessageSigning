using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace IdentityStream.HttpMessageSigning.ServiceModel {
    public class HttpMessageSigningEndpointBehavior : IEndpointBehavior {
        public HttpMessageSigningEndpointBehavior(HttpMessageSigningConfiguration config) {
            Config = config;
        }

        private HttpMessageSigningConfiguration Config { get; }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) {
            // No implementation necessary.
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) {
            clientRuntime.ClientMessageInspectors.Add(new HttpMessageSigningMessageInspector(Config));
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) {
            // No implementation necessary.
        }

        public void Validate(ServiceEndpoint endpoint) {
            // No implementation necessary.
        }
    }
}
