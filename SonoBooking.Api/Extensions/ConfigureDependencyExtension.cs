using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetCore.AutoRegisterDi;
using Newtonsoft.Json.Converters;
using SonoBooking.Domain.Entities.Identity;
using SonoBooking.Infrastructure.Context;
using SonoBooking.Infrastructure.DataInitializer;
using SonoBooking.Infrastructure.UnitOfWork;
using SonoBooking.Api.Extensions.Swagger.Headers;
using SonoBooking.Api.Extensions.Swagger.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using SonoBooking.Common.Extensions;
using SonoBooking.Common.Helpers.JsonHelper;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.DTO.Identity.User;
using SonoBooking.Common.DTO.Email;
using SonoBooking.Application.Services.Email;
using SonoBooking.Application.Services.Housing.Reservations;
using SonoBooking.Application.Services.Housing.Notifications;
using SonoBooking.Common.Helpers.HttpClient.RestSharp;
using SonoBooking.Application.Mapping;
using SonoBooking.Application.Services.Validators.Base;
using SonoBooking.Integration.FileRepository;
using SonoBooking.Integration.CacheRepository;
using SonoBooking.Integration.UserRepository;
using SonoBooking.Application.Helper;
using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Infrastructure.UnitOfWork;
using SonoBooking.Application.Services.Identity.Accounts;
using SonoBooking.Application.Services.BackgroundJobs.Housing.Reservations;
using SonoBooking.Application.Services.BackgroundJobs.Housing.Units;
using SonoBooking.Application.Services.BusinessNotification.Chat;
using SonoBooking.Application.Services.BusinessNotification.Notification;
using SonoBooking.Api.Hubs;
using SonoBooking.Api.Services.BusinessNotification;
using Hangfire;
using Microsoft.AspNetCore.SignalR;

namespace SonoBooking.Api.Extensions
{
    /// <summary>
    /// Dependency Extensions
    /// </summary>
    public static class ConfigureDependencyExtension
    {
        private const string ConnectionStringName = "Default";
        /// <summary>
        /// Register Extensions
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterDbContext(configuration);
            services.RegisterCores();
            services.RegisterLocalization();
            services.RegisterIntegrationRepositories();
            services.RegisterCustomRepositories();
            services.RegisterAutoMapper();
            services.RegisterCommonServices(configuration);
            services.ConfigureAuthentication(configuration);
            services.RegisterHttpClientHelpers();
            services.RegisterValidators();
            services.RegisterApiMonitoring();
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.Converters.Add(new TrimmingConverter());
            });
            services.RegisterApiVersioning();
            services.RegisterSwaggerConfig();
            services.RegisterLowerCaseUrls();
            services.RegisterSignalR();
            services.AddScoped<IChatRealtimePublisher, ChatRealtimePublisher>();
            services.AddScoped<INotificationRealtimePublisher, NotificationRealtimePublisher>();
            services.RegisterHangfire(configuration);
            services.AddScoped<IAccountService, AccountService>();
            services.AddIdentityCore<User>(options =>
            {
                // configuration can be written here:
                // builder.Services.Configure<IdentityOptions>
                options.SignIn.RequireConfirmedAccount = true;

                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 3;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(60);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;


                options.Lockout.MaxFailedAccessAttempts = 2;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedAccount = false;
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;

            })
            .AddRoles<Role>().AddEntityFrameworkStores<SonoBookingDbContext>()
            .AddApiEndpoints()
            .AddDefaultTokenProviders();
            services.AddHttpContextAccessor();
            // Read User Data from HttpContext (empty user outside HTTP requests, e.g. Hangfire jobs)
            services.AddTransient(provider =>
            {
                HttpContext? context = provider.GetService<IHttpContextAccessor>()?.HttpContext;
                if (context == null)
                {
                    return new UserDataDto(
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        [],
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        null);
                }

                ClaimsPrincipal user = context.User;

                string Id =
                user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ??
                string.Empty;

                string name =
                user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ??
                string.Empty;

                string organizationId =
                user.Claims.FirstOrDefault(x => x.Type == AuthConstants.OrgId)?.Value ??
                string.Empty;

                string leaderId =
                user.Claims.FirstOrDefault(x => x.Type == AuthConstants.LeaderId)?.Value ??
                string.Empty;

                string governorateId =
                user.Claims.FirstOrDefault(x => x.Type == AuthConstants.GovId)?.Value ??
                string.Empty;

                string employeeId =
                user.Claims.FirstOrDefault(x => x.Type == AuthConstants.EmployeeId)?.Value ??
                string.Empty;

                string role =
                user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value ?? "";


                List<UserPermissionDto> permissions =
                [.. user.Claims
                .Where(c =>
                    c.Type != ClaimTypes.NameIdentifier && c.Type != ClaimTypes.Name &&
                    c.Type != AuthConstants.OrgId && c.Type != ClaimTypes.Role &&
                    c.Type != AuthConstants.LeaderId &&
                    c.Type != AuthConstants.EmployeeId &&
                    c.Type != "exp" && c.Type != "iss")
                .Select(c =>
                    new UserPermissionDto
                    {
                        Name = c.Type,
                        Value = c.Value
                    })];

                //bool parsedUserId = int.TryParse(stringId, out int id);
                //bool parsedOrgId = int.TryParse(stringOrganizationId, out int organizationId);

                return new UserDataDto(Id, name, role, permissions, organizationId, leaderId, governorateId,
                    string.IsNullOrWhiteSpace(employeeId) ? null : employeeId);
            });
            services.AddCors(option =>
            {
                option.AddPolicy("policy", builder =>
                {
                    builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed(origin =>
                        {
                            if (string.IsNullOrWhiteSpace(origin))
                            {
                                return false;
                            }

                            if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                            {
                                return false;
                            }

                            if (uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
                                || uri.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase))
                            {
                                return true;
                            }

                            return uri.Host.EndsWith(".sono.net", StringComparison.OrdinalIgnoreCase)
                                || uri.Host.Equals("sonobooking.runasp.net", StringComparison.OrdinalIgnoreCase);
                        })
                        .AllowCredentials();
                });
            });

            // Register Email Service
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }

        /// <summary>
        /// Registers SignalR services with custom options.
        /// </summary>
        /// <param name="services">The service collection to add SignalR to.</param>
        public static void RegisterSignalR(this IServiceCollection services)
        {
            services.AddSingleton<IUserIdProvider, SignalRUserIdProvider>();
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
                options.MaximumReceiveMessageSize = 102400000; // 100MB
                options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
                options.HandshakeTimeout = TimeSpan.FromSeconds(30);
            })
            .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });
        }


        /// <summary>
        /// Registers Hangfire services with SQL Server storage.
        /// </summary>
        /// <param name="services">The service collection to add Hangfire to.</param>
        /// <param name="configuration"></param>
        public static void RegisterHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(x =>
            {
                x.UseSqlServerStorage(configuration.GetConnectionString(ConnectionStringName));
            });
            services.AddHangfireServer();
        }

        /// <summary>
        /// Add DbContext
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        private static void RegisterDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SonoBookingDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString(ConnectionStringName));
            });
            services.AddScoped<DbContext, SonoBookingDbContext>();
            services.AddSingleton<IDataInitializer>(sp =>
            {
                var env = sp.GetRequiredService<IWebHostEnvironment>();
                return new DataInitializer(env.ContentRootPath);
            });
        }
        /// <summary>
        /// Add DbContext
        /// </summary>
        /// <param name="services"></param>
        private static void RegisterApiMonitoring(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddDbContextCheck<SonoBookingDbContext>();
        }
        /// <summary>
        /// Configure Authentication With Identity Server
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwt = configuration.GetSection("Jwt");
            services.AddAuthentication(options =>
            {
                //options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    // Remove Default Plus Time (5 min)
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var path = context.HttpContext.Request.Path;
                        if (!path.StartsWithSegments("/hubs") &&
                            !path.StartsWithSegments("/api/v1/hubs"))
                        {
                            return Task.CompletedTask;
                        }

                        var accessToken = context.Request.Query["access_token"];
                        if (string.IsNullOrEmpty(accessToken))
                        {
                            var authHeader = context.Request.Headers.Authorization.ToString();
                            if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                            {
                                accessToken = authHeader["Bearer ".Length..].Trim();
                            }
                        }

                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });
            //.AddJwtBearer("Bearer", config =>
            //{
            //    config.Authority = configuration["StsConfig:Authority"];
            //    config.Audience = configuration["StsConfig:Audience"];
            //    config.RequireHttpsMetadata = false;

            //});
        }
        /// <summary>
        /// Register Http Client Helpers
        /// </summary>
        /// <param name="services"></param>
        private static void RegisterHttpClientHelpers(this IServiceCollection services)
        {
            services.AddTransient<IRestSharpClient, RestSharpClient>();
        }

        /// <summary>
        /// register auto-mapper
        /// </summary>
        /// <param name="services"></param>
        private static void RegisterAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingService).Assembly));
        }

        /// <summary>
        /// register auto-mapper
        /// </summary>
        /// <param name="services"></param>
        private static void RegisterLocalization(this IServiceCollection services)
        {
            services.AddLocalization();
        }


        /// <summary>
        /// Register Business Validators
        /// </summary>
        /// <param name="services"></param>
        public static void RegisterValidators(this IServiceCollection services)
        {
            services.AddTransient(typeof(IValidator<>), typeof(Validator<>));
        }

        /// <summary>
        /// register Integration Repositories
        /// </summary>
        /// <param name="services"></param>
        private static void RegisterIntegrationRepositories(this IServiceCollection services)
        {
            services.AddTransient<IFileRepository, FileRepository>();
            services.AddTransient<ICacheRepository, CacheRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
        }

        /// <summary>
        /// register Custom Repositories
        /// </summary>
        /// <param name="services"></param>
        private static void RegisterCustomRepositories(this IServiceCollection services)
        {
            //services.AddScoped<ICompanyCustomRepository, CompanyCustomRepository>();
        }



        /// <summary>
        /// Register Api Versioning
        /// </summary>
        /// <param name="services"></param>
        private static void RegisterApiVersioning(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            })
            .AddApiExplorer(config =>
            {
                config.GroupNameFormat = "'v'VVV";
                config.SubstituteApiVersionInUrl = true;
            });
        }


        /// <summary>
        /// Lower Case Urls
        /// </summary>
        /// <param name="services"></param>
        private static void RegisterLowerCaseUrls(this IServiceCollection services)
        {
            services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });
        }

        /// <summary>
        /// Swagger Config
        /// </summary>
        /// <param name="services"></param>

        private static void RegisterSwaggerConfig(this IServiceCollection services)
        {
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigureOptions>();
            services.AddSwaggerGen(options =>
            {
                var security = new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                    }
                                },
                                new string[] { }

                            }
                        };
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });
                options.AddSecurityRequirement(security);
                options.OperationFilter<LanguageHeader>();
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            services.AddSwaggerGenNewtonsoftSupport();
        }
        /// <summary>
        /// Register Main Core
        /// </summary>
        /// <param name="services"></param>
        private static void RegisterCores(this IServiceCollection services)
        {
            services.AddSingleton<AppHelper>();
            services.AddTransient(typeof(IBaseService<,,,,,>), typeof(BaseService<,,,,,>));
            services.AddTransient(typeof(IServiceBaseParameter<>), typeof(ServiceBaseParameter<>));
            services.AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            var servicesToScan = Assembly.GetAssembly(typeof(AccountService));
            services.RegisterAssemblyPublicNonGenericClasses(servicesToScan)
                .Where(c => c.Name.EndsWith("Service"))
                .AsPublicImplementedInterfaces();
            services.RegisterAssemblyPublicNonGenericClasses(servicesToScan)
                .Where(c => c.Name.EndsWith("Validator"))
                .AsPublicImplementedInterfaces();
            services.AddScoped<ReservationStatusEmailNotifier>();
            services.AddScoped<HousingNotificationService>();
            services.AddTransient<ReservationNoShowJob>();
            services.AddTransient<ReservationCheckoutJob>();
            services.AddTransient<ReservationUnitAvailabilityGapJob>();
            services.AddSingleton<IUnitAdministrativeStatusJobScheduler, UnitAdministrativeStatusJobScheduler>();
            services.AddTransient<UnitAdministrativeStatusJob>();
        }
    }
}

