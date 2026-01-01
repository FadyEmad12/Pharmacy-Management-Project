using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharmacy.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteBehaviorToRestrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_admin_logs_admin",
                table: "admin_logs");

            migrationBuilder.DropForeignKey(
                name: "fk_dt_drug",
                table: "drug_tags");

            migrationBuilder.DropForeignKey(
                name: "fk_dt_tag",
                table: "drug_tags");

            migrationBuilder.DropForeignKey(
                name: "fk_ii_drug",
                table: "invoice_items");

            migrationBuilder.DropForeignKey(
                name: "fk_ii_invoice",
                table: "invoice_items");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_admin",
                table: "invoices");

            migrationBuilder.AddForeignKey(
                name: "fk_admin_logs_admin",
                table: "admin_logs",
                column: "admin_id",
                principalTable: "admins",
                principalColumn: "admin_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_dt_drug",
                table: "drug_tags",
                column: "drug_id",
                principalTable: "drugs",
                principalColumn: "drug_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_dt_tag",
                table: "drug_tags",
                column: "tag_id",
                principalTable: "tags",
                principalColumn: "tag_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ii_drug",
                table: "invoice_items",
                column: "drug_id",
                principalTable: "drugs",
                principalColumn: "drug_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ii_invoice",
                table: "invoice_items",
                column: "invoice_id",
                principalTable: "invoices",
                principalColumn: "invoice_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_admin",
                table: "invoices",
                column: "admin_id",
                principalTable: "admins",
                principalColumn: "admin_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_admin_logs_admin",
                table: "admin_logs");

            migrationBuilder.DropForeignKey(
                name: "fk_dt_drug",
                table: "drug_tags");

            migrationBuilder.DropForeignKey(
                name: "fk_dt_tag",
                table: "drug_tags");

            migrationBuilder.DropForeignKey(
                name: "fk_ii_drug",
                table: "invoice_items");

            migrationBuilder.DropForeignKey(
                name: "fk_ii_invoice",
                table: "invoice_items");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_admin",
                table: "invoices");

            migrationBuilder.AddForeignKey(
                name: "fk_admin_logs_admin",
                table: "admin_logs",
                column: "admin_id",
                principalTable: "admins",
                principalColumn: "admin_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_dt_drug",
                table: "drug_tags",
                column: "drug_id",
                principalTable: "drugs",
                principalColumn: "drug_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_dt_tag",
                table: "drug_tags",
                column: "tag_id",
                principalTable: "tags",
                principalColumn: "tag_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_ii_drug",
                table: "invoice_items",
                column: "drug_id",
                principalTable: "drugs",
                principalColumn: "drug_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_ii_invoice",
                table: "invoice_items",
                column: "invoice_id",
                principalTable: "invoices",
                principalColumn: "invoice_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_admin",
                table: "invoices",
                column: "admin_id",
                principalTable: "admins",
                principalColumn: "admin_id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
