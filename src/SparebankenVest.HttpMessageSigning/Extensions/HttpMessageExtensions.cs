namespace SparebankenVest.HttpMessageSigning {
    internal static class HttpMessageExtensions {
        public static bool HasHeader(this IHttpMessage message, string name) => message.TryGetHeaderValues(name, out _);
    }
}
