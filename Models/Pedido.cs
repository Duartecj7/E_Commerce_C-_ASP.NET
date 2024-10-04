namespace E_Commerce_C__ASP.NET.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public string NumPedido { get; set; }
        public string Nome { get; set; }
        public string NumTel { get; set; }
        public string Email { get; set; }
        public string Morada { get; set; }
        public string codigoPost { get; set; }
        public string Localidade { get; set; }
        public string Status { get; set; } = "Pendente";
        public DateTime DataPedido { get; set; }
        public virtual ICollection<ItemPedido> ItensPedido { get; set; } = new List<ItemPedido>(); 

    }
}
