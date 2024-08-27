using Microsoft.AspNetCore.Antiforgery;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce_C__ASP.NET.Models
{
    public class TiposProduto
    {
        public int Id { get; set; }
        [Required]
        [Display(Name ="Nome do produto")]
        public string TipoProduto { get; set; }
    }
}
