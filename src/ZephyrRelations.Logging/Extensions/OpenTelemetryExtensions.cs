﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace ZephyrRelations.OpenTelemetry.Extensions;

public static class Extensions
{
    private const string JaegerUrlKey = "Jaeger:Url";
    private const string JaegerPortKey = "Jaeger:Port";
    private const string PrometheusEndpointPathKey = "Prometheus:EndpointPath";
    private const string ServiceName = "ServiceSettings:ServiceName";

    public static IServiceCollection UseOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var tracingOtlpEndpoint = configuration["OTLP_ENDPOINT_URL"];
        var otel = services.AddOpenTelemetry();

        // Add Metrics for ASP.NET Core and our custom metrics and export to Prometheus
        otel.WithMetrics(metrics => metrics
            // Metrics provider from OpenTelemetry
            .AddAspNetCoreInstrumentation()
            //.AddMeter(greeterMeter.Name)
            // Metrics provides by ASP.NET Core in .NET 8
            .AddMeter("Microsoft.AspNetCore.Hosting")
            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
            .AddPrometheusExporter());

        // Add Tracing for ASP.NET Core and our custom ActivitySource and export to Jaeger
        otel.WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation();
            tracing.AddHttpClientInstrumentation();
            //tracing.AddSource(greeterActivitySource.Name);
            if (tracingOtlpEndpoint != null)
            {
                tracing.AddOtlpExporter(otlpOptions =>
                 {
                     otlpOptions.Endpoint = new Uri(tracingOtlpEndpoint);
                 });
            }
            else
            {
                tracing.AddConsoleExporter();
            }
        });

        var configErrorLogger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        ValidateConfiguration(configuration, configErrorLogger);

        // try
        // {
        //     services.AddOpenTelemetry()
        //         //.ConfigureResource(builder => builder.AddService(ServiceName))
        //         .WithTracing(builder =>
        //         {
        //             builder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(ServiceName))
        //                 .AddSource(ServiceName)
        //                 //.SetSampler(new AlwaysOnSampler())
        //                 .AddAspNetCoreInstrumentation()
        //                 .AddHttpClientInstrumentation()
        //             .AddOtlpExporter(opts =>
        //             {
        //                 opts.Endpoint = new Uri(JaegerUrlKey);
        //             })
        //             ;

        //         }
        //         // .AddConsoleExporter()
        //         // .AddAspNetCoreInstrumentation()
        //         // .AddOtlpExporter(otlpOptions =>
        //         // {
        //         //     otlpOptions.Endpoint = new Uri(JaegerUrlKey);
        //         // })
        //         )
        //         .WithMetrics(builder => builder
        //             .AddConsoleExporter()
        //             .AddPrometheusExporter(prometheusOptions =>
        //             {
        //                 prometheusOptions.ScrapeEndpointPath = PrometheusEndpointPathKey;
        //             }
        //         ));
        // }
        // catch (Exception ex)
        // {
        //     string errorMessage = "Error configuring OpenTelemetry with Jaeger and Prometheus";
        //     configErrorLogger.Error(ex, errorMessage);
        //     throw new InvalidOperationException(errorMessage, ex);
        // }

        return services;
    }

    private static void ValidateConfiguration(IConfiguration configuration, ILogger configErrorLogger)
    {
        if (string.IsNullOrWhiteSpace(configuration[ServiceName]))
        {
            string errorMessage = $"{ServiceName} is not configured in appsettings.json";
            configErrorLogger.Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (string.IsNullOrWhiteSpace(configuration[JaegerUrlKey]))
        {
            string errorMessage = $"{JaegerUrlKey} is not configured in appsettings.json";
            configErrorLogger.Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (string.IsNullOrWhiteSpace(configuration[JaegerPortKey]))
        {
            string errorMessage = $"{JaegerPortKey} is not configured in appsettings.json";
            configErrorLogger.Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (string.IsNullOrWhiteSpace(configuration[PrometheusEndpointPathKey]))
        {
            string errorMessage = $"{PrometheusEndpointPathKey} is not configured in appsettings.json";
            configErrorLogger.Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }
    }
}
