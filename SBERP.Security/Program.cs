//#region Final Code
////////////////////////////////////////////////Key Changes///////////////////////////////////////////////////////////////////////
//////1. Middleware Order: Ensures app.UseRouting(), app.UseCors(), app.UseAuthentication(), and app.UseAuthorization() follow Microsoft's 
//////recommended pipeline order.
//////2. Structured Comments: Clear sections for configuration, service registration, and middleware.
//////3. Static File Handling: Ensures static file middleware(app.UseStaticFiles()) is included early but after HTTPS redirection.
//////4. Response Compression: Placed after essential authentication and authorization middlewares.

//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Data.SqlClient;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using Newtonsoft.Json.Serialization;
//using SBERP.DataAccessLayer;
//using SBERP.EmailService.Service;
//using SBERP.Security.Filter;
//using SBERP.Security.Helper;
//using SBERP.Security.Middlewares;
//using SBERP.Security.Models.Configuration;
//using SBERP.Security.Persistence;
//using SBERP.Security.Service;
//using Serilog;
//using System.Data;
//using System.Data.Odbc;
//using System.Data.OleDb;
//using System.Data.OracleClient;
//using System.Text;
//using System.Text.Json.Serialization;

//var builder = WebApplication.CreateBuilder(args);

//// 1. Load Configuration
//builder.Configuration
//    .SetBasePath(Directory.GetCurrentDirectory())
//    .AddJsonFile(ConstantSupplier.APP_SETTINGS_FILE_NAME);

//var configuration = builder.Configuration;

//// 2. Configure Logging (Serilog)
//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(configuration)
//    .Enrich.FromLogContext()
//    .Enrich.WithMachineName()
//    .Enrich.WithEnvironmentUserName()
//    .CreateLogger();

//builder.Host.UseSerilog();

//Log.Information(ConstantSupplier.LOG_INFO_APP_START_MSG);
//StringBuilder sb = new StringBuilder();
//sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_FIRST);
//sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_SECOND_GATEWAY);
//sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_THIRD_VERSION);
//sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_FOURTH_COPYRIGHT);
//sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_END);
//Log.Logger.Information(sb.ToString());

//// 3. Configure Services
//var services = builder.Services;

//// 3.1 Load App Settings
//services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
//var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

//// 3.2 Register Database
//RegisterDatabase(services, appSettings);

//// 3.3 Add Email Service
//services.AddScoped<IEmailService, EmailSender>();

//// 3.4 Configure Controllers and JSON Options
//services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
//    })
//    .AddNewtonsoftJson(o =>
//    {
//        o.SerializerSettings.ContractResolver = new DefaultContractResolver();
//    });

//// 3.5 API Versioning
//services.AddApiVersioning(options =>
//{
//    options.AssumeDefaultVersionWhenUnspecified = true;
//    options.DefaultApiVersion = new ApiVersion(1, 0);
//    options.ReportApiVersions = true;
//});

//// 3.6 Add CORS with specific origins from config
////services.AddCors(options =>
////{
////    options.AddPolicy(ConstantSupplier.CORSS_POLICY_NAME, builder =>
////    {
////        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
////    });
////});
//var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "https://localhost:4200", "http://localhost:4200" };

//services.AddCors(options =>
//{
//    options.AddPolicy(ConstantSupplier.CORSS_POLICY_NAME, builder =>
//    {
//        builder
//            .WithOrigins(allowedOrigins)
//            .AllowAnyMethod()
//            .AllowAnyHeader()
//            .WithExposedHeaders("X-Pagination");
//    });
//});

//// 3.7 Add Swagger
//services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc(ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_VERSION_NAME, new OpenApiInfo
//    {
//        Title = ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_TITLE,
//        Version = ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_VERSION_NAME,
//        Description = ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_DESCRIPTION,
//        Contact = new OpenApiContact
//        {
//            Name = ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_CONTACT_NAME,
//            Email = ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_CONTACT_EMAIL,
//            Url = new Uri(ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_CONTACT_URL)
//        }
//    });
//    c.ResolveConflictingActions(apiDescription => apiDescription.First());
//    c.OperationFilter<SwaggerDefaultValues>();

//    var securitySchema = new OpenApiSecurityScheme
//    {
//        Description = ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_SCHEME_DESC,
//        Name = ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_SCHEME_NAME,
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.Http,
//        Scheme = ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_SCHEME_SCHEME,
//        Reference = new OpenApiReference
//        {
//            Type = ReferenceType.SecurityScheme,
//            Id = ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_SCHEME_REF_ID
//        }
//    };

//    c.AddSecurityDefinition(ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_SCHEME_REF_ID, securitySchema);
//    var securityRequirement = new OpenApiSecurityRequirement();
//    securityRequirement.Add(securitySchema, new[] { ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_SCHEME_REF_ID });
//    c.AddSecurityRequirement(securityRequirement);
//});

//// 3.8 Configure Authentication and Authorization
//var key = Encoding.ASCII.GetBytes(appSettings?.JWT?.Key ?? "");
//services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.RequireHttpsMetadata = false; // Set false if testing locally // Always require HTTPS in production and set true
//    options.SaveToken = true;
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = appSettings?.JWT?.Issuer,
//        ValidAudience = appSettings?.JWT?.Audience,
//        IssuerSigningKey = new SymmetricSecurityKey(key),
//        ClockSkew = TimeSpan.Zero // Prevents issues where JWT tokens are still accepted after they expire.
//    };

//    // Handle JWT validation errors
//    options.Events = new JwtBearerEvents
//    {
//        OnAuthenticationFailed = context =>
//        {
//            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
//            {
//                context.Response?.Headers?.Add("Token-Expired", "true");
//            }
//            return Task.CompletedTask;
//        }
//    };
//});

//services.AddAuthorization();

//// 3.9 Add Additional Services
//services.AddHttpContextAccessor();
//services.AddResponseCompression();
//services.AddDistributedMemoryCache();

//// 3.10 Register DI Services
//services.AddTransient<ISecurityLogService, SecurityLogService>();
//services.AddTransient<IAuthService, AuthService>();
//services.AddTransient<IDataAnalyticsService, DataAnalyticsService>();
//services.AddTransient<IUserService, UserService>();
//services.AddTransient<IRoleMenuService, RoleMenuService>();
//services.AddScoped<ITokenService, TokenService>();
//services.AddScoped<IDatabaseManager, DatabaseManager>();
//services.AddScoped<IDatabaseHandlerFactory, DatabaseHandlerFactory>();
//services.AddScoped<ValidateModelAttribute>();

//// 4. Build the Application
//var app = builder.Build();

//app.UseMiddleware<GlobalExceptionMiddleware>();
//app.UseMiddleware<SecurityHeadersMiddleware>();
//app.UseMiddleware<HttpVerbsConstraintMiddleware>();

//app.UseHsts();
//// 5. Configure Middleware
//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint(ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_END_POINT, ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_END_POINT_NAME);
//        c.RoutePrefix = string.Empty;
//    });
//}

//app.UseSerilogRequestLogging();
//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();
//app.UseCors(ConstantSupplier.CORSS_POLICY_NAME);
//app.UseAuthentication();
//app.UseAuthorization();
//app.UseResponseCompression();

//app.MapControllers();
//app.Run();
//Log.CloseAndFlush(); //Ensures that Serilog flushes all logs before the app shuts down.

//// Static Method for Database Registration
//static void RegisterDatabase(IServiceCollection services, AppSettings? appSettings)
//{
//    switch (appSettings?.AppDB)
//    {
//        case ConstantSupplier.SQLSERVER:
//            services.AddDbContext<SecurityDBContext>(options =>
//                options.UseSqlServer(appSettings?.ConnectionStrings?.ProdSqlConnectionString));
//            services.AddTransient<IDbConnection>(_ => new SqlConnection(appSettings?.ConnectionStrings?.ProdSqlConnectionString));
//            break;
//        case ConstantSupplier.ORACLE:
//            services.AddDbContext<SecurityDBContext>(options =>
//                options.UseOracle(appSettings?.ConnectionStrings?.ProdOracleConnectionString));
//            services.AddScoped<IDbConnection>(_ => new OracleConnection(appSettings?.ConnectionStrings?.ProdOracleConnectionString));
//            break;
//        case ConstantSupplier.ODBC:
//            services.AddDbContext<SecurityDBContext>(options =>
//                options.UseJetOdbc(appSettings?.ConnectionStrings?.ProdOdbcConnectionString));
//            services.AddScoped<IDbConnection>(_ => new OdbcConnection(appSettings?.ConnectionStrings?.ProdOdbcConnectionString));
//            break;
//        case ConstantSupplier.OLEDB:
//            services.AddDbContext<SecurityDBContext>(options =>
//                options.UseJetOleDb(appSettings?.ConnectionStrings?.ProdOledbConnectionString));
//            services.AddScoped<IDbConnection>(_ => new OleDbConnection(appSettings?.ConnectionStrings?.ProdOledbConnectionString));
//            break;
//    }
//}
//#endregion

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
               .WithExposedHeaders("X-Pagination");
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