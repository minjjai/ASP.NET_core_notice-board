using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoticeBoard.Migrations
{
    /// <inheritdoc />
    public partial class addcategoryviewmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FixedCategories_Post_PostId",
                table: "FixedCategories");

            migrationBuilder.DropIndex(
                name: "IX_FixedCategories_PostId",
                table: "FixedCategories");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "FixedCategories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PostId",
                table: "FixedCategories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FixedCategories_PostId",
                table: "FixedCategories",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_FixedCategories_Post_PostId",
                table: "FixedCategories",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "PostId");
        }
    }
}
