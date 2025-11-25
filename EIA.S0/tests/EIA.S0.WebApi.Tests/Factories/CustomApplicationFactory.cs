using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EIA.S0.WebApi.Initializers;

namespace EIA.S0.WebApi.Tests.Factories;

public class CustomApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("UnitTest");
        builder.ConfigureServices(services =>
        {
            // 移除db初始化.
            var dbContextDescriptor = services.SingleOrDefault(d => d.ImplementationType == typeof(DbInitializer));
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            services.AddAuthentication("Test").AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", null);
        });
    }
}