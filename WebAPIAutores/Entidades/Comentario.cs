using Microsoft.AspNetCore.Identity;

namespace WebAPIAutores.Entidades
{
    public class Comentario
    {

        public int Id { get; set; }
        public string Contenido { get; set; }
        public int LibroId { get; set; }
//relacion uno a muchos, un libro con muchos comentrios
        public Libro Libro { get; set; }
        public string UsuarioId { get; set; }
        public IdentityUser Usuario { get; set; }
    }
}
