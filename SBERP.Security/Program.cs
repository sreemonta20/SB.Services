#region Initial Code (Template)
//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();
#endregion

#region Tested Code
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

//var builder = WebApplication.CreateBuilder(args); // 1

//// Load Configuration
//builder.Configuration
//    .SetBasePath(Directory.GetCurrentDirectory())
//    .AddJsonFile(ConstantSupplier.APP_SETTINGS_FILE_NAME);

//var configuration = builder.Configuration;

//// Configure Serilog
//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(configuration)
//    .Enrich.FromLogContext()
//    .Enrich.WithMachineName()
//    .Enrich.WithEnvironmentUserName()
//    .CreateLogger();

//builder.Host.UseSerilog();

//// Log startup information
//Log.Information(ConstantSupplier.LOG_INFO_APP_START_MSG);
//StringBuilder sb = new StringBuilder();
//sb.AppendLine();
//sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_FIRST);
//sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_SECOND_GATEWAY);
//sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_THIRD_VERSION);
//sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_FOURTH_COPYRIGHT);
//sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_END);
//Log.Logger.Information(sb.ToString());

//// Add services to the container
//var services = builder.Services;

//// Load app settings
//services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
//var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

//// Register Database
//RegisterDatabase(services, appSettings); // 2

//// Add Email Configuration
//services.AddScoped<IEmailService, EmailSender>();

//// Add Controllers with JSON options
//services.AddControllers().AddJsonOptions(options =>
//{
//    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
//}).AddNewtonsoftJson(o =>
//{
//    o.SerializerSettings.ContractResolver = new DefaultContractResolver();
//});

//// Add services to the container
//services.AddApiVersioning(options =>
//{
//    options.AssumeDefaultVersionWhenUnspecified = true; // Use a default version when none is specified
//    options.DefaultApiVersion = new ApiVersion(1, 0);    // Set the default version to 1.0
//    options.ReportApiVersions = true;                   // Include version information in responses
//});

//// Add CORS
//services.AddCors(options =>
//{
//    options.AddPolicy(ConstantSupplier.CORSS_POLICY_NAME, builder =>
//    {
//        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
//    });
//});

//// Add Swagger
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

//#region Configuring authorization middleware into the service

//var key = Encoding.ASCII.GetBytes(appSettings?.JWT?.Key);
//services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = appSettings.JWT.Issuer,
//        ValidAudience = appSettings.JWT.Audience,
//        IssuerSigningKey = new SymmetricSecurityKey(key)
//    };
//});
//#endregion 

//// Add additional services
//services.AddHttpContextAccessor();
//services.AddResponseCompression();
//services.AddDistributedMemoryCache();

//// Register Dependency Injection
//services.AddTransient<ISecurityLogService, SecurityLogService>();
//services.AddTransient<IAuthService, AuthService>();
//services.AddTransient<IDataAnalyticsService, DataAnalyticsService>();
//services.AddTransient<IUserService, UserService>();
//services.AddTransient<IRoleMenuService, RoleMenuService>();
//services.AddScoped<ITokenService, TokenService>();
//services.AddScoped<IDatabaseManager, DatabaseManager>();
//services.AddScoped<IDatabaseHandlerFactory, DatabaseHandlerFactory>();
//services.AddScoped<ValidateModelAttribute>();

//var app = builder.Build();

//// Configure the HTTP request pipeline
//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint(ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_END_POINT, ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_END_POINT_NAME);
//        c.RoutePrefix = string.Empty; // Set Swagger as the default page
//    });
//}

//// Configure middleware
//app.UseHttpsRedirection();
//app.UseRouting();

//app.UseAuthentication();
//app.UseAuthorization();
//app.UseCors(ConstantSupplier.CORSS_POLICY_NAME);
//app.UseResponseCompression();
//app.UseStaticFiles();
//app.UseSerilogRequestLogging();

//app.MapControllers();

//app.Run();

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

#endregion

#region Final Code
//////////////////////////////////////////////Key Changes///////////////////////////////////////////////////////////////////////
////1. Middleware Order: Ensures app.UseRouting(), app.UseCors(), app.UseAuthentication(), and app.UseAuthorization() follow Microsoft's 
////recommended pipeline order.
////2. Structured Comments: Clear sections for configuration, service registration, and middleware.
////3. Static File Handling: Ensures static file middleware(app.UseStaticFiles()) is included early but after HTTPS redirection.
////4. Response Compression: Placed after essential authentication and authorization middlewares.

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using SBERP.DataAccessLayer;
using SBERP.EmailService.Service;
using SBERP.Security.Filter;
using SBERP.Security.Helper;
using SBERP.Security.Models.Configuration;
using SBERP.Security.Persistence;
using SBERP.Security.Service;
using Serilog;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.OracleClient;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Load Configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(ConstantSupplier.APP_SETTINGS_FILE_NAME);

var configuration = builder.Configuration;

// 2. Configure Logging (Serilog)
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentUserName()
    .CreateLogger();

builder.Host.UseSerilog();

Log.Information(ConstantSupplier.LOG_INFO_APP_START_MSG);
StringBuilder sb = new StringBuilder();
sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_FIRST);
sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_SECOND_GATEWAY);
sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_THIRD_VERSION);
sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_FOURTH_COPYRIGHT);
sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_END);
Log.Logger.Information(sb.ToString());

// 3. Configure Services
var services = builder.Services;

// 3.1 Load App Settings
services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

// 3.2 Register Database
RegisterDatabase(services, appSettings);

// 3.3 Add Email Service
services.AddScoped<IEmailService, EmailSender>();

// 3.4 Configure Controllers and JSON Options
services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    })
    .AddNewtonsoftJson(o =>
    {
        o.SerializerSettings.ContractResolver = new DefaultContractResolver();
    });

// 3.5 API Versioning
services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

// 3.6 Add CORS
services.AddCors(options =>
{
    options.AddPolicy(ConstantSupplier.CORSS_POLICY_NAME, builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// 3.7 Add Swagger
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
        Scheme = ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_SCHEME_SCHEME,
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_SCHEME_REF_ID
        }
    };

    c.AddSecurityDefinition(ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_SCHEME_REF_ID, securitySchema);
    var securityRequirement = new OpenApiSecurityRequirement();
    securityRequirement.Add(securitySchema, new[] { ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_SCHEME_REF_ID });
    c.AddSecurityRequirement(securityRequirement);
});

// 3.8 Configure Authentication and Authorization
var key = Encoding.ASCII.GetBytes(appSettings?.JWT?.Key);
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set false if testing locally
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = appSettings.JWT.Issuer,
        ValidAudience = appSettings.JWT.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero // Prevents issues where JWT tokens are still accepted after they expire.
    };
});

services.AddAuthorization();

// 3.9 Add Additional Services
services.AddHttpContextAccessor();
services.AddResponseCompression();
services.AddDistributedMemoryCache();

// 3.10 Register DI Services
services.AddTransient<ISecurityLogService, SecurityLogService>();
services.AddTransient<IAuthService, AuthService>();
services.AddTransient<IDataAnalyticsService, DataAnalyticsService>();
services.AddTransient<IUserService, UserService>();
services.AddTransient<IRoleMenuService, RoleMenuService>();
services.AddScoped<ITokenService, TokenService>();
services.AddScoped<IDatabaseManager, DatabaseManager>();
services.AddScoped<IDatabaseHandlerFactory, DatabaseHandlerFactory>();
services.AddScoped<ValidateModelAttribute>();

// 4. Build the Application
var app = builder.Build();

// 5. Configure Middleware
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

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(ConstantSupplier.CORSS_POLICY_NAME);
app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCompression();

app.MapControllers();
app.Run();
Log.CloseAndFlush(); //Ensures that Serilog flushes all logs before the app shuts down.

// Static Method for Database Registration
static void RegisterDatabase(IServiceCollection services, AppSettings? appSettings)
{
    switch (appSettings?.AppDB)
    {
        case ConstantSupplier.SQLSERVER:
            services.AddDbContext<SecurityDBContext>(options =>
                options.UseSqlServer(appSettings?.ConnectionStrings?.ProdSqlConnectionString));
            services.AddTransient<IDbConnection>(_ => new SqlConnection(appSettings?.ConnectionStrings?.ProdSqlConnectionString));
            break;
        case ConstantSupplier.ORACLE:
            services.AddDbContext<SecurityDBContext>(options =>
                options.UseOracle(appSettings?.ConnectionStrings?.ProdOracleConnectionString));
            services.AddScoped<IDbConnection>(_ => new OracleConnection(appSettings?.ConnectionStrings?.ProdOracleConnectionString));
            break;
        case ConstantSupplier.ODBC:
            services.AddDbContext<SecurityDBContext>(options =>
                options.UseJetOdbc(appSettings?.ConnectionStrings?.ProdOdbcConnectionString));
            services.AddScoped<IDbConnection>(_ => new OdbcConnection(appSettings?.ConnectionStrings?.ProdOdbcConnectionString));
            break;
        case ConstantSupplier.OLEDB:
            services.AddDbContext<SecurityDBContext>(options =>
                options.UseJetOleDb(appSettings?.ConnectionStrings?.ProdOledbConnectionString));
            services.AddScoped<IDbConnection>(_ => new OleDbConnection(appSettings?.ConnectionStrings?.ProdOledbConnectionString));
            break;
    }
}
#endregion