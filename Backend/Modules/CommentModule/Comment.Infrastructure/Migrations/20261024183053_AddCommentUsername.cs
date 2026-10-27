using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentUsername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                schema: "comment",
                table: "Comments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                schema: "comment",
                table: "Comments");
        }
    }
}
