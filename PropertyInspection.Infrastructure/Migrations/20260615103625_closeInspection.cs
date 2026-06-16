using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyInspection.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class closeInspection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "InspectionCloseDate",
                table: "Inspections",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InspectionCompletedDate",
                table: "Inspections",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SignatureDate",
                table: "Inspections",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignatureImageUrl",
                table: "Inspections",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InspectionCloseDate",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "InspectionCompletedDate",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "SignatureDate",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "SignatureImageUrl",
                table: "Inspections");
        }
    }
}
