using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using EIA.S0.WebApi.Settings;

namespace EIA.S0.WebApi.ServiceDefaults;

/// <summary>
/// swagger.
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// 添加swagger.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="settings"></param>
    public static void AddDefaultSwagger(this IServiceCollection services, AppSettings settings)
    {
        if (!settings.Swagger.Enable)
        {
            return;
        }

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            // API DOC
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = settings.AppName,
                Version = "v1"
            });

            // 添加 JWT 支持
            if (settings.Swagger.Authorization)
            {
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "请输入 Bearer token."
                });
                options.AddOperationFilterInstance(new SecurityRequirementsOperationFilter());
            }

            // ignore obsolete
            options.IgnoreObsoleteActions();
            options.IgnoreObsoleteProperties();

            // include XML comments
            var assembly = typeof(Program).Assembly;
            var assemblyRootPath = Path.GetDirectoryName(assembly.Location);
            if (assemblyRootPath != null)
            {
                var assemblyName = assembly.GetName().Name;
                if (assemblyName == null)
                {
                    throw new ArgumentNullException(nameof(assemblyName));
                }
                
                var webXmlPath = Path.Combine(assemblyRootPath, $"{assemblyName}.xml");
                if (File.Exists(webXmlPath))
                {
                    options.IncludeXmlComments(webXmlPath, true);
                }
                
                var baseName = string.Join('.', assemblyName.Split('.').SkipLast(1));
                var contractsXmlPath = Path.Combine(assemblyRootPath, $"{baseName}.Contracts.xml");
                if (File.Exists(contractsXmlPath))
                {
                    options.IncludeXmlComments(contractsXmlPath);
                }
                var applicationXmlPath = Path.Combine(assemblyRootPath, $"{baseName}.Application.xml");
                if (File.Exists(applicationXmlPath))
                {
                    options.IncludeXmlComments(applicationXmlPath);
                }
            }

            options.SupportNonNullableReferenceTypes();
        });
    }

    /// <summary>
    /// Operation Filter.
    /// </summary>
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        /// <summary>
        /// apply.
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAuth = context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
                ||
                (context.MethodInfo.DeclaringType != null && context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any());

            if (!hasAuth) return;

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, context.Document)] = []
                }
            };
        }
    }
}