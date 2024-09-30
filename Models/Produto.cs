using E_Commerce_C__ASP.NET.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace E_Commerce_C__ASP.NET.Models
{ 
    public class Produto
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public double Preco { get; set; }

        public string? Imagem { get; set; }

        [Display(Name = "Cor do produto")]
        public string Cor { get; set; }

        [Display(Name = "Disponibilidade")]
        [Required]
        public bool Disponivel { get; set; }

        [Display(Name = "Tipo de Produto")]
        [Required]
        public int TipoProdutoId { get; set; }

        public virtual TiposProduto? TipoProduto { get; set; }

        [Display(Name = "Tag do Produto")]
        [Required]
        public int TagId { get; set; }

        public virtual SpecialTag? Tag { get; set; }
    }
}