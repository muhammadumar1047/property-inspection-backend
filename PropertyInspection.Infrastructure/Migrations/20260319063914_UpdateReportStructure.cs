using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyInspection.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReportStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ReportMedias",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ReportItemConditions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "ReportItemConditions");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "ReportMedias",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
