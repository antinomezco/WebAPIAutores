using System.ComponentModel.DataAnnotations;
using WebAPIAutores.Validaciones;

namespace WebAPIAutores.DTOs
{
    public class LibroDTO
    {
        public int? Id { get; set; }
        public string? Titulo { get; set; }
        public DateTime FechaPublicacion { get; set; }

        //relacion uno a muchos, un libro con muchos comentrios


        //public List<ComentarioDTO> Comentarios { get; set; }
    }
}
