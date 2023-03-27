using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoticeBoard.Migrations
{
    /// <inheritdoc />
    public partial class attachFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttachFile_Post_PostId",
                table: "AttachFile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttachFile",
                table: "AttachFile");

            migrationBuilder.RenameTable(
                name: "AttachFile",
                newName: "AttachFiles");

            migrationBuilder.RenameIndex(
                name: "IX_AttachFile_PostId",
                table: "AttachFiles",
                newName: "IX_AttachFiles_PostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttachFiles",
                table: "AttachFiles",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttachFiles_Post_PostId",
                table: "AttachFiles",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "PostId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttachFiles_Post_PostId",
                table: "AttachFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttachFiles",
                table: "AttachFiles");

            migrationBuilder.RenameTable(
                name: "AttachFiles",
                newName: "AttachFile");

            migrationBuilder.RenameIndex(
                name: "IX_AttachFiles_PostId",
                table: "AttachFile",
                newName: "IX_AttachFile_PostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttachFile",
                table: "AttachFile",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttachFile_Post_PostId",
                table: "AttachFile",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "PostId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
