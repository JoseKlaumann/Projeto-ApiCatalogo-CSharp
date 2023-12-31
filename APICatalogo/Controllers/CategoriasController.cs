﻿using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CategoriasController : Controller
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public CategoriasController(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }

        [HttpGet("produto")]
        public ActionResult<IEnumerable<CategoriaDTO>> GetCategoriasProdutos()
        {
            try
            {
                var categorias = _uof.CategoriaRepository.GetCategoriaProdutos().ToList();
                var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);

                return categoriasDto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<CategoriaDTO>> Get()
        {
            try
            {
                var categorias = _uof.CategoriaRepository.Get().ToList();

                if (categorias is null)
                {
                    return NotFound("Categorias não encontradas!");
                }
                var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);

                // throw new Exception();
                return categoriasDto;
            }
            catch (Exception)
            {
                // return BadRequest();
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpGet("paginacao")]
        public ActionResult<IEnumerable<CategoriaDTO>> GetPaginacao(int pag=1, int reg=5)
        {
            try
            {
                if(reg > 10)
                {
                    reg = 5;
                }

                var categorias = _uof.CategoriaRepository.LocalizaPagina<Categoria>(pag, reg).ToList();

                var totalDeRegistros = _uof.CategoriaRepository.GetTotalRegistros();
                var numeroPaginas = ((int) Math.Ceiling((double) totalDeRegistros / reg));

                Response.Headers["X-Total-Registros"] = totalDeRegistros.ToString();
                Response.Headers["X-Numero-Paginas"] = numeroPaginas.ToString();

                if (categorias is null)
                {
                    return NotFound("Categorias não encontradas!");
                }
                var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);

                return categoriasDto;
            }
            catch (Exception)
            {
                // return BadRequest();
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<CategoriaDTO> Get(int id)
        {
            try
            {
                var categoria = _uof.CategoriaRepository.GetById(p => p.CategoriaId == id);

                if (categoria is null)
                {
                    return NotFound($"Categoria com id = {id} não encontrada!");
                }
                var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

                return categoriaDto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpPost]
        public ActionResult Post(CategoriaDTO categoriaDto)
        {
            var categoria = _mapper.Map<Categoria>(categoriaDto);

            if (categoria is null)
            {
                return BadRequest("Dados inválidos!");
            }
            _uof.CategoriaRepository.Add(categoria);
            _uof.Commit();

            var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

            return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId }, categoriaDTO);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, CategoriaDTO categoriaDto)
        {
            if (id != categoriaDto.CategoriaId)
            {
                return BadRequest("Dados inválidos!");
            }

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            _uof.CategoriaRepository.Update(categoria);
            _uof.Commit();

            return Ok(categoria);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<CategoriaDTO> Delete(int id)
        {
            var categoria = _uof.CategoriaRepository.GetById(p => p.CategoriaId == id);

            if (categoria is null)
            {
                return NotFound($"Categoria com id = {id} não localizada!");
            }

            _uof.CategoriaRepository.Delete(categoria);
            _uof.Commit();

            var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

            return Ok(categoriaDto);
        }
    }
}
