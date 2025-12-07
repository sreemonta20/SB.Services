#region Program.cs Final Version

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using SBERP.DataAccessLayer;
using SBERP.EmailService.Service;
using SBERP.Security.Filter;
using SBERP.Security.Helper;
using SBERP.Security.Middlewares;
using SBERP.Security.Models.Configuration;
using SBERP.Security.Persistence;
using SBERP.Security.Service;
using Serilog;
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
    .AddJsonOptions(options => options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)
    .AddNewtonsoftJson(o => o.SerializerSettings.ContractResolver = new DefaultContractResolver());

services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

// 3.4 CORS
var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "https://localhost:4200", "http://localhost:4200" };
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

    c.ResolveConflictingActions(apiDescription => apiDescription.First());
    c.OperationFilter<SwaggerDefaultValues>();

    var securitySchema = new OpenApiSecurityScheme
    {
        Description = ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_SCHEME_DESC,
        Name = ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_SCHEME_NAME,
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme
        }
    };

    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securitySchema);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securitySchema, new[] { JwtBearerDefaults.AuthenticationScheme } }
    });
});

// 3.6 JWT Authentication
var key = Encoding.ASCII.GetBytes(appSettings?.JWT?.Key ?? "");
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Change to true in production
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
                    context.Response?.Headers?.Add("Token-Expired", "true");
                return Task.CompletedTask;
            }
        };
    });

services.AddAuthorization();
services.AddHttpContextAccessor();
services.AddResponseCompression();
services.AddDistributedMemoryCache();

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

// 4. Build App
var app = builder.Build();

try
{
    // 5. Middleware
    // Global exception handler — should be first to catch all exceptions
    app.UseMiddleware<GlobalExceptionMiddleware>();
    // Environment-specific middlewares (development tools)
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint(ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_END_POINT, ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_END_POINT_NAME);
            c.RoutePrefix = string.Empty;
        });
    }
    else
    {
        // Use HSTS only in non-development environments
        app.UseHsts();
    }

    // Optional: Log all HTTP requests
    app.UseSerilogRequestLogging();
    // Enforce HTTPS
    app.UseHttpsRedirection();
    // Serve static files (e.g., wwwroot)
    app.UseStaticFiles();
    // Routing starts here
    app.UseRouting();
    // CORS must be applied before any auth middleware
    app.UseCors(ConstantSupplier.CORSS_POLICY_NAME);
    // Security headers — safe to add here
    app.UseMiddleware<SecurityHeadersMiddleware>();
    // Custom middleware related to routing (e.g., HTTP verb checks)
    app.UseMiddleware<HttpVerbsConstraintMiddleware>();
    // Authentication and Authorization
    app.UseAuthentication();
    app.UseAuthorization();
    // Compression (applied before sending response)
    app.UseResponseCompression();
    // Final step: Map controller endpoints
    app.MapControllers();
    // Run the application
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}

// Database Registration
static void RegisterDatabase(IServiceCollection services, AppSettings? appSettings)
{
    var cs = appSettings?.ConnectionStrings;

    switch (appSettings?.AppDB)
    {
        case ConstantSupplier.SQLSERVER:
            services.AddDbContext<SecurityDBContext>(options =>
                options.UseSqlServer(cs?.ProdSqlConnectionString));
            services.AddTransient<IDbConnection>(_ => new SqlConnection(cs?.ProdSqlConnectionString));
            break;

        case ConstantSupplier.ORACLE:
            // Recommendation: use Oracle.ManagedDataAccess.Client instead of deprecated System.Data.OracleClient
            services.AddDbContext<SecurityDBContext>(options =>
                options.UseOracle(cs?.ProdOracleConnectionString));
            services.AddScoped<IDbConnection>(_ => new Oracle.ManagedDataAccess.Client.OracleConnection(cs?.ProdOracleConnectionString));
            break;

        case ConstantSupplier.ODBC:
            services.AddDbContext<SecurityDBContext>(options =>
                options.UseJetOdbc(cs?.ProdOdbcConnectionString));
            services.AddScoped<IDbConnection>(_ => new OdbcConnection(cs?.ProdOdbcConnectionString));
            break;

        case ConstantSupplier.OLEDB:
            services.AddDbContext<SecurityDBContext>(options =>
                options.UseJetOleDb(cs?.ProdOledbConnectionString));
            services.AddScoped<IDbConnection>(_ => new OleDbConnection(cs?.ProdOledbConnectionString));
            break;
    }
}

#endregion