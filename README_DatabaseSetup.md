# Database Object Setup (Before Running EF Core Code-First Commands)

This document explains how to integrate SQL **functions**, **stored procedures**, and **triggers** into the EF Core **code-first** migration pipeline so that all database objects are automatically created when running:

```
Add-Migration InitialSeedData
Update-Database
```

---

## 1. Final Architecture (Persistence Folder)

Ensure your backend contains a `Persistence` folder with the following files:

```
Persistence/
    Functions.sql
    StoredProcedures.sql
    Triggers.sql
    (optional) Tables.sql
    (optional) DB.sql
```

EF Core will execute:

- `Functions.sql`
- `StoredProcedures.sql`
- `Triggers.sql`

during migration.

---

## 2. Mark SQL Files as Embedded Resources

For each file:

- `Functions.sql`
- `StoredProcedures.sql`
- `Triggers.sql`

do the following:

1. Right-click the file → **Properties**
2. Set:  
   ```
   Build Action = Embedded Resource
   Copy to Output Directory = Do not copy
   ```

This makes the SQL files accessible inside EF migrations.

---

## 3. Add SQL Script Loader Helper

Create a helper class in the `Persistence` folder:

```csharp
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
```

EF migrations will use this helper to load SQL objects.

---

## 4. Create Migration and Execute SQL Files

1. Generate migration:

```
Add-Migration InitialSeedData
```

2. Open the generated migration file and modify it:

```csharp
using Microsoft.EntityFrameworkCore.Migrations;
using SBERP.Security.Persistence;

public partial class InitialSeedData : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // EF auto-generated table creation stays here

        migrationBuilder.Sql(SqlScriptHelper.LoadEmbeddedSql("Functions.sql"));
        migrationBuilder.Sql(SqlScriptHelper.LoadEmbeddedSql("StoredProcedures.sql"));
        migrationBuilder.Sql(SqlScriptHelper.LoadEmbeddedSql("Triggers.sql"));

        // Optional: Add seed data here
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Optional cleanup (drop functions, procedures, triggers)
    }
}
```

3. Apply migration:

```
Update-Database
```

---

## 5. Expected Result After Update-Database

After running:

```
Add-Migration InitialSeedData
Update-Database
```

EF Core will:

✔ Create all database **tables**  
✔ Insert **seed data** (if any)  
✔ Execute:
- `Functions.sql` → Creates all SQL functions  
- `StoredProcedures.sql` → Creates all stored procedures  
- `Triggers.sql` → Creates all triggers  

Your database will now include **all schema + all SQL objects** in a single automated migration.

---
