using System.ComponentModel.DataAnnotations;
using WebAPIAutores.Validaciones;

namespace WebAPIAutores.Entidades
{
    public class Autor
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength: 120, ErrorMessage = "Demasiados caracteres, pon menos de 120")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }

        public List<AutorLibro> AutoresLibros { get; set; }

    }
}
