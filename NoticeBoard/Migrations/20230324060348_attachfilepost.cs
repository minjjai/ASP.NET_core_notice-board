using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoticeBoard.Migrations
{
    /// <inheritdoc />
    public partial class attachfilepost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AttachFile_PostId",
                table: "AttachFile",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttachFile_Post_PostId",
                table: "AttachFile",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "PostId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttachFile_Post_PostId",
                table: "AttachFile");

            migrationBuilder.DropIndex(
                name: "IX_AttachFile_PostId",
                table: "AttachFile");
        }
    }
}
