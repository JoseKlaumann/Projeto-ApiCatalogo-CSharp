using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : Controller
    {
        private readonly AppDbContext _contex;

        public CategoriasController(AppDbContext contex)
        {
            _contex = contex;
        }

        [HttpGet("produto")]
        public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
        {
            try
            {
                // return _contex.Categorias.Include(p => p.Produtos).AsNoTracking().ToList();
                return _contex.Categorias.Include(p => p.Produtos).Where(c => c.CategoriaId <= 5).ToList();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            try
            {
                var categorias = _contex.Categorias.AsNoTracking().ToList();

                if (categorias is null)
                {
                    return NotFound("Categorias não encontradas!");
                }

                return categorias;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<Categoria> Get(int id)
        {
            try
            {
                var categoria = _contex.Categorias.FirstOrDefault(p => p.CategoriaId == id);
                if (categoria is null)
                {
                    return NotFound($"Categoria com id = {id} não encontrada!");
                }
                return categoria;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpPost]
        public ActionResult Post(Categoria categoria)
        {
            if (categoria is null)
            {
                return BadRequest("Dados inválidos!");
            }
            _contex.Categorias.Add(categoria);
            _contex.SaveChanges();
            return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId }, categoria);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Categoria categoria)
        {
            if (id != categoria.CategoriaId)
            {
                return BadRequest("Dados inválidos!");
            }

            _contex.Entry(categoria).State = EntityState.Modified;
            _contex.SaveChanges();

            return Ok(categoria);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var categoria = _contex.Categorias.FirstOrDefault(p => p.CategoriaId == id);

            if (categoria is null)
            {
                return NotFound($"Categoria com id = {id} não localizada!");
            }

            _contex.Categorias.Remove(categoria);
            _contex.SaveChanges();

            return Ok(categoria);
        }
    }
}
