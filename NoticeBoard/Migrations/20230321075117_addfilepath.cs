using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoticeBoard.Migrations
{
    /// <inheritdoc />
    public partial class addfilepath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "Post",
                newName: "filePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "filePath",
                table: "Post",
                newName: "ImagePath");
        }
    }
}
