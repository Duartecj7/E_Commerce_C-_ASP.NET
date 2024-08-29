using System.ComponentModel.DataAnnotations;

namespace E_Commerce_C__ASP.NET.Models
{
    public class SpecialTag
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O nome da Tag de produto não pode ser nulo")]
        [Display(Name = "Nome do produto")]
        [MinLength(4, ErrorMessage = "O nome da Tag deve ter no mínimo 5 caracteres")]
        [MaxLength(64, ErrorMessage = "O nome da Tag deve ter no máximo 64 caracteres")]

        public string TagNome { get; set; }

    }
}
