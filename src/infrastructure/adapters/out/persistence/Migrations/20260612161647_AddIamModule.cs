using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiBasica.src.infrastructure.adapters.@out.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIamModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Scopes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scopes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleScopes",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ScopeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleScopes", x => new { x.RoleId, x.ScopeId });
                    table.ForeignKey(
                        name: "FK_RoleScopes_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleScopes_Scopes_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "Scopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserScopes",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ScopeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserScopes", x => new { x.UserId, x.ScopeId });
                    table.ForeignKey(
                        name: "FK_UserScopes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserScopes_Scopes_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "Scopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleScopes_ScopeId",
                table: "RoleScopes",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_Scopes_Name",
                table: "Scopes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserScopes_ScopeId",
                table: "UserScopes",
                column: "ScopeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleScopes");

            migrationBuilder.DropTable(
                name: "UserScopes");

            migrationBuilder.DropTable(
                name: "Scopes");
        }
    }
}
