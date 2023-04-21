using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace SparebankenVest.HttpMessageSigning.ServiceModel {
    /// <summary>
    /// Behavior that adds a <see cref="HttpMessageSigningMessageInspector"/> to the client runtime.
    /// </summary>
    public class HttpMessageSigningEndpointBehavior : IEndpointBehavior {
        /// <summary>
        /// Creates a new behavior with the specified <paramref name="config"/>.
        /// </summary>
        /// <param name="config">The configuration to use when signing HTTP messages.</param>
        public HttpMessageSigningEndpointBehavior(HttpMessageSigningConfiguration config) {
            Config = config;
        }

        private HttpMessageSigningConfiguration Config { get; }

        /// <inheritdoc/>
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) {
            // No implementation necessary.
        }

        /// <inheritdoc/>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) {
            clientRuntime.ClientMessageInspectors.Add(new HttpMessageSigningMessageInspector(Config));
        }

        /// <inheritdoc/>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) {
            // No implementation necessary.
        }

        /// <inheritdoc/>
        public void Validate(ServiceEndpoint endpoint) {
            // No implementation necessary.
        }
    }
}
