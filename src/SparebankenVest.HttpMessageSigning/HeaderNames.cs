namespace SparebankenVest.HttpMessageSigning {
    /// <summary>
    /// A collection of header names used for signing HTTP messages.
    /// </summary>
    public static class HeaderNames {
        /// <summary>
        /// The <c>(request-target)</c> header.
        /// </summary>
        public const string RequestTarget = "(request-target)";

        /// <summary>
        /// The <c>(created)</c> header.
        /// </summary>
        public const string Created = "(created)";

        /// <summary>
        /// The <c>Expires</c> header.
        /// </summary>
        public const string Expires = "(expires)";

        /// <summary>
        /// The <c>Signature</c> header.
        /// </summary>
        public const string Signature = "Signature";

        /// <summary>
        /// The <c>Digest</c> header.
        /// </summary>
        public const string Digest = "Digest";

        /// <summary>
        /// The <c>Date</c> header.
        /// </summary>
        public const string Date = "Date";

        /// <summary>
        /// The <c>Host</c> header.
        /// </summary>
        public const string Host = "Host";
    }
}
