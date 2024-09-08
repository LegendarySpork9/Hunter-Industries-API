// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace HunterIndustriesAPI
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Path.Combine(AppContext.BaseDirectory, "Log4Net.config")));
            LoggerService _logger = new("Application");
            _logger.LogMessage(StandardValues.LoggerValues.Info, "Logging Started");

            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings - Development.json")
            .Build();

            DatabaseModel.ConnectionString = configuration["SQLConnectionString"];

            configuration.GetSection("JwtSettings").Get<ValidationModel>();

            _logger.LogMessage(StandardValues.LoggerValues.Debug, $"Connection String: {DatabaseModel.ConnectionString}");
            _logger.LogMessage(StandardValues.LoggerValues.Debug, $"Valid Token Issuer: {ValidationModel.Issuer}");
            _logger.LogMessage(StandardValues.LoggerValues.Debug, $"Valid Token Audience: {ValidationModel.Audience}");
            _logger.LogMessage(StandardValues.LoggerValues.Debug, $"Valid Token Security Key: {ValidationModel.SecretKey}");

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "1.0.0",
                    Title = "Hunter Industries API",
                    Description = "This is the OpenAPI documentation for the Hunter Industries API.",
                    Contact = new OpenApiContact
                    {
                        /* Change these details to as see fit. */
                        Name = "Development Team",
                        Email = "Toby.Hunter@Hunter-Industries.co.uk"
                    }
                    /* Add a terms of service and license. */
                });

                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));

                options.OperationFilter<RequiredParameterOperationFilter>();

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
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
                                Id = "Bearer"
                            }
                        },

                        Array.Empty<string>()
                    }
                });
            });

            _logger.LogMessage(StandardValues.LoggerValues.Debug, "Swagger Generation Configured.");

            builder.Services.AddCors(Options =>
            {
                Options.AddPolicy("AllowAllOrigins",
                builder => builder.WithOrigins("https://hunter-industries.co.uk").AllowAnyHeader().AllowAnyMethod());
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = ValidationModel.Issuer,
                    ValidAudience = ValidationModel.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ValidationModel.SecretKey)),
                    ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Assistant", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "Assistant API");
                });

                options.AddPolicy("BookReader", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "Book Reader API");
                });

                options.AddPolicy("APIControlPanel", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "Control Panel API");
                });

                options.AddPolicy("AIAccess", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c => (c.Type == "scope" && c.Value == "Assistant API") || (c.Type == "scope" && c.Value == "Control Panel API")));
                });
            });

            _logger.LogMessage(StandardValues.LoggerValues.Debug, "Authentication Configured.");

            var app = builder.Build();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "CSS")),
                RequestPath = "/CSS"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "Content")),
                RequestPath = "/Content"
            });

            app.UseSwagger(options =>
            {
                options.RouteTemplate = "api/swagger/{documentname}/swagger.json";
            });

            app.UseSwaggerUI(options =>
            {
                if (app.Environment.IsDevelopment())
                {
                    options.SwaggerEndpoint("https://localhost:7026/api/swagger/v1/swagger.json", "V1.0.0");
                }

                if (app.Environment.IsStaging())
                {
                    options.SwaggerEndpoint("https://hunter-industries.co.uk/qa/api/swagger/v1/swagger.json", "V1.0.0");
                    app.UsePathBase("/qa");
                }

                else
                {
                    options.SwaggerEndpoint("https://hunter-industries.co.uk/api/swagger/v1/swagger.json", "V1.0.0");
                }
                
                options.RoutePrefix = "api/swagger";
                options.InjectStylesheet("/CSS/Swagger.css");
            });

            app.UseCors("AllowAllOrigins");

            app.UseHttpsRedirection();

            app.MapControllers();

            app.UseAuthentication();

            app.UseAuthorization();

            _logger.LogMessage(StandardValues.LoggerValues.Debug, "Application Built.");

            app.Run();
        }
    }
}