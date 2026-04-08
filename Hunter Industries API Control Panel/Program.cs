// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Components;
using HunterIndustriesAPIControlPanel.Implementations;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Services;

namespace HunterIndustriesAPIControlPanel
{
    public class Program
    {
        // Configures the application at startup.
        public static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Path.Combine(AppContext.BaseDirectory, "log4net.config")));

            ILoggerService _logger = new LoggerServiceWrapper("System");

            _logger.LogMessage(StandardValues.LoggerValues.Info, "Starting Website");

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            _logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Builder");

            builder.Services.AddRazorComponents().AddInteractiveServerComponents();

            APISettingsModel apiSettings = new();

            builder.Configuration.Bind("AppSettings", apiSettings);

            _logger.LogMessage(StandardValues.LoggerValues.Debug, "Loaded Configuration");

            builder.Services.AddSingleton(apiSettings);
            builder.Services.AddSingleton<ExampleAPIService>();
            builder.Services.AddScoped<Radzen.DialogService>();
            builder.Services.AddScoped<Radzen.NotificationService>();
            builder.Services.AddScoped<Radzen.TooltipService>();
            builder.Services.AddScoped<Radzen.ContextMenuService>();
            builder.Services.AddScoped<UserModel>();

            _logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Services");

            WebApplication app = builder.Build();

            _logger.LogMessage(StandardValues.LoggerValues.Debug, "Built Application");

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            _logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured HTTPS Redirection");

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

            _logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Status Code Pages");

            app.UseAntiforgery();

            _logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Antiforgery");

            app.MapStaticAssets();

            _logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Static Assets");

            app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

            _logger.LogMessage(StandardValues.LoggerValues.Debug, "Mapped Razor Components with Interactive Server Render Mode");
            _logger.LogMessage(StandardValues.LoggerValues.Info, "Running Website");

            app.Run();
        }
    }
}
