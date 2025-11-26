
> 注意路径前是否有 `PathBase`（本项目默认 `/app`）。如果你在本地修改了 `PathBase` 或使用测试环境，请相应调整 URL。

---

## 5. 单元测试 / WebApi 测试使用的数据库

- **单元测试 (`.Application.Tests`, `.Infrastructure.Tests`)**：
  - 多数使用 **InMemoryDatabase**，不依赖真实 PostgreSQL。
- **WebApi 集成测试 (`EIA.S0.WebApi.Tests`)**：
  - 通过 `appsettings.UnitTest.json` 使用 InMemory 引擎：

    ```json
    "Database": {
      "Engine": "InMemory",
      "ConnectionString": "InMemory"
    }
    ```

  - 不会连到你本地的 PostgreSQL。

因此：

- `dotnet test` 在 CI 或本地执行时，不需要你手工创建数据库表。
- 只有在 **本地自己运行 WebApi 并连接真实 PostgreSQL** 时，才需要按上面的 Migration / SQL 方式初始化数据库。

---

## 6. 总结

- **是否 Code First？** 是的，本项目使用 EF Core Code First 配合 Migration。
- **如何初始化数据？**
  - 推荐：通过 Migration + `DbInitializer` 自动建表，再用 API 或脚本注入基础数据（PhaseDefinition、DocType）。
  - 也可以：直接执行 `migrations/*.sql` 手写脚本，然后照样使用 API 创建数据。
- **本地连接串** 已在 `src/EIA.S0.WebApi/appsettings.Development.json` 中配置，你可以根据自己的环境修改。