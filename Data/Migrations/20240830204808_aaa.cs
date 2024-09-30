using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce_C__ASP.NET.Data.Migrations
{
    /// <inheritdoc />
    public partial class aaa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DbSet_Produto_DbSet_Tags_TagNomeId",
                table: "DbSet_Produto");

            migrationBuilder.DropIndex(
                name: "IX_DbSet_Produto_TagNomeId",
                table: "DbSet_Produto");

            migrationBuilder.DropColumn(
                name: "TagNomeId",
                table: "DbSet_Produto");

            migrationBuilder.CreateIndex(
                name: "IX_DbSet_Produto_TagId",
                table: "DbSet_Produto",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_DbSet_Produto_DbSet_Tags_TagId",
                table: "DbSet_Produto",
                column: "TagId",
                principalTable: "DbSet_Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DbSet_Produto_DbSet_Tags_TagId",
                table: "DbSet_Produto");

            migrationBuilder.DropIndex(
                name: "IX_DbSet_Produto_TagId",
                table: "DbSet_Produto");

            migrationBuilder.AddColumn<int>(
                name: "TagNomeId",
                table: "DbSet_Produto",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DbSet_Produto_TagNomeId",
                table: "DbSet_Produto",
                column: "TagNomeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DbSet_Produto_DbSet_Tags_TagNomeId",
                table: "DbSet_Produto",
                column: "TagNomeId",
                principalTable: "DbSet_Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
