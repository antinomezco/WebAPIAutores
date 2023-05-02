namespace WebAPIAutores.Entidades
{
    public class AutorLibro
    {
        //relacion muchos a muchos, un autor puede escribir varios libros y un libro puede tener varios autores
        public int LibroId { get; set; }
        public int AutorId { get; set; }
        public int Orden { get; set; }
        public Libro Libro { get; set; }
        public Autor Autor { get; set; }

    }
}
