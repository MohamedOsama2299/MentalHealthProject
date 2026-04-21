using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBTClinic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixAdminReportRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminReports_AspNetUsers_AdminId",
                table: "AdminReports");

            migrationBuilder.AlterColumn<int>(
                name: "AdminId",
                table: "AdminReports",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminReports_Admins_AdminId",
                table: "AdminReports",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminReports_Admins_AdminId",
                table: "AdminReports");

            migrationBuilder.AlterColumn<string>(
                name: "AdminId",
                table: "AdminReports",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminReports_AspNetUsers_AdminId",
                table: "AdminReports",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
