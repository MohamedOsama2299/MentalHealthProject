using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBTClinic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RestoreQuizColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Quizzes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Quizzes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Quizzes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Quizzes");
        }
    }
}
