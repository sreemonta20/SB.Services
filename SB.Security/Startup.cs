using AspNetCoreRateLimit;
using SB.EmailService.Service;
using SB.Security.Filter;
using SB.Security.Helper;
using SB.Security.Middlewares;
using SB.Security.Models.Configuration;
using SB.Security.Persistence;
using SB.Security.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Serilog;
using StackExchange.Redis.Extensions.Newtonsoft;
using System.Configuration;
using System.Security;
using System.Text;
using System.Text.Json.Serialization;

namespace SB.Security
{
    public class Startup
    {
        #region Constructor initialization
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        #endregion

        #region Declaration & initialization
        public IConfiguration Configuration { get; }
        #endregion

        #region All methods
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        /// <returns>void</returns>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));

            #region Reading AppSettings from appsettings.json
            var appSettings = Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
            #endregion

            #region Registers the given security db context as a service into the services
            services.AddDbContext<SBSecurityDBContext>(options =>
            options.UseSqlServer(appSettings.ConnectionStrings.SecurityConectionString));
            #endregion

            #region Email Configuration
            services.AddScoped<IEmailService, EmailSender>();
            #endregion

            //services.AddControllers().AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.PropertyNamingPolicy = null;
            //    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            //});

            //services.AddControllers().AddNewtonsoftJson();
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            }).AddNewtonsoftJson(o =>
            {
                o.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            #region Register Cors into the service
            services.AddCors(c =>
            {
                c.AddPolicy(ConstantSupplier.CORSS_POLICY_NAME, options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            #endregion

            #region Register Swagger into the service

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


            #endregion

            #region Api call rate limiting
            //services.AddMemoryCache();
            //services.Configure<IpRateLimitOptions>(options =>
            //{
            //    options.EnableEndpointRateLimiting = true;
            //    options.StackBlockedRequests = false;
            //    options.HttpStatusCode = 429;
            //    options.RealIpHeader = "X-Real-IP";
            //    options.ClientIdHeader = "X-ClientId";
            //    options.GeneralRules = new List<RateLimitRule>
            //    {
            //        new RateLimitRule
            //        {
            //            Endpoint = "POST:/api/User/login",
            //            Period = "10s",
            //            Limit = 2,
            //        }
            //    };
            //});
            #endregion

            services.AddHttpContextAccessor();

            #region Dependency Injection added into service
            //services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            //services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            //services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            //services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
            //services.AddInMemoryRateLimiting();
            //services.Add(new EncryptDecryptMiddleware(appSettings.EncryptKey, appSettings.EncryptIV));
            //services.Add(new DelegatingHandlerProxy<EncryptDecryptMiddleware>(appSettings.EncryptKey, appSettings.EncryptIV));
            //services.AddSingleton<IEncryptDecryptService, EncryptDecryptService>();
            //services.AddSingleton(new EncryptDecryptService(appSettings.EncryptKey, appSettings.EncryptIV));
            //services.Add(new EncryptDecryptMiddleware(new EncryptDecryptService(appSettings.EncryptKey, appSettings.EncryptIV)));
            services.AddTransient<ISecurityLogService, SecurityLogService>();
            services.AddTransient<IUserService, UserService>();
            services.AddScoped<ValidateModelAttribute>();
            //services.AddTransient<EncryptDecryptMiddleware>();
            //services.AddHttpClient("myclient").AddHttpMessageHandler<EncryptDecryptMiddleware>();
            #endregion



            #region Case sentivity registration for all responses

            services.AddResponseCompression();

            #endregion

            #region Register Distributed Cache Service into the service collection
            //To Store session in Memory, This is default implementation of IDistributedCache
            services.AddDistributedMemoryCache();
            #endregion


            #region Configuring authorization middleware into the service
            //If SB hosted on-premise and customer want to use default Identity 
            var key = Encoding.ASCII.GetBytes(appSettings.JWT.Key);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        DateTime secretDate = DateTime.UtcNow.AddSeconds(appSettings.AccessTokenExpireTime);

                        if (DateTime.UtcNow.Subtract(secretDate).TotalSeconds > 0)
                        {
                            context.Response.Headers.Add(ConstantSupplier.AUTHORIZATION_TOKEN_HEADER_ADD_NAME_01,

                                ConstantSupplier.AUTHORIZATION_TOKEN_HEADER_ADD_VALUE_01);
                        }

                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
            #endregion
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <returns>void</returns>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseEncDecMiddleware();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                #region Inject or enable Swagger middleware into the request pipeline to serve generated swagger as a JSON endpoint in case of development env.
                app.UseSwagger();
               
                app.UseSwaggerUI(c => c.SwaggerEndpoint(ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_END_POINT,
                    ConstantSupplier.SWAGGER_SB_API_SERVICE_DOC_END_POINT_NAME));
                #endregion
            }
            app.UseHttpsRedirection();
            app.UseRouting();

            #region Inject cors middleware into the request pipeline
            app.UseCors(ConstantSupplier.CORSS_POLICY_NAME);
            #endregion

            #region Inject authentication and authorization middleware into the request pipeline
            app.UseAuthentication();
            app.UseAuthorization();
            
            #endregion

            app.UseResponseCompression();
            
            //app.UseMiddleware<EncryptDecryptMiddleware>();
            app.UseStaticFiles();

            #region Inject serilog middleware into the request pipeline
            app.UseSerilogRequestLogging();
            #endregion
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        #endregion
    }
}
