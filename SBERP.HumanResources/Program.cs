#region Program.cs

// .NET 10 / Swashbuckle 10.x — same pattern as SBERP.Security.

using Asp.Versioning;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Newtonsoft.Json.Serialization;
using SBERP.HumanResources.Filter;
using SBERP.HumanResources.Helper;
using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Persistence;
using SBERP.HumanResources.Service;
using SBERP.Shared.Extensions;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// 1. Configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile(
        $"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

bool isConnectionStrInAzureVault = bool.Parse(builder.Configuration["IsConnectionStrInAzureVault"] ?? "false");
if (isConnectionStrInAzureVault)
{
    // --- Azure Key Vault ---
    var keyVaultUri = builder.Configuration["KeyVaultUri"];
    if (!string.IsNullOrWhiteSpace(keyVaultUri))
    {
        // DefaultAzureCredential works across environments without code changes:
        //   - local dev: Visual Studio / az login / VS Code sign-in
        //   - Azure host: the app's Managed Identity
        // Grant that identity "Key Vault Secrets User" on the vault (RBAC) or a
        // Get/List secrets access policy.
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUri),
            new DefaultAzureCredential());
    }
}

var configuration = builder.Configuration;

// 2. Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentUserName()
    .CreateLogger();

builder.Host.UseSerilog();
Log.Information("SBERP.HumanResources starting...");

// 3. Services
var services = builder.Services;

// 3.1 Bind AppSettings — used by HRSettingsService for upload paths
services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

// 3.2 DbContext — points at HumanResourcesDB (SQL Server for now, but switchable via AppSettings.AppDB and DI) [Register Database]
//var hrConnection = configuration.GetSection("ConnectionStrings")["HRDatabase"];
//if (string.IsNullOrWhiteSpace(hrConnection))
//    throw new InvalidOperationException(
//        "ConnectionStrings:HRDatabase missing in appsettings.json");

//services.AddDbContext<HumanResourcesDBContext>(options =>
//                options.UseSqlServer(hrConnection));
// 3.2 Register Database
Utilities.RegisterDatabase(services, appSettings);


// 3.3 CORS
//var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>()
//    ?? new[] { "https://localhost:4200", "http://localhost:4200" };
var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:5201", "https://localhost:5200", "http://localhost:5201", "https://localhost:4200", "http://localhost:4200", };

services.AddCors(o =>
{
    o.AddPolicy(ConstantSupplier.CORSS_POLICY_NAME, p =>
        p.WithOrigins(allowedOrigins)
         .AllowAnyMethod()
         .AllowAnyHeader()
         .WithExposedHeaders("X-Pagination", "Token-Expired"));
});

// 3.4 Controllers + JSON
//services.AddControllers()
//    .AddJsonOptions(o =>
//        o.JsonSerializerOptions.DefaultIgnoreCondition =
//            JsonIgnoreCondition.WhenWritingNull)
//    .AddNewtonsoftJson(o =>
//        o.SerializerSettings.ContractResolver =
//            new DefaultContractResolver());

services.AddControllers()
    .AddJsonOptions(o =>{
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    })
    .AddNewtonsoftJson(o =>
        o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

services.AddEndpointsApiExplorer();

// 3.5 API Versioning
services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// 3.6 Swagger
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SBERP Human Resources Service",
        Version = "v1",
        Description = "Human Resources microservice — employee, department, " +
                      "designation, attendance, and HR settings management",
        Contact = new OpenApiContact
        {
            Name = "Sreemonta Bhowmik",
            Email = "sreemonta.bhowmik@yahoo.com",
            Url = new Uri("https://localhost:44358")
        }
    });

    c.OperationFilter<SwaggerDefaultValues>();

    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (!apiDesc.TryGetMethodInfo(out var methodInfo)) return false;
        var versions = methodInfo.DeclaringType?
            .GetCustomAttributes(true)
            .OfType<ApiVersionAttribute>()
            .SelectMany(attr => attr.Versions)
            .Select(v => $"v{v.MajorVersion}");
        if (versions == null || !versions.Any()) return docName == "v1";
        return versions.Any(v => v == docName);
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter your JWT token only — Swagger adds 'Bearer ' automatically.",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

// 3.7 JWT via SBERP.Shared — same Key/Issuer/Audience as Security
services.AddSharedJwtAuthentication(builder.Configuration);

// 3.8 Redis (token blacklist + future caching)
services.AddStackExchangeRedisCache(o =>
{
    o.Configuration =
        configuration["RedisSettings:ConnectionString"] ?? "localhost:6379";
    o.InstanceName =
        configuration["RedisSettings:InstanceName"] ?? "SBERP_HR_";
});

// 3.9 Core services
services.AddHttpContextAccessor();
services.AddResponseCompression();

// 3.10 DI registrations
services.AddSingleton<IHRLogService, HRLogService>();
services.AddScoped<IDepartmentService, DepartmentService>();
services.AddScoped<IDesignationService, DesignationService>();
services.AddScoped<IEmployeeService, EmployeeService>();
services.AddScoped<IHRSettingsService, HRSettingsService>();
services.AddScoped<ValidateModelAttribute>();

// 4. Build
var app = builder.Build();

try
{
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint(
                ConstantSupplier.SWAGGER_HR_DOC_END_POINT,
                ConstantSupplier.SWAGGER_HR_DOC_END_POINT_NAME);
            c.RoutePrefix = string.Empty;
        });
    }
    else
    {
        app.UseHsts();
    }

    // ── Middleware pipeline — order is critical ─────────────────────────────
    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseCors(ConstantSupplier.CORSS_POLICY_NAME);
    app.UseAuthentication();
    app.UseTokenBlacklist();
    app.UseAuthorization();
    app.UseResponseCompression();
    app.MapControllers();
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}

//static void RegisterDatabase(IServiceCollection services, AppSettings? appSettings)
//{
//    var cs = appSettings?.ConnectionStrings;
//    switch (appSettings?.AppDB)
//    {
//        case ConstantSupplier.SQLSERVER:
//            services.AddDbContext<HumanResourcesDBContext>(options =>
//                options.UseSqlServer(cs?.HRSqlConnectionString));
//            services.AddTransient<IDbConnection>(_ =>
//                new SqlConnection(cs?.HRSqlConnectionString));
//            break;

//        default:
//            services.AddDbContext<HumanResourcesDBContext>(options =>
//                options.UseSqlServer(cs?.HRDefaultConnectionString));
//            services.AddTransient<IDbConnection>(_ =>
//                new SqlConnection(cs?.HRDefaultConnectionString));
//            break;
//    }
//}

#endregion
