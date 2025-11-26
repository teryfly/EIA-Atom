using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EIA.S0.Domain.Governance.Entities;

namespace EIA.S0.Infrastructure.EntityFrameworkCore.EntityConfigurations;

/// <summary>
/// DocType EF 映射.
/// </summary>
public class DocTypeEntityTypeConfiguration : IEntityTypeConfiguration<DocType>
{
    public void Configure(EntityTypeBuilder<DocType> builder)
    {
        builder.ToTable("doc_type");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(x => x.Code)
            .HasColumnName("code")
            .HasMaxLength(64)
            .IsRequired()
            .HasComment("文档类型编码");

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(128)
            .IsRequired()
            .HasComment("名称");

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .IsRequired(false)
            .HasComment("描述");

        // 将 AllowedPhaseCodes 映射到 allowed_phases jsonb 列
        builder.Property(x => x.AllowedPhaseCodes)
            .HasColumnName("allowed_phases")
            .HasColumnType("jsonb")
            .IsRequired()
            .HasComment("允许的阶段编码集合");

        builder.Property(x => x.DefaultPhaseCode)
            .HasColumnName("default_phase")
            .HasMaxLength(64)
            .IsRequired()
            .HasComment("默认阶段编码");

        builder.Property(x => x.CategoryId)
            .HasColumnName("category_id")
            .IsRequired(false)
            .HasComment("分类 Id");

        builder.Property(x => x.AiDraftPromptTemplateId)
            .HasColumnName("ai_draft_prompt_template_id")
            .IsRequired(false)
            .HasComment("默认草稿 PromptTemplate Id");

        builder.Property(x => x.MetadataJson)
            .HasColumnName("metadata")
            .HasColumnType("jsonb")
            .IsRequired(false)
            .HasComment("元数据");

        builder.Property(x => x.CustomFieldsJson)
            .HasColumnName("custom_fields")
            .HasColumnType("jsonb")
            .IsRequired(false)
            .HasComment("自定义字段");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasComment("创建时间");

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired()
            .HasComment("最后更新时间");

        builder.HasIndex(x => x.Code).HasDatabaseName("idx_doctype_code");
        builder.HasIndex(x => x.CategoryId).HasDatabaseName("idx_doctype_category");
        builder.HasIndex(x => x.DefaultPhaseCode).HasDatabaseName("idx_doctype_defaultphase");
    }
}