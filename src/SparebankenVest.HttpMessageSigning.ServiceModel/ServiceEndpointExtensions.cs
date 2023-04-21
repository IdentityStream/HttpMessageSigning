using System.ServiceModel.Description;
using SparebankenVest.HttpMessageSigning;
using SparebankenVest.HttpMessageSigning.ServiceModel;

// ReSharper disable once CheckNamespace
namespace System.ServiceModel {
    /// <summary>
    /// Extensions for hooking up HTTP message signing to a WCF service endpoint.
    /// </summary>
    public static class ServiceEndpointExtensions {
        /// <summary>
        /// Adds HTTP message signing behavior using the specified <paramref name="config"/>.
        /// </summary>
        /// <typeparam name="TChannel">The channel type of the client proxy.</typeparam>
        /// <param name="client">The client proxy.</param>
        /// <param name="config">The configuration to use for signing HTTP messages.</param>
        public static void UseHttpMessageSigning<TChannel>(this ClientBase<TChannel> client, HttpMessageSigningConfiguration config)
            where TChannel : class =>
                client.Endpoint.UseHttpMessageSigning(config);

        /// <summary>
        /// Adds HTTP message signing behavior using the specified <paramref name="config"/>.
        /// </summary>
        /// <param name="endpoint">The endpoint to add the behavior to.</param>
        /// <param name="config">The configuration to use for signing HTTP messages.</param>
        public static void UseHttpMessageSigning(this ServiceEndpoint endpoint, HttpMessageSigningConfiguration config) {
            if (endpoint is null) {
                throw new ArgumentNullException(nameof(endpoint));
            }
            if (!endpoint.EndpointBehaviors.Contains(typeof(HttpMessageSigningEndpointBehavior))) {
                endpoint.EndpointBehaviors.Add(new HttpMessageSigningEndpointBehavior(config));
            }
        }
    }
}
