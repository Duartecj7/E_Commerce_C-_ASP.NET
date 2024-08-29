using Microsoft.AspNetCore.Antiforgery;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce_C__ASP.NET.Models
{
    public class TiposProduto
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="O Nome do tipo de produto não pode ser nulo")]
        [Display(Name ="Nome do produto")]
        [MinLength(4,ErrorMessage ="O nome do produto deve ter no mínimo 5 caracteres")]
        [MaxLength(64, ErrorMessage = "O nome do produto deve ter no máximo 64 caracteres")]
        
        public string TipoProduto { get; set; }
    }
}
