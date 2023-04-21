using System.IO;
using System.Runtime.CompilerServices;
using VerifyTests;

namespace SparebankenVest.HttpMessageSigning.Tests
{
    public static class ModuleInitializer {
        [ModuleInitializer]
        public static void Initialize() {
            var directory = Path.Combine(AttributeReader.GetProjectDirectory(), "Snapshots");
            VerifierSettings.DerivePathInfo((_, _, type, method) => new(directory, type.Name, method.Name));
        }
    }
}