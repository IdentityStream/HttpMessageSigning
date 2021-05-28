using System;

namespace IdentityStream.HttpMessageSigning {
    /// <summary>
    /// Thown when a configuration is invalid.
    /// </summary>
    public class InvalidHttpMessageSigningConfiguration : Exception {
        /// <summary>
        /// Creates an instance of <see cref="InvalidHttpMessageSigningConfiguration"/>.
        /// </summary>
        public InvalidHttpMessageSigningConfiguration() {
        }

        /// <summary>
        /// Creates an instance of <see cref="InvalidHttpMessageSigningConfiguration"/>.
        /// </summary>
        public InvalidHttpMessageSigningConfiguration(string message) : base(message) {
        }

        /// <summary>
        /// Creates an instance of <see cref="InvalidHttpMessageSigningConfiguration"/>.
        /// </summary>
        public InvalidHttpMessageSigningConfiguration(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
