using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProiectII.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminCommentToAdoption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "FoxId",
                table: "Reports",
                type: "int unsigned",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminComment",
                table: "Adoptions",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_FoxId",
                table: "Reports",
                column: "FoxId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Foxes_FoxId",
                table: "Reports",
                column: "FoxId",
                principalTable: "Foxes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Foxes_FoxId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_FoxId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "FoxId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "AdminComment",
                table: "Adoptions");
        }
    }
}
