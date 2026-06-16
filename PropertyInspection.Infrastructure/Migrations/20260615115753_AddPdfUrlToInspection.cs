using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyInspection.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPdfUrlToInspection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PdfUrl",
                table: "Inspections",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdfUrl",
                table: "Inspections");
        }
    }
}
