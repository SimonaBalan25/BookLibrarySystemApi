using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using Microsoft.ApplicationInsights.Extensibility.Implementation;

namespace BookLibrarySystem.Web.Middleware
{
    public static class ApplicationInsightsMiddleware
    {
        public static IServiceCollection AddApplicationInsightsLogging(this IServiceCollection services, ConfigurationManager configuration)
        {
            ApplicationInsightsServiceOptions telemetryOptions = new();
            telemetryOptions.ConnectionString = configuration["ApplicationInsights:ConnectionString"];
            telemetryOptions.EnableRequestTrackingTelemetryModule = true;
            telemetryOptions.EnableDependencyTrackingTelemetryModule = true;
            telemetryOptions.EnableDiagnosticsTelemetryModule = true;
            telemetryOptions.EnableQuickPulseMetricStream = true;
            telemetryOptions.EnableAdaptiveSampling = false;
            services.AddApplicationInsightsTelemetry(telemetryOptions);

            services.AddLogging(logBuilder =>
            {
                logBuilder.AddApplicationInsights();
            });

            // Set api key for AppInsights module.
            var apiKey = configuration["ApplicationInsights:APIKey"];

            if (!string.IsNullOrEmpty(apiKey))
            {
                var module = TelemetryModules.Instance.Modules
                                             .OfType<QuickPulseTelemetryModule>().FirstOrDefault();
                if (module != null)
                {
                    module.AuthenticationApiKey = apiKey;
                }
            }

            return services;
        }

        public static IApplicationBuilder UseApplicationInsightsLogging(this IApplicationBuilder app)
        {
            app.UseApplicationInsightsExceptionTelemetry();
            return app;
        }
    }
}
