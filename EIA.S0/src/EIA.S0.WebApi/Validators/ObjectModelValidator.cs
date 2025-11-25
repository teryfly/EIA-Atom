using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EIA.S0.WebApi.Validators;

/// <summary>
/// 模型验证.
/// </summary>
public static class ObjectModelValidator
{
    /// <summary>
    /// 验证模型.
    /// </summary>
    /// <param name="settings"></param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="Exception"></exception>
    public static void Validate<T>(T settings)
        where T : class
    {
        if (settings == null)
        {
            throw new Exception("模型不能为空.");
        }

        var services = new ServiceCollection();

        // 1. 添加 MVC，以便我们能解析 IObjectModelValidator 等服务
        services.AddControllers();

        // 2. serviceProvider
        var serviceProvider = services.BuildServiceProvider();

        // 3. 获取 IObjectModelValidator 和构造 ActionContext
        var validator = serviceProvider.GetRequiredService<IObjectModelValidator>();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor(),
            new ModelStateDictionary()
        );

        // 4. 执行验证
        validator.Validate(
            actionContext: actionContext,
            validationState: null,
            prefix: "",
            model: settings
        );

        // 5. 检查结果
        if (!actionContext.ModelState.IsValid)
        {
            var errors = actionContext.ModelState
                .Where(e => e.Value!.Errors.Count > 0)
                .Select(e => $"{e.Key}: {string.Join(", ", e.Value!.Errors.Select(x => x.ErrorMessage))}")
                .ToList();

            throw new Exception("配置验证失败:\n" + string.Join("\n", errors));
        }
    }
}