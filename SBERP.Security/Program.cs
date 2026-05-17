#region Program.cs

// .NET 10 / Swashbuckle 10.x NAMESPACE CHANGES vs .NET 9 / Swashbuckle 6.x:
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
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Newtonsoft.Json.Serialization;
using SBERP.DataAccessLayer;
using SBERP.EmailService.Service;
using SBERP.Security.Filter;
using SBERP.Security.Helper;
using SBERP.Security.Middlewares;
using SBERP.Security.Models.Configuration;
using SBERP.Security.Persistence;
using SBERP.Security.Service;
using SBERP.Shared.Extensions;
using SBERP.Shared.Services;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(ConstantSupplier.APP_SETTINGS_FILE_NAME);

var configuration = builder.Configuration;

// 2. Logging (Serilog)
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentUserName()
    .CreateLogger();

builder.Host.UseSerilog();

Log.Information(ConstantSupplier.LOG_INFO_APP_START_MSG);
Log.Logger.Information(new StringBuilder()
    .AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_FIRST)
    .AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_SECOND_GATEWAY)
    .AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_THIRD_VERSION)
    .AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_FOURTH_COPYRIGHT)
    .AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_END)
    .ToString());

// 3. Services
var services = builder.Services;

// 3.1 App Settings
services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

// 3.2 Register Database
RegisterDatabase(services, appSettings);

// 3.3 Core Services
services.AddScoped<IEmailService, EmailSender>();

services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)
    .AddNewtonsoftJson(o =>
        o.SerializerSettings.ContractResolver = new DefaultContractResolver());

// ADDED in .NET 10: Required for Swashbuckle to discover controller metadata
services.AddEndpointsApiExplorer();

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
    options.GroupNameFormat = "'v'VVV";         // formats version as "v1", "v2" etc.
    options.SubstituteApiVersionInUrl = true;   // replaces {version} route tokens
});

// 3.4 CORS
var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>()
    ?? new[] { "https://localhost:4200", "http://localhost:4200" };

services.AddCors(options =>
{
    options.AddPolicy(ConstantSupplier.CORSS_POLICY_NAME, builder =>
    {
        builder.WithOrigins(allowedOrigins)
               .AllowAnyMethod()
               .AllowAnyHeader()
               .WithExposedHeaders("X-Pagination")
               .WithExposedHeaders("Token-Expired");
    });
});

// 3.5 Swagger
// CHANGED in .NET 10: OpenApiInfo, OpenApiContact, OpenApiSecurityScheme,
// SecuritySchemeType, ParameterLocation are now in Microsoft.OpenApi namespace
// (not Microsoft.OpenApi.Models which no longer exists).
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_VERSION_NAME, new OpenApiInfo
    {
        Title = ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_TITLE,
        Version = ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_VERSION_NAME,
        Description = ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_DESCRIPTION,
        Contact = new OpenApiContact
        {
            Name = ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_CONTACT_NAME,
            Email = ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_CONTACT_EMAIL,
            Url = new Uri(ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_CONTACT_URL)
        }
    });

    c.OperationFilter<SwaggerDefaultValues>();

    // Maps each controller action to the correct swagger doc version
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
    c.AddSecurityDefinition(
        ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_SCHEME_REF_ID,
        new OpenApiSecurityScheme
        {
            Description = "Enter your JWT token only — Swagger adds 'Bearer ' automatically.",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer"   // lowercase — RFC 6750
        });

    // CHANGED in .NET 10: pass the lambda's document parameter directly into
    // OpenApiSecuritySchemeReference so it serialises as "Bearer":[] correctly.
    // No SecurityDocumentFilter needed.
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(
            ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_SCHEME_REF_ID, document)] = []
    });
});

// 3.6 JWT Authentication
var key = Encoding.ASCII.GetBytes(appSettings?.JWT?.Key ?? "");
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = appSettings?.JWT?.Issuer,
            ValidAudience = appSettings?.JWT?.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception is SecurityTokenExpiredException)
                    // CHANGED in .NET 10: .Add() deprecated → use .Append()
                    context.Response?.Headers?.Append("Token-Expired", "true");
                return Task.CompletedTask;
            }
        };
    });

services.AddAuthorization();
services.AddHttpContextAccessor();
services.AddResponseCompression();
//services.AddDistributedMemoryCache();
var redisSettings = configuration
    .GetSection("RedisSettings")
    .Get<RedisSettings>();

services.Configure<RedisSettings>(
    configuration.GetSection("RedisSettings"));

// AddStackExchangeRedisCache — Microsoft recommended pattern
// (learn.microsoft.com/aspnetcore/performance/caching/distributed)
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisSettings?.ConnectionString ?? "localhost:6379";
    options.InstanceName = redisSettings?.InstanceName ?? "SBERP_Security_";
});

// 3.7 Dependency Injection Services
services.AddTransient<ISecurityLogService, SecurityLogService>();
services.AddTransient<IAuthService, AuthService>();
services.AddTransient<IDataAnalyticsService, DataAnalyticsService>();
services.AddTransient<IUserService, UserService>();
services.AddTransient<IRoleMenuService, RoleMenuService>();
services.AddScoped<ITokenService, TokenService>();
services.AddScoped<IDatabaseManager, DatabaseManager>();
services.AddScoped<IDatabaseHandlerFactory, DatabaseHandlerFactory>();
services.AddScoped<ValidateModelAttribute>();
services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();

// 4. Build App
var app = builder.Build();

try
{
    app.UseMiddleware<GlobalExceptionMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint(
                ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_END_POINT,
                ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_END_POINT_NAME);
            c.RoutePrefix = string.Empty;
        });
    }
    else
    {
        app.UseHsts();
    }

    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseCors(ConstantSupplier.CORSS_POLICY_NAME);
    app.UseMiddleware<SecurityHeadersMiddleware>();
    app.UseMiddleware<HttpVerbsConstraintMiddleware>();
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

static void RegisterDatabase(IServiceCollection services, AppSettings? appSettings)
{
    var cs = appSettings?.ConnectionStrings;
    switch (appSettings?.AppDB)
    {
        case ConstantSupplier.SQLSERVER:
            services.AddDbContext<SecurityDBContext>(options =>
                options.UseSqlServer(cs?.ProdSqlConnectionString));
            services.AddTransient<IDbConnection>(_ =>
                new SqlConnection(cs?.ProdSqlConnectionString));
            break;
        case ConstantSupplier.ORACLE:
            services.AddDbContext<SecurityDBContext>(options =>
                options.UseOracle(cs?.ProdOracleConnectionString));
            services.AddScoped<IDbConnection>(_ =>
                new Oracle.ManagedDataAccess.Client.OracleConnection(cs?.ProdOracleConnectionString));
            break;
        case ConstantSupplier.ODBC:
            services.AddDbContext<SecurityDBContext>(options =>
                options.UseJetOdbc(cs?.ProdOdbcConnectionString));
            services.AddScoped<IDbConnection>(_ =>
                new OdbcConnection(cs?.ProdOdbcConnectionString));
            break;
        case ConstantSupplier.OLEDB:
            services.AddDbContext<SecurityDBContext>(options =>
                options.UseJetOleDb(cs?.ProdOledbConnectionString));
            services.AddScoped<IDbConnection>(_ =>
                new OleDbConnection(cs?.ProdOledbConnectionString));
            break;
    }
}

#endregion