using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace marketplace_AdIns_BE.Migrations
{
    public partial class ScopeVoucherCodeToStore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vouchers_Code",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_StoreId",
                table: "Vouchers");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_StoreId_Code",
                table: "Vouchers",
                columns: new[] { "StoreId", "Code" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vouchers_StoreId_Code",
                table: "Vouchers");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_Code",
                table: "Vouchers",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_StoreId",
                table: "Vouchers",
                column: "StoreId");
        }
    }
}
