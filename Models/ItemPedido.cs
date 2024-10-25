namespace E_Commerce_C__ASP.NET.Models
{
    public class ItemPedido
    {
        public int Id { get; set; }  

        public int ProdutoId { get; set; }  

        public double Preco { get; set; }  

        public int Quantidade { get; set; }  

        public virtual Produto Produto { get; set; }
        public int PedidoId { get; set; }
        public virtual Pedido Pedido { get; set; }
    }
}
