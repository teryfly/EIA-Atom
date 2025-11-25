using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi;
using EIA.S0.WebApi.Json;
using EIA.S0.WebApi.Middlewares;
using EIA.S0.WebApi.Settings;

namespace EIA.S0.WebApi.ServiceDefaults;

/// <summary>
/// 默认扩展.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// 添加默认服务.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="settings"></param>
    public static void AddServiceDefaults(this IServiceCollection services, AppSettings settings)
    {
        // 转发头
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.ForwardedForHeaderName = "X-Real-IP";
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        // 控制器
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new LocalDateTimeConverter());
        });
        
        // 跨域
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            });
        });
        
        // 认证
        services.AddDefaultAuthentication(settings);
        
        // swagger.
        services.AddDefaultSwagger(settings);
    }

    /// <summary>
    /// 使用默认服务.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="settings"></param>
    public static void UseServiceDefaults(this WebApplication app, AppSettings settings)
    {
        app.UseForwardedHeaders();

        if (!string.IsNullOrEmpty(settings.PathBase))
        {
            // app.Logger.LogDebug("Using PATH BASE '{pathBase}'", settings.PathBase);
            app.UsePathBase(settings.PathBase);
        }

        if (settings.Swagger.Enable)
        {
            app.UseSwagger(c =>
                {
                    c.PreSerializeFilters.Add((swagger, req) =>
                    {
                        var basePath = req.PathBase.Value ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(settings.PathBase) && !basePath.StartsWith(settings.PathBase))
                        {
                            basePath = settings.PathBase + basePath;
                        }
                
                        swagger.Servers = new List<OpenApiServer>
                        {
                            new() { Url = $"{req.Scheme}://{req.Host.Value}{basePath}" }
                        };
                    });
                })
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint(
                        $"{(!string.IsNullOrWhiteSpace(settings.PathBase) ? settings.PathBase : string.Empty)}/swagger/v1/swagger.json",
                        settings.AppName);
                });
        }

        app.UseMiddleware<ExceptionHandlerMiddleware>();
        app.UseCors();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}