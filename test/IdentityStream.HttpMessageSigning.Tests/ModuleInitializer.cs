using System.IO;
using System.Runtime.CompilerServices;
using VerifyTests;
using VerifyXunit;

namespace IdentityStream.HttpMessageSigning.Tests
{
    public static class ModuleInitializer {
        [ModuleInitializer]
        public static void Initialize() {
            var directory = Path.Combine(AttributeReader.GetProjectDirectory(), "Snapshots");
            Verifier.DerivePathInfo((_, _, type, method) => new(directory, type.Name, method.Name));
        }
    }
}