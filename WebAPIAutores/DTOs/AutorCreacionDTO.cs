using System.ComponentModel.DataAnnotations;
using WebAPIAutores.Validaciones;

namespace WebAPIAutores.DTOs
{
    public class AutorCreacionDTO
    {
        [Required]
        [StringLength(maximumLength: 120, ErrorMessage = "Demasiados caracteres, pon menos de 120")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
    }
}
