using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using EIA.S0.WebApi.Settings;

namespace EIA.S0.WebApi.ServiceDefaults;

/// <summary>
/// 授权扩展.
/// </summary>
public static class AuthenticationExtensions
{
    /// <summary>
    /// 添加认证.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    public static void AddDefaultAuthentication(this IServiceCollection services, AppSettings settings)
    {
        if (settings.OAuth == null)
        {
            return;
        }

        // Authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = settings.OAuth.Authority;
                options.Audience = settings.OAuth.Audience;
                options.RequireHttpsMetadata = false;
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = settings.OAuth.ValidateAudience
                };
            });
    }
}