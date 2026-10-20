using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace User.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_Roles_RoleId",
                schema: "users",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_Users_UserId",
                schema: "users",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_Users_UserId",
                schema: "users",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_Users_UserId",
                schema: "users",
                table: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserTokens",
                schema: "users",
                table: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserLogins",
                schema: "users",
                table: "AspNetUserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserClaims",
                schema: "users",
                table: "AspNetUserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoleClaims",
                schema: "users",
                table: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                schema: "users",
                newName: "UserTokens",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                schema: "users",
                newName: "UserLogins",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                schema: "users",
                newName: "UserClaims",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                schema: "users",
                newName: "RoleClaims",
                newSchema: "users");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "users",
                table: "UserLogins",
                newName: "IX_UserLogins_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "users",
                table: "UserClaims",
                newName: "IX_UserClaims_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "users",
                table: "RoleClaims",
                newName: "IX_RoleClaims_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTokens",
                schema: "users",
                table: "UserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLogins",
                schema: "users",
                table: "UserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserClaims",
                schema: "users",
                table: "UserClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleClaims",
                schema: "users",
                table: "RoleClaims",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleClaims_Roles_RoleId",
                schema: "users",
                table: "RoleClaims",
                column: "RoleId",
                principalSchema: "users",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaims_Users_UserId",
                schema: "users",
                table: "UserClaims",
                column: "UserId",
                principalSchema: "users",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogins_Users_UserId",
                schema: "users",
                table: "UserLogins",
                column: "UserId",
                principalSchema: "users",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTokens_Users_UserId",
                schema: "users",
                table: "UserTokens",
                column: "UserId",
                principalSchema: "users",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleClaims_Roles_RoleId",
                schema: "users",
                table: "RoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClaims_Users_UserId",
                schema: "users",
                table: "UserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLogins_Users_UserId",
                schema: "users",
                table: "UserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTokens_Users_UserId",
                schema: "users",
                table: "UserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTokens",
                schema: "users",
                table: "UserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLogins",
                schema: "users",
                table: "UserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserClaims",
                schema: "users",
                table: "UserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleClaims",
                schema: "users",
                table: "RoleClaims");

            migrationBuilder.RenameTable(
                name: "UserTokens",
                schema: "users",
                newName: "AspNetUserTokens",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "UserLogins",
                schema: "users",
                newName: "AspNetUserLogins",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "UserClaims",
                schema: "users",
                newName: "AspNetUserClaims",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "RoleClaims",
                schema: "users",
                newName: "AspNetRoleClaims",
                newSchema: "users");

            migrationBuilder.RenameIndex(
                name: "IX_UserLogins_UserId",
                schema: "users",
                table: "AspNetUserLogins",
                newName: "IX_AspNetUserLogins_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserClaims_UserId",
                schema: "users",
                table: "AspNetUserClaims",
                newName: "IX_AspNetUserClaims_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RoleClaims_RoleId",
                schema: "users",
                table: "AspNetRoleClaims",
                newName: "IX_AspNetRoleClaims_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserTokens",
                schema: "users",
                table: "AspNetUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserLogins",
                schema: "users",
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserClaims",
                schema: "users",
                table: "AspNetUserClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoleClaims",
                schema: "users",
                table: "AspNetRoleClaims",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_Roles_RoleId",
                schema: "users",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalSchema: "users",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_Users_UserId",
                schema: "users",
                table: "AspNetUserClaims",
                column: "UserId",
                principalSchema: "users",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_Users_UserId",
                schema: "users",
                table: "AspNetUserLogins",
                column: "UserId",
                principalSchema: "users",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_Users_UserId",
                schema: "users",
                table: "AspNetUserTokens",
                column: "UserId",
                principalSchema: "users",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
