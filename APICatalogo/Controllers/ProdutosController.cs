using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext _contex;

        public ProdutosController(AppDbContext contex)
        {
            _contex = contex;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            try
            {
                var produtos = _contex.Produtos.Take(10).ToList();

                if (produtos is null)
                {
                    return NotFound("Produtos não encontrados!");
                }

                return produtos;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                   "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpGet("{id:int}", Name = "ObterProduto")]
        public ActionResult<Produto> Get(int id)
        {
            try
            {
                var produto = _contex.Produtos.FirstOrDefault(p => p.ProdutoId == id);
                if (produto is null)
                {
                    return NotFound($"Produto com id = {id} não encontrado!");
                }
                return produto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                                  "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpPost]
        public ActionResult Post(Produto produto)
        {
            if (produto is null)
            {
                return BadRequest("Dados inválidos!");
            }
            _contex.Produtos.Add(produto);
            _contex.SaveChanges();
            return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId }, produto);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            if (id != produto.ProdutoId)
            {
                return BadRequest("Dados inválidos!");
            }

            _contex.Entry(produto).State = EntityState.Modified;
            _contex.SaveChanges();

            return Ok(produto);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var produto = _contex.Produtos.FirstOrDefault(p => p.ProdutoId == id);

            if (produto is null)
            {
                return NotFound($"Produto com id = {id} não localizado!");
            }

            _contex.Produtos.Remove(produto);
            _contex.SaveChanges();

            return Ok(produto);
        }
    }
}
