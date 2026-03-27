using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyInspection.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBillingPlanInAgency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BillingPlanId",
                table: "Agencies",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agencies_BillingPlanId",
                table: "Agencies",
                column: "BillingPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agencies_Billings_BillingPlanId",
                table: "Agencies",
                column: "BillingPlanId",
                principalTable: "Billings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agencies_Billings_BillingPlanId",
                table: "Agencies");

            migrationBuilder.DropIndex(
                name: "IX_Agencies_BillingPlanId",
                table: "Agencies");

            migrationBuilder.DropColumn(
                name: "BillingPlanId",
                table: "Agencies");
        }
    }
}
