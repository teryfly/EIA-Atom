# 迁移运行命令
本示例使用 powershell + dotnet ef tool 执行；
1. 运行迁移命令前需要先build项目
2. 迁移命令要在启动项目的文件夹运行，指定迁移放置的项目，输出路径为迁移项目的路径

```shell
# Add Migration
# 在项目 WebApi 中执行
dotnet ef migrations add {迁移名称(Initial)} --context EiaS0dbContext --project ../EIA.S0.Infrastructure.EntityFrameworkCore.PostgreSQL/EIA.S0.Infrastructure.EntityFrameworkCore.PostgreSQL.csproj -o Migrations
```

```shell
# Remove Migration 
dotnet ef migrations remove --context EiaS0dbContext --project ../EIA.S0.Infrastructure.EntityFrameworkCore.PostgreSQL/EIA.S0.Infrastructure.EntityFrameworkCore.PostgreSQL.csproj
```