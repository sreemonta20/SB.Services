using System.Reflection;

namespace SBERP.Security.Persistence
{
    public static class SqlScriptHelper
    {
        public static string LoadEmbeddedSql(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                                       .FirstOrDefault(r => r.EndsWith(fileName));

            if (resourceName == null)
                throw new Exception($"SQL resource file '{fileName}' not found.");

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
