// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Models;
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
            // Access the json inside the appsettings file.
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings - Development.json")
            .Build();

            DatabaseModel.ConnectionString = configuration["SQLConnectionString"];

            configuration.GetSection("JwtSettings").Get<ValidationModel>();

            // Builds the web application.
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Sets up the swagger page.
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

            builder.Services.AddCors(Options =>
            {
                Options.AddPolicy("AllowAllOrigins",
                builder => builder.WithOrigins("https://hunter-industries.co.uk").AllowAnyHeader().AllowAnyMethod());
            });

            // Sets up the authentication token and Scopes.
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

                options.AddPolicy("AssistantControlPanel", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "Assistant Control Panel API");
                });

                options.AddPolicy("AIAccess", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c => (c.Type == "scope" && c.Value == "Assistant API") || (c.Type == "scope" && c.Value == "Assistant Control Panel API")));
                });
            });

            // Runs the web application.
            var app = builder.Build();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "CSS")),
                RequestPath = "/CSS"
            });

            app.UseSwagger(options =>
            {
                options.RouteTemplate = "api/swagger/{documentname}/swagger.json";
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/api/swagger/v1/swagger.json", "V1.0.0");
                options.RoutePrefix = "api/swagger";
                options.InjectStylesheet("/CSS/Swagger.css");
            });

            app.UseCors("AllowAllOrigins");

            app.UseHttpsRedirection();

            app.MapControllers();

            app.UseAuthentication();

            app.UseAuthorization();

            app.Run();
        }
    }
}