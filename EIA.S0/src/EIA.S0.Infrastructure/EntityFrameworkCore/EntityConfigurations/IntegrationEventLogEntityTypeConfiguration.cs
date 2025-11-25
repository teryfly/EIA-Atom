using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EIA.S0.Domain.IntegrationEventLogs.Aggregates;

namespace EIA.S0.Infrastructure.EntityFrameworkCore.EntityConfigurations;

public class IntegrationEventLogEntityTypeConfiguration : IEntityTypeConfiguration<IntegrationEventLog>
{
    public void Configure(EntityTypeBuilder<IntegrationEventLog> builder)
    {
        builder.ToTable(nameof(IntegrationEventLog));
        builder.HasKey(k => k.Id);
        builder.Property(p => p.Id).HasMaxLength(50).HasComment("id");
        builder.Property(p => p.EventName).HasMaxLength(50).IsRequired().HasComment("事件名称");
        builder.Property(p => p.EventTypeName).HasMaxLength(255).IsRequired().HasComment("事件类型名称");
        builder.Property(p => p.Identifier).HasMaxLength(50).IsRequired(false).HasComment("事件标识");
        builder.Property(p => p.Content).IsRequired().HasComment("事件内容");
        builder.Property(p => p.CreateTime).IsRequired().HasComment("创建时间");
        builder.Property(p => p.Sequence).IsRequired().HasComment("同一时间的顺序");
        builder.Property(p => p.LastUpdateTime).IsRequired(false).HasComment("最后更新时间");
        builder.Property(p => p.TimesSent).IsRequired().HasComment("发送次数");
        builder.Property(p => p.State).IsRequired().HasComment("事件状态");
        builder.Property(p => p.Error).IsRequired(false).HasComment("错误信息");

        // 添加索引.
        builder.HasIndex(p => new { p.CreateTime, p.State });

        // 忽略DomainEvent
        builder.Ignore(b => b.DomainEvent);
    }
}
