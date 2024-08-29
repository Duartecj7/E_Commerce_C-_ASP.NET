using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_C__ASP.NET.Models
{
    public class Produto
    {
        public int Id { get; set; }
        
        [Required]
        public string  Nome { get; set; }
        
        [Required]
        public decimal Preco { get; set; }
        
        public string Imagem { get; set; }
        
        [Display(Name = "Cor do produto")]
        public string Cor { get; set; }
        
        [Required]
        [Display(Name = "Disponibilidade")]
        public bool Disponivel { get; set; }

        [Required]
        [Display(Name = "Tipo de Produto")]
        public int TipoProdutoId { get; set; }
       
        [ForeignKey("TipoProdutoId")]
        public TiposProduto TipoProduto { get; set; }
        
        [Required]
        [Display(Name = "Tag do Produto")]
        public int TagId { get; set; }
        
        [ForeignKey("TagId")]
        public SpecialTag TagNome { get; set; }
    }
}
