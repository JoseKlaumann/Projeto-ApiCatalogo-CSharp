using APICatalogo.Context;
using APICatalogo.Controllers;
using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Repository;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ApiCatalogoxUnitTests
{
    public class CategoriasUnitTestController
    {
        private IMapper mapper;
        private IUnitOfWork repository;
        public static DbContextOptions<AppDbContext> dbContextOptions { get; }

        public static string connectionString = "Server=localhost;Database=CatalogoDB;Uid=root;Pwd=poolparty";

        static CategoriasUnitTestController()
        {
            dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                 .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                 .Options;
        }

        public CategoriasUnitTestController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            mapper = config.CreateMapper();

            var context = new AppDbContext(dbContextOptions);

            // DBUnitTestsMockInitializer db = new DBUnitTestsMockInitializer();
            // db.Seed(context);

            repository = new UnitOfWork(context);
        }

        // Testes unitários
        // Get
        [Fact]
        public void GetCategorias_Return_OkResult()
        {
            // Arrange
            var controller = new CategoriasController(repository, mapper);

            // Act
            var data = controller.Get();

            // Assert
            Assert.IsType<List<CategoriaDTO>>(data.Value);
        }

        // Get - BadRequest
        [Fact]
        public void GetCategorias_Return_BadRequestResult()
        {
            // Arrange
            var controller = new CategoriasController(repository, mapper);

            // Act
            var data = controller.Get();

            // Assert
            Assert.IsType<BadRequestResult>(data.Result);
        }

        // Get - Retornar uma lista de objetos de categorias
        [Fact]
        public void GetCategorias_MatchResult()
        {
            // Arrange
            var controller = new CategoriasController(repository, mapper);

            // Act
            var data = controller.Get();

            // Assert
            Assert.IsType<List<CategoriaDTO>>(data.Value);
            var cat = data.Value.Should().BeAssignableTo<List<CategoriaDTO>>().Subject;

            Assert.Equal("Bebidas", cat[0].Nome);
            Assert.Equal("bebidas.jpg", cat[0].ImagemUrl);

            Assert.Equal("Lanches", cat[1].Nome);
            Assert.Equal("lanches.jpg", cat[1].ImagemUrl);
        }

        // Get - Retorno pelo Id
        [Fact]
        public void GetCategoriaById_Return_OkResult()
        {
            // Arrange
            var controller = new CategoriasController(repository, mapper);
            var catId = 2;

            // Act
            var data = controller.Get(catId);

            // Assert
            Assert.IsType<CategoriaDTO>(data.Value);
        }

        // Get Id - NotFound
        [Fact]
        public void GetCategoriaById_Return_NotFoundResult()
        {
            // Arrange
            var controller = new CategoriasController(repository, mapper);
            var catId = 999;

            // Act
            var data = controller.Get(catId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(data.Result);
        }

        // Post - CreatedResult
        [Fact]
        public void Post_Categoria_AddValidData_Return_CreatedResult()
        {
            // Arrange
            var controller = new CategoriasController(repository, mapper);
            var cat = new CategoriaDTO() { Nome = "Teste Unitário Inclusão", ImagemUrl = "testecatinclusao.jpg" };

            // Act
            var data = controller.Post(cat);

            // Assert
            Assert.IsType<CreatedAtRouteResult>(data);
        }

        //PUT - Atualizar uma categoria existente com sucesso
        [Fact]
        public void Put_Categoria_Update_ValidData_Return_OkResult()
        {
            //Arrange  
            var controller = new CategoriasController(repository, mapper);
            var catId = 7;

            //Act  
            //var existingPost = controller.Get(catId);
            //var result = existingPost.Value.Should().BeAssignableTo<CategoriaDTO>().Subject;

            var catDto = new CategoriaDTO();
            catDto.CategoriaId = catId;
            catDto.Nome = "Categoria Atualizada - Testes 1";
            catDto.ImagemUrl = "Teste.jpg";

            var updatedData = controller.Put(catId, catDto);

            //Assert  
            Assert.IsType<OkObjectResult>(updatedData);
        }

        //Delete - Deleta categoria por id - Retorna CategoriaDTO
        [Fact]
        public void Delete_Categoria_Return_OkResult()
        {
            //Arrange  
            var controller = new CategoriasController(repository, mapper);
            var catId = 11;

            //Act  
            var data = controller.Delete(catId);

            //Assert  
            Assert.IsType<OkObjectResult>(data.Result);
        }
    }
}
