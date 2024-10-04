using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce_C__ASP.NET.Data.Migrations
{
    /// <inheritdoc />
    public partial class Produto_Adicionar_Quantidade_e_Stock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantidade",
                table: "DbSet_Produto",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "DbSet_Produto",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantidade",
                table: "DbSet_Produto");

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "DbSet_Produto");
        }
    }
}
