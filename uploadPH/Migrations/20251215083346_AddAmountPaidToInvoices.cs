using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharmacy.Migrations
{
    /// <inheritdoc />
    public partial class AddAmountPaidToInvoices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "admins",
                columns: table => new
                {
                    admin_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__admins__43AA4141A585EE2F", x => x.admin_id);
                });

            migrationBuilder.CreateTable(
                name: "drugs",
                columns: table => new
                {
                    drug_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    selling_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    purchasing_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    barcode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    image_url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    description_before_use = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description_how_to_use = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description_side_effects = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    requires_prescription = table.Column<bool>(type: "bit", nullable: false),
                    drug_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    manufacturer = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    expiration_date = table.Column<DateOnly>(type: "date", nullable: true),
                    shelf_amount = table.Column<int>(type: "int", nullable: false),
                    stored_amount = table.Column<int>(type: "int", nullable: false),
                    low_amount = table.Column<int>(type: "int", nullable: false, defaultValue: 10),
                    sub_amount_quantity = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__drugs__73F2330CA7F8E910", x => x.drug_id);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    tag_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tags__4296A2B656706952", x => x.tag_id);
                });

            migrationBuilder.CreateTable(
                name: "admin_logs",
                columns: table => new
                {
                    log_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    admin_id = table.Column<int>(type: "int", nullable: false),
                    action_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    action_time = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__admin_lo__9E2397E0E51EA211", x => x.log_id);
                    table.ForeignKey(
                        name: "fk_admin_logs_admin",
                        column: x => x.admin_id,
                        principalTable: "admins",
                        principalColumn: "admin_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "invoices",
                columns: table => new
                {
                    invoice_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    admin_id = table.Column<int>(type: "int", nullable: true),
                    invoice_time = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    total_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tax_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    discount_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    change_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__invoices__F58DFD4921BD016E", x => x.invoice_id);
                    table.ForeignKey(
                        name: "fk_invoice_admin",
                        column: x => x.admin_id,
                        principalTable: "admins",
                        principalColumn: "admin_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "drug_tags",
                columns: table => new
                {
                    drug_id = table.Column<int>(type: "int", nullable: false),
                    tag_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__drug_tag__07DB5927FFBD29C7", x => new { x.drug_id, x.tag_id });
                    table.ForeignKey(
                        name: "fk_dt_drug",
                        column: x => x.drug_id,
                        principalTable: "drugs",
                        principalColumn: "drug_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dt_tag",
                        column: x => x.tag_id,
                        principalTable: "tags",
                        principalColumn: "tag_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "invoice_items",
                columns: table => new
                {
                    item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    invoice_id = table.Column<int>(type: "int", nullable: false),
                    drug_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__invoice___52020FDDA3592531", x => x.item_id);
                    table.ForeignKey(
                        name: "fk_ii_drug",
                        column: x => x.drug_id,
                        principalTable: "drugs",
                        principalColumn: "drug_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ii_invoice",
                        column: x => x.invoice_id,
                        principalTable: "invoices",
                        principalColumn: "invoice_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_admin_logs_admin_id",
                table: "admin_logs",
                column: "admin_id");

            migrationBuilder.CreateIndex(
                name: "UQ__admins__AB6E6164FF3CA0A0",
                table: "admins",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__admins__F3DBC57214218078",
                table: "admins",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_drug_tags_tag_id",
                table: "drug_tags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "UQ__drugs__C16E36F80ED4F771",
                table: "drugs",
                column: "barcode",
                unique: true,
                filter: "[barcode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_items_drug_id",
                table: "invoice_items",
                column: "drug_id");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_items_invoice_id",
                table: "invoice_items",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "IX_invoices_admin_id",
                table: "invoices",
                column: "admin_id");

            migrationBuilder.CreateIndex(
                name: "UQ__tags__72E12F1B6BE40C81",
                table: "tags",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin_logs");

            migrationBuilder.DropTable(
                name: "drug_tags");

            migrationBuilder.DropTable(
                name: "invoice_items");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "drugs");

            migrationBuilder.DropTable(
                name: "invoices");

            migrationBuilder.DropTable(
                name: "admins");
        }
    }
}
