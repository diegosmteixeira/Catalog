using APICatalogo.Context;
using APICatalogo.Controllers;
using APICatalogo.DTO;
using APICatalogo.DTO.Mappings;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ApiCatalogoxUnitTests
{
    public class CategoriesUnitTestController
    {
        private IUnityOfWork _uof;
        private IMapper _mapper;
        public IConfiguration _configuration;
        private ILogger<CategoriesController> _logger;

        public static DbContextOptions<AppDbContext> dbContextOptions { get; }

        public static string mySqlConnection =
            "Server=localhost;Database=CatalogoDB;Uid=root;Pwd=root";

        static CategoriesUnitTestController()
        {
            dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseMySql(mySqlConnection,
                  ServerVersion.AutoDetect(mySqlConnection))
                .Options;
        }

        public CategoriesUnitTestController()
        {
            var map = new MapperConfiguration(configuration =>
            {
                configuration.AddProfile(new MappingProfile());
            });
            _mapper = map.CreateMapper();

            var context = new AppDbContext(dbContextOptions);

            //DBUnitTestsMockInitializer db = new DBUnitTestsMockInitializer();
            //db.Seed(context);

            _uof = new UnityOfWork(context);

            IConfiguration cfg = new ConfigurationBuilder().Build();

            _configuration = cfg;

            var serviceProvider = new ServiceCollection().AddLogging().BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();

            var logger = factory.CreateLogger<CategoriesController>();
            _logger = logger;
        }

        //================================================
        // Unit Test (GET)
        //================================================
        [Fact]
        public async Task GetCategories_Return_OkResult()
        {
            //Arrange  
            var controller = new CategoriesController(_uof, _configuration, _mapper, _logger);

            //Act
            var param = new CategoriesParameters();
            var data = await controller.Get(param);

            //Assert  
            Assert.IsType<List<CategoryDTO>>(data.Value);
        }

        //================================================
        // Unit Test (GET - Bad Request)
        //================================================
        [Fact]
        public async Task GetCategories_Return_BadRequestResult()
        {
            //Arrange
            var controller = new CategoriesController(_uof, _configuration, _mapper, _logger);

            //Act
            var param = new CategoriesParameters();
            var data = await controller.Get(param);

            //Assert
            Assert.IsType<BadRequestResult>(data.Result);
        }

        //================================================
        // Unit Test (GET - List<CategoryDTO>)
        //================================================
        [Fact]
        public async Task GetCategories_MatchResult()
        {
            //Arrange
            var controller = new CategoriesController(_uof, _configuration, _mapper, _logger);

            //Act
            var param = new CategoriesParameters();
            var data = await controller.Get(param);

            //Assert
            Assert.IsType<List<CategoryDTO>>(data.Value);

            var cat = data.Value.Should().BeAssignableTo<List<CategoryDTO>>().Subject;

            Assert.Equal("UnitTest", cat[3].Name);
            Assert.Equal("test.jpg", cat[3].ImageUrl);

            Assert.Equal("UnitTest", cat[4].Name);
            Assert.Equal("test.jpg", cat[4].ImageUrl);
        }

        //================================================
        // Unit Test (GET - ById)
        //================================================
        [Fact]
        public async Task GetCategoryById_Return_OkResult()
        {
            //Arrange
            var controller = new CategoriesController(_uof, _configuration, _mapper, _logger);
            var catId = 16;

            //Act
            var data = await controller.Get(catId);

            //Assert
            Assert.IsType<CategoryDTO>(data.Value);
        }

        //================================================
        // Unit Test (GET - NotFound)
        //================================================
        [Fact]
        public async Task GetCategoryById_Return_NotFoundResult()
        {
            //Arrange
            var controller = new CategoriesController(_uof, _configuration, _mapper, _logger);
            var catId = 9999;

            //Act
            var data = await controller.Get(catId);

            //Assert
            Assert.IsType<NotFoundObjectResult>(data.Result);
        }

        //================================================
        // Unit Test (POST)
        //================================================
        [Fact]
        public async Task Post_Category_AddValidData_Return_CreatedResult()
        {
            //Arrange
            var controller = new CategoriesController(_uof, _configuration, _mapper, _logger);

            var cat = new CategoryDTO()
            {
                Name = "UnitTestPost",
                ImageUrl = "test.jpg"
            };

            //Act
            var data = await controller.Post(cat);

            //Assert
            Assert.IsType<CreatedAtRouteResult>(data);
        }

        //================================================
        // Unit Test (PUT)
        //================================================
        [Fact]
        public async Task Put_Category_Update_ValidData_Return_OkResult()
        {
            //Arrange
            var controller = new CategoriesController(_uof, _configuration, _mapper, _logger);

            var catId = 19;

            //Act
            var existingPost = await controller.Get(catId);
            var result = existingPost.Value.Should().BeAssignableTo<CategoryDTO>().Subject;

            var catDto = new CategoryDTO();
            catDto.CategoryId = catId;
            catDto.Name = "Updated Category";
            catDto.ImageUrl = result.ImageUrl;

            var updatedData = await controller.Put(catId, catDto);

            //Assert
            Assert.IsType<ObjectResult>(updatedData);
        }

        //================================================
        // Unit Test (PUT)
        //================================================
        [Fact]
        public async Task Delete_Category_Return_OkResult()
        {
            //Arrange
            var controller = new CategoriesController(_uof, _configuration, _mapper, _logger);

            var catId = 21;

            //Act
            var data = await controller.Delete(catId);

            //Assert
            Assert.IsType<CategoryDTO>(data.Value);
        }
    }
}