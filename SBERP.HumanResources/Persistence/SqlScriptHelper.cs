using Microsoft.EntityFrameworkCore.Migrations;
using System.Reflection;

namespace SBERP.HumanResources.Persistence
{
    /// <summary>
    /// Applies Persistence/Sql/HumanResourcesDB.sql, the single file holding every SQL object of the service
    /// (Embedded Resource via the csproj wildcard), at migration time.
    /// <para>
    /// The file is read from the assembly being migrated, i.e. always its newest version. So call
    /// <see cref="ApplyAll"/> only from the NEWEST migration: when adding a migration, move the call
    /// from the previous one into it. Databases that already ran the previous migration are not
    /// affected (EF never re-runs it), and a fresh database runs the SQL once, after all tables exist.
    /// </para>
    /// </summary>
    public static class SqlScriptHelper
    {
        private const string SqlFolderMarker = ".Persistence.Sql.";

        public static string LoadEmbeddedSql(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(r => r.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

            if (resourceName == null)
                throw new FileNotFoundException(
                    $"Embedded SQL '{fileName}' not found. Confirm Build Action = Embedded Resource.");

            return Read(assembly, resourceName);
        }

        /// <summary>
        /// Queue the Persistence/Sql script(s) onto the migration, in file-name order. EF splits the
        /// script on its GO batch separators. Every object is dropped and created again, so the
        /// script can be applied any number of times.
        /// </summary>
        public static void ApplyAll(MigrationBuilder migrationBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var scripts = assembly.GetManifestResourceNames()
                .Where(r => r.Contains(SqlFolderMarker, StringComparison.OrdinalIgnoreCase)
                         && r.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
                .OrderBy(r => r, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (scripts.Count == 0)
                throw new FileNotFoundException("No embedded scripts found under Persistence/Sql.");

            foreach (var name in scripts)
                migrationBuilder.Sql(Read(assembly, name));
        }

        private static string Read(Assembly assembly, string resourceName)
        {
            using var stream = assembly.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
