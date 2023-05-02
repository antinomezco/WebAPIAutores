using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entidades;

namespace WebAPIAutores.Controllers
{
    [Route("api/libros")]
    [ApiController]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper;
        }

        [HttpGet("{id:int}", Name = "ObtenerLibro")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {
            var libro = await _context.Libros
                .Include(libroDB => libroDB.AutoresLibros)
                .ThenInclude(autorLibrosDB => autorLibrosDB.Autor)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (libro == null)
                return NotFound();

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x=> x.Orden).ToList();

            return _mapper.Map<LibroDTOConAutores>(libro);
        }

        [HttpPost]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            if (libroCreacionDTO.AutoresId == null)
                return BadRequest("no se puede crear libro sin autores");

            var autoresIds = await _context.Autores
                .Where(autorBD => libroCreacionDTO.AutoresId.Contains(autorBD.Id))
                .Select(x=>x.Id).ToListAsync();

            if(libroCreacionDTO.AutoresId.Count != autoresIds.Count)
                return BadRequest("no existe uno de los autores enviados");
            

            var libro = _mapper.Map<Libro>(libroCreacionDTO);

            AsignarNombresAutores(libro);

            _context.Add(libro);
            await _context.SaveChangesAsync();

            var libroDTO = _mapper.Map<LibroDTO>(libro);

            return CreatedAtRoute("ObtenerLibro", new { id = libro.Id }, libroDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libroDB = await _context.Libros
                .Include(x=>x.AutoresLibros)
                .FirstOrDefaultAsync(x=>x.Id== id);

            if (libroDB == null)
                return NotFound();

            libroDB = _mapper.Map(libroCreacionDTO, libroDB);

            AsignarNombresAutores(libroDB);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO> patchDocument)
        {
            if (patchDocument == null)
                return BadRequest();

            var libroDB = await _context.Libros.FirstOrDefaultAsync(x=>x.Id== id);

            if (libroDB == null)
                return NotFound();

            var libroDTO = _mapper.Map<LibroPatchDTO>(libroDB);

            patchDocument.ApplyTo(libroDTO, ModelState);

            var esValido = TryValidateModel(libroDTO);

            if (!esValido)
                return BadRequest(ModelState);

            _mapper.Map(libroDTO, libroDB);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private void AsignarNombresAutores(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                for (int i = 1; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }
    }
}
