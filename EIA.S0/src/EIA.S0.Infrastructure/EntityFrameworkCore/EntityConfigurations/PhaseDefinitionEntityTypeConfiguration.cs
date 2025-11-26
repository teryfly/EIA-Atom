using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EIA.S0.Domain.Governance.Entities;

namespace EIA.S0.Infrastructure.EntityFrameworkCore.EntityConfigurations;

/// <summary>
/// PhaseDefinition EF 映射.
/// </summary>
public class PhaseDefinitionEntityTypeConfiguration : IEntityTypeConfiguration<PhaseDefinition>
{
    public void Configure(EntityTypeBuilder<PhaseDefinition> builder)
    {
        builder.ToTable("phase_definition");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(x => x.PhaseCode)
            .HasColumnName("phase_code")
            .HasMaxLength(64)
            .IsRequired()
            .HasComment("阶段编码");

        builder.Property(x => x.DisplayName)
            .HasColumnName("display_name")
            .HasMaxLength(128)
            .IsRequired()
            .HasComment("显示名称");

        builder.Property(x => x.Order)
            .HasColumnName("order")
            .IsRequired()
            .HasComment("顺序号");

        builder.Property<List<string>>(nameof(PhaseDefinition.AllowedTransitionPhaseCodes))
            .HasField(nameof(PhaseDefinition.AllowedTransitionPhaseCodes))
            .HasColumnName("allowed_transitions")
            .HasColumnType("jsonb")
            .IsRequired()
            .HasComment("允许跳转到的阶段编码集合");

        builder.Property(x => x.PropertiesJson)
            .HasColumnName("properties")
            .HasColumnType("jsonb")
            .IsRequired(false)
            .HasComment("扩展属性");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasComment("创建时间");

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired()
            .HasComment("最后更新时间");

        builder.HasIndex(x => x.PhaseCode)
            .IsUnique()
            .HasDatabaseName("ux_phase_definition_phase_code");

        builder.HasIndex(x => x.Order)
            .HasDatabaseName("idx_phase_definition_order");
    }
}