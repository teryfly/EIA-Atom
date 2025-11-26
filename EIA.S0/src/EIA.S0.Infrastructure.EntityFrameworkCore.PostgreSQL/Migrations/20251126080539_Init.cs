using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EIA.S0.Infrastructure.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "doc_type",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, comment: "文档类型编码"),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false, comment: "名称"),
                    description = table.Column<string>(type: "text", nullable: true, comment: "描述"),
                    allowed_phases = table.Column<string>(type: "jsonb", nullable: false, comment: "允许的阶段编码集合"),
                    default_phase = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, comment: "默认阶段编码"),
                    category_id = table.Column<Guid>(type: "uuid", nullable: true, comment: "分类 Id"),
                    ai_draft_prompt_template_id = table.Column<Guid>(type: "uuid", nullable: true, comment: "默认草稿 PromptTemplate Id"),
                    metadata = table.Column<string>(type: "jsonb", nullable: true, comment: "元数据"),
                    custom_fields = table.Column<string>(type: "jsonb", nullable: true, comment: "自定义字段"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "创建时间"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "最后更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doc_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "IntegrationEventLog",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "id"),
                    EventName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "事件名称"),
                    EventTypeName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, comment: "事件类型名称"),
                    Identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, comment: "事件标识"),
                    Content = table.Column<string>(type: "text", nullable: false, comment: "事件内容"),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "创建时间"),
                    Sequence = table.Column<int>(type: "integer", nullable: false, comment: "同一时间的顺序"),
                    LastUpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "最后更新时间"),
                    TimesSent = table.Column<int>(type: "integer", nullable: false, comment: "发送次数"),
                    State = table.Column<int>(type: "integer", nullable: false, comment: "事件状态"),
                    Error = table.Column<string>(type: "text", nullable: true, comment: "错误信息")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationEventLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "phase_definition",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    phase_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, comment: "阶段编码"),
                    display_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false, comment: "显示名称"),
                    order = table.Column<int>(name: "\"order\"", type: "integer", nullable: false, comment: "顺序号"),
                    allowed_transitions = table.Column<string>(type: "jsonb", nullable: false, comment: "允许跳转到的阶段编码集合"),
                    properties = table.Column<string>(type: "jsonb", nullable: true, comment: "扩展属性"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "创建时间"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "最后更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_phase_definition", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_doctype_category",
                table: "doc_type",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "idx_doctype_code",
                table: "doc_type",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "idx_doctype_defaultphase",
                table: "doc_type",
                column: "default_phase");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationEventLog_CreateTime_State",
                table: "IntegrationEventLog",
                columns: new[] { "CreateTime", "State" });

            migrationBuilder.CreateIndex(
                name: "idx_phase_definition_order",
                table: "phase_definition",
                column: "\"order\"");

            migrationBuilder.CreateIndex(
                name: "ux_phase_definition_phase_code",
                table: "phase_definition",
                column: "phase_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "doc_type");

            migrationBuilder.DropTable(
                name: "IntegrationEventLog");

            migrationBuilder.DropTable(
                name: "phase_definition");
        }
    }
}
