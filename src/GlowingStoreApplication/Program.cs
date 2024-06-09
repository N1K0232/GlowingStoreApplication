using System.Diagnostics;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using GlowingStoreApplication.Authentication;
using GlowingStoreApplication.Authentication.Entities;
using GlowingStoreApplication.Authentication.Handlers;
using GlowingStoreApplication.Authentication.Requirements;
using GlowingStoreApplication.BusinessLayer.Diagnostics.HealthChecks;
using GlowingStoreApplication.BusinessLayer.Mapping;
using GlowingStoreApplication.BusinessLayer.Services;
using GlowingStoreApplication.BusinessLayer.Settings;
using GlowingStoreApplication.BusinessLayer.StartupServices;
using GlowingStoreApplication.BusinessLayer.Validations;
using GlowingStoreApplication.DataAccessLayer;
using GlowingStoreApplication.Exceptions;
using GlowingStoreApplication.Extensions;
using GlowingStoreApplication.StorageProviders;
using GlowingStoreApplication.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using MinimalHelpers.OpenApi;
using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;
using Serilog;
using TinyHelpers.AspNetCore.Extensions;
using TinyHelpers.AspNetCore.Swagger;
using TinyHelpers.Extensions;
using TinyHelpers.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder.Services, builder.Configuration, builder.Environment, builder.Host);

var app = builder.Build();
Configure(app, app.Environment, app.Services);

await app.RunAsync();

void ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment, IHostBuilder host)
{
    host.UseSerilog((hostingContext, loggerConfiguration) =>
    {
        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
    });

    var appSettings = services.ConfigureAndGet<AppSettings>(configuration, nameof(AppSettings));
    var jwtSettings = services.ConfigureAndGet<JwtSettings>(configuration, nameof(JwtSettings));
    var swaggerSettings = services.ConfigureAndGet<SwaggerSettings>(configuration, nameof(SwaggerSettings));

    services.AddRequestLocalization(appSettings.SupportedCultures);
    services.AddWebOptimizer(minifyCss: true, minifyJavaScript: environment.IsProduction());

    services.AddHttpContextAccessor();
    services.AddMemoryCache();

    services.AddExceptionHandler<DefaultExceptionHandler>();
    services.AddRazorPages();

    services.AddHealthChecks().AddCheck<SqlConnectionHealthCheck>("sql")
        .AddDbContextCheck<AuthenticationDbContext>("identity")
        .AddDbContextCheck<ApplicationDbContext>("database");

    services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        options.SerializerOptions.Converters.Add(new UtcDateTimeConverter());
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

    services.AddProblemDetails(options =>
    {
        options.CustomizeProblemDetails = context =>
        {
            var statusCode = context.ProblemDetails.Status.GetValueOrDefault(StatusCodes.Status500InternalServerError);
            context.ProblemDetails.Type ??= $"https://httpstatuses.io/{statusCode}";
            context.ProblemDetails.Title ??= ReasonPhrases.GetReasonPhrase(statusCode);
            context.ProblemDetails.Instance ??= context.HttpContext.Request.Path;
            context.ProblemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
        };
    });

    services.AddAutoMapper(typeof(ImageMapperProfile).Assembly);
    services.AddValidatorsFromAssemblyContaining<SaveCategoryRequestValidator>();

    services.AddFluentValidationAutoValidation(options =>
    {
        options.DisableDataAnnotationsValidation = true;
    });

    services.AddOperationResult(options =>
    {
        options.ErrorResponseFormat = ErrorResponseFormat.List;
    });

    if (swaggerSettings.Enabled)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = swaggerSettings.Title,
                Version = swaggerSettings.Version
            });

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Insert JWT token with the \"Bearer \" prefix",
                Name = HeaderNames.Authorization,
                Type = SecuritySchemeType.ApiKey
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });

            options.AddDefaultResponse();
            options.AddAcceptLanguageHeader();
            options.AddFormFile();
        });
    }

    services.AddSqlServer<ApplicationDbContext>(configuration.GetConnectionString("SqlConnection"));
    services.AddScoped<IApplicationDbContext>(services => services.GetRequiredService<ApplicationDbContext>());

    services.AddSqlServer<AuthenticationDbContext>(configuration.GetConnectionString("SqlConnection"));
    services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Lockout.MaxFailedAccessAttempts = 3;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
        options.User.RequireUniqueEmail = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
    })
    .AddEntityFrameworkStores<AuthenticationDbContext>()
    .AddDefaultTokenProviders();

    var azureStorageConnectionString = configuration.GetConnectionString("AzureStorageConnection");
    if (azureStorageConnectionString.HasValue() && appSettings.ContainerName.HasValue())
    {
        services.AddAzureStorage(options =>
        {
            options.ConnectionString = azureStorageConnectionString;
            options.ContainerName = appSettings.ContainerName;
        });
    }
    else
    {
        services.AddFileSystemStorage(options =>
        {
            options.SiteRootFolder = environment.ContentRootPath ?? AppContext.BaseDirectory;
            options.StorageFolder = appSettings.StorageFolder;
        });
    }

    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecurityKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

    services.AddScoped<IAuthorizationHandler, UserActiveHandler>();
    services.AddAuthorization(options =>
    {
        var policyBuilder = new AuthorizationPolicyBuilder().RequireAuthenticatedUser();
        policyBuilder.Requirements.Add(new UserActiveRequirement());

        options.DefaultPolicy = policyBuilder.Build();

        options.AddPolicy("Administrator", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole(RoleNames.Administrator, RoleNames.PowerUser);
            policy.Requirements.Add(new UserActiveRequirement());
        });

        options.AddPolicy("UserActive", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole(RoleNames.User);
            policy.Requirements.Add(new UserActiveRequirement());
        });
    });

    services.Scan(scan => scan.FromAssemblyOf<IdentityService>()
        .AddClasses(classes => classes.InNamespaceOf<IdentityService>())
        .AsImplementedInterfaces()
        .WithScopedLifetime());

    services.AddHostedService<IdentityRoleService>();
}

void Configure(IApplicationBuilder app, IWebHostEnvironment environment, IServiceProvider services)
{
    var appSettings = services.GetRequiredService<IOptions<AppSettings>>().Value;
    var swaggerSetting = services.GetRequiredService<IOptions<SwaggerSettings>>().Value;

    environment.ApplicationName = appSettings.ApplicationName;

    app.UseHttpsRedirection();
    app.UseRequestLocalization();

    app.UseRouting();
    app.UseWebOptimizer();

    app.UseWhen(context => context.IsWebRequest(), builder =>
    {
        if (!environment.IsDevelopment())
        {
            builder.UseExceptionHandler("/Errors/500");
            builder.UseHsts();
        }

        builder.UseStatusCodePagesWithReExecute("/Errors/{0}");
    });

    app.UseWhen(context => context.IsApiRequest(), builder =>
    {
        builder.UseExceptionHandler();
        builder.UseStatusCodePages();
    });

    app.UseDefaultFiles();
    app.UseStaticFiles();

    if (swaggerSetting.Enabled)
    {
        app.UseMiddleware<SwaggerAuthenticationMiddleware>();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", $"{swaggerSetting.Title} {swaggerSetting.Version}");
            options.InjectStylesheet("/css/swagger.css");
        });
    }

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseSerilogRequestLogging(options =>
    {
        options.IncludeQueryInRequestPath = true;
    });

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapEndpoints();
        endpoints.MapRazorPages();
        endpoints.MapHealthChecks("/status", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                var result = JsonSerializer.Serialize(
                new
                {
                    status = report.Status.ToString(),
                    duration = report.TotalDuration.TotalMilliseconds,
                    details = report.Entries.Select(entry => new
                    {
                        service = entry.Key,
                        status = entry.Value.Status.ToString(),
                        description = entry.Value.Description,
                        exception = entry.Value.Exception?.Message,
                    })
                });

                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsync(result);
            }
        });
    });
}