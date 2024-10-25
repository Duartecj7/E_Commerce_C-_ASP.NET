namespace E_Commerce_C__ASP.NET.Models
{
    public class Pagamento
    {
        public int Id { get; set; } 
        public string ModoPagamento { get; set; } // Ex: "Cartão", "MBWAY", "Referência"
        public string TokenPagamento { get; set; } 
        public string ReferenciaPagamento { get; set; } 
        public string Status { get; set; } = "Pagamento Pendente"; 
        public DateTime? DataPagamento { get; set; } 
        public string Detalhes { get; set; } 
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; } 
    }
}
