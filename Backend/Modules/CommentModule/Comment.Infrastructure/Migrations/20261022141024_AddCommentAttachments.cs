using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_CommentId",
                schema: "comment",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CommentId",
                schema: "comment",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "CommentId",
                schema: "comment",
                table: "Comments");

            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                schema: "comment",
                table: "Comments",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                schema: "comment",
                table: "Comments",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CommentAttachments",
                schema: "comment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommentId = table.Column<Guid>(type: "uuid", nullable: false),
                    BlobName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentAttachments_Comments_CommentId",
                        column: x => x.CommentId,
                        principalSchema: "comment",
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CreatedAt",
                schema: "comment",
                table: "Comments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentId",
                schema: "comment",
                table: "Comments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_Rating",
                schema: "comment",
                table: "Comments",
                column: "Rating");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                schema: "comment",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentAttachments_CommentId",
                schema: "comment",
                table: "CommentAttachments",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                schema: "comment",
                table: "Comments",
                column: "ParentCommentId",
                principalSchema: "comment",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                schema: "comment",
                table: "Comments");

            migrationBuilder.DropTable(
                name: "CommentAttachments",
                schema: "comment");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CreatedAt",
                schema: "comment",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ParentCommentId",
                schema: "comment",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_Rating",
                schema: "comment",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_UserId",
                schema: "comment",
                table: "Comments");

            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                schema: "comment",
                table: "Comments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                schema: "comment",
                table: "Comments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CommentId",
                schema: "comment",
                table: "Comments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentId",
                schema: "comment",
                table: "Comments",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_CommentId",
                schema: "comment",
                table: "Comments",
                column: "CommentId",
                principalSchema: "comment",
                principalTable: "Comments",
                principalColumn: "Id");
        }
    }
}
