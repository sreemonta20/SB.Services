#region Program.cs

// .NET 10 / Swashbuckle 10.x NAMESPACE CHANGES — same as SBERP.Security:
//
// REMOVED: using Microsoft.OpenApi.Models  → namespace gone in OpenApi 2.x+
// REMOVED: using Microsoft.AspNetCore.Mvc.Versioning → package renamed to Asp.Versioning
// ADDED:   using Microsoft.OpenApi         → all OpenApi types now in root namespace
// ADDED:   using Asp.Versioning            → replacement for deprecated versioning package
// ADDED:   using Microsoft.AspNetCore.Mvc.ApiExplorer → for TryGetMethodInfo()

using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Newtonsoft.Json.Serialization;
using SBERP.HumanResources.Filter;
using SBERP.Shared.Extensions;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
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

var configuration = builder.Configuration;

// 2. Serilog — reads from "Serilog" section in appsettings.json
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

// 3.1 CORS — accept from Gateway and direct Angular during development
var allowedOrigins = configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>()
    ?? new[] { "https://localhost:4200", "http://localhost:4200" };

services.AddCors(options =>
{
    options.AddPolicy("HRCorsPolicy", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("X-Pagination", "Token-Expired");
    });
});

// 3.2 Controllers + JSON serialization — mirrors Security
services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            JsonIgnoreCondition.WhenWritingNull)
    .AddNewtonsoftJson(o =>
        o.SerializerSettings.ContractResolver =
            new DefaultContractResolver());

// ADDED in .NET 10: Required for Swashbuckle to discover controller metadata
services.AddEndpointsApiExplorer();

// 3.3 API Versioning
// CHANGED in .NET 10: Must chain .AddApiExplorer() — Asp.Versioning 10.x requires
// this for its IApiVersionDescriptionProvider to register with the API explorer.
// Without it Swashbuckle finds no versioned endpoints and returns HTTP 500.
services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";       // formats as "v1", "v2" etc.
    options.SubstituteApiVersionInUrl = true;           // replaces {version} route tokens
});

// 3.4 Swagger
// CHANGED in .NET 10: OpenApiInfo, OpenApiContact, OpenApiSecurityScheme,
// SecuritySchemeType, ParameterLocation are now in Microsoft.OpenApi namespace
// (not Microsoft.OpenApi.Models which no longer exists).
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SBERP Human Resources Service",
        Version = "v1",
        Description = "Human Resources microservice — employee and department management",
        Contact = new OpenApiContact
        {
            Name = "Sreemonta Bhowmik",
            Email = "sreemonta.bhowmik@yahoo.com",
            Url = new Uri("https://localhost:44358")
        }
    });

    // Operation filter — removes auto-injected api-version query param from Swagger UI
    c.OperationFilter<SwaggerDefaultValues>();

    // Maps each controller action to the correct swagger doc version.
    // Falls back to v1 when no [ApiVersion] attribute is present — HR controllers
    // without the attribute still appear in Swagger under v1.
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

    // Http Bearer scheme — Swagger UI adds "Bearer " prefix automatically.
    // Users paste ONLY the raw JWT token in the Authorize dialog.
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter your JWT token only — Swagger adds 'Bearer ' automatically.",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"   // lowercase — RFC 6750
    });

    // CHANGED in .NET 10: pass the lambda's document parameter directly into
    // OpenApiSecuritySchemeReference so it serialises as "Bearer":[] correctly.
    // Old OpenApiSecurityRequirement dictionary syntax no longer works.
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

// 3.5 JWT Auth via SBERP.Shared — reads "SharedJwtSettings" from appsettings.json.
// Also registers ITokenBlacklistService (Scoped, used by TokenBlacklistMiddleware).
services.AddSharedJwtAuthentication(builder.Configuration);

// 3.6 Redis for token blacklist.
// Microsoft.Extensions.Caching.StackExchangeRedis v10.0.0.
// AddSharedJwtAuthentication registers ITokenBlacklistService which depends on
// IDistributedCache — this Redis registration satisfies that dependency.
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        configuration["RedisSettings:ConnectionString"] ?? "localhost:6379";
    options.InstanceName =
        configuration["RedisSettings:InstanceName"] ?? "SBERP_HR_";
});

// 3.7 Core services
services.AddHttpContextAccessor();
services.AddResponseCompression();

// 4. Build application
var app = builder.Build();

try
{
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json",
                              "SBERP Human Resources v1");
            c.RoutePrefix = string.Empty; // Swagger at root https://localhost:44358
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
    app.UseCors("HRCorsPolicy");
    app.UseAuthentication();  // Layer 2: validates JWT using SharedJwtSettings key
    app.UseTokenBlacklist();  // Layer 3: checks Redis — blocks revoked tokens (SBERP.Shared)
    app.UseAuthorization();   // Layer 4: enforces [Authorize] and [Authorize(Roles="Admin")]
    app.UseResponseCompression();
    app.MapControllers();
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}

#endregion