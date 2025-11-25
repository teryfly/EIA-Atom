using Microsoft.EntityFrameworkCore;
using EIA.S0.WebApi.ServiceDefaults;
using EIA.S0.WebApi.Settings;
using EIA.S0.WebApi.Validators;
using EIA.S0.WebApi.Extensions;
using EIA.S0.WebApi.Initializers;

var builder = WebApplication.CreateBuilder(args);

var settings = builder.Configuration.Get<AppSettings>();
if (settings == null)
{
    throw new ArgumentNullException(nameof(settings));
}

// 配置验证.
ObjectModelValidator.Validate(settings);

builder.Services.AddSingleton(settings);

// 添加默认服务.
builder.Services.AddServiceDefaults(settings);

// 添加应用服务.
builder.Services.AddApplicationServices(settings);

var app = builder.Build();

// use service defaults.
app.UseServiceDefaults(settings);

// 添加数据库迁移.
using var scope = app.Services.CreateScope();
var initializers = scope.ServiceProvider.GetServices<IInitializer>();
foreach (var initializer in initializers)
{
    await initializer.InitializeAsync();
}

app.Run();