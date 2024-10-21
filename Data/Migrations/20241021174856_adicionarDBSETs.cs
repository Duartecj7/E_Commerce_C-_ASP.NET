using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce_C__ASP.NET.Data.Migrations
{
    /// <inheritdoc />
    public partial class adicionarDBSETs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DbSet_Pedido",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumPedido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumTel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Morada = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    codigoPost = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Localidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataPedido = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbSet_Pedido", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DbSet_ItemPedido",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    Preco = table.Column<double>(type: "float", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    PedidoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbSet_ItemPedido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbSet_ItemPedido_DbSet_Pedido_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "DbSet_Pedido",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DbSet_ItemPedido_DbSet_Produto_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "DbSet_Produto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DbSet_ItemPedido_PedidoId",
                table: "DbSet_ItemPedido",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_DbSet_ItemPedido_ProdutoId",
                table: "DbSet_ItemPedido",
                column: "ProdutoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DbSet_ItemPedido");

            migrationBuilder.DropTable(
                name: "DbSet_Pedido");
        }
    }
}
