using System.Reflection;

namespace SBERP.HumanResources.Persistence
{
    /// <summary>
    /// Loads embedded SQL files at migration time. Mark Functions.sql,
    /// StoredProcedures.sql, and Triggers.sql with Build Action = Embedded
    /// Resource in the csproj for this to work.
    /// </summary>
    public static class SqlScriptHelper
    {
        public static string LoadEmbeddedSql(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(r => r.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

            if (resourceName == null)
                throw new FileNotFoundException(
                    $"Embedded SQL '{fileName}' not found. Confirm Build Action = Embedded Resource.");

            using var stream = assembly.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
