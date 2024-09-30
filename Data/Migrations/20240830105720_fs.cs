using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce_C__ASP.NET.Data.Migrations
{
    /// <inheritdoc />
    public partial class fs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DbSet_Produto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Imagem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Disponivel = table.Column<bool>(type: "bit", nullable: false),
                    TipoProdutoId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbSet_Produto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbSet_Produto_DbSet_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "DbSet_Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DbSet_Produto_DbSet_TiposProduto_TipoProdutoId",
                        column: x => x.TipoProdutoId,
                        principalTable: "DbSet_TiposProduto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DbSet_Produto_TagId",
                table: "DbSet_Produto",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_DbSet_Produto_TipoProdutoId",
                table: "DbSet_Produto",
                column: "TipoProdutoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DbSet_Produto");
        }
    }
}
