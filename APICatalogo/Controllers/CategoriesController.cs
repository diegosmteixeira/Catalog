using APICatalogo.DTO;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using APICatalogo.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
 * ModelBinding:
 * [BindNever] = don't match information to parameter
 * [BindRequired] = error on ModelState if no data sended

 * Data Source:
 * [FromForm]
 * [FromRoute]
 * [FromQuery] = querystring
 * [FromHeader] = HTTP Header
 * [FromBody] = Body request
 * [FromServices] = container of dependency injection
*/

namespace APICatalogo.Controllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/v{v:apiVersion}/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnityOfWork _uof;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        public CategoriesController(IUnityOfWork uof, IConfiguration config, ILogger<CategoriesController> logger, IMapper mapper)
        {
            //this dependency injection stay visible to all ActionResult
            _uof = uof;
            _configuration = config;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("autor")]
        public string GetAutor()
        {
            var autor = _configuration["autor"];
            var connect = _configuration["ConnectionStrings:DefaultConnection"];

            return $"Autor: {autor} \nConnection: {connect}";
        }

        [HttpGet("hello/{name}")]
        public ActionResult<string> GetHello([FromServices] IMyService myService, string name)
        {
            //Dependency Injection [FromServices] stay visible just to specified method
            return myService.Hello(name);
        }

        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriesProducts()
        {
            _logger.LogInformation("========GET api/categories/products ==========");
            var category = await _uof.CategoryRepository.GetCategoryProducts();
            var categoryDto = _mapper.Map<List<CategoryDTO>>(category);
            return categoryDto;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> Get([FromQuery] CategoriesParameters categoriesParameters)
        {
            _logger.LogInformation("========GET api/categories ==========");
            try
            {
                var category = await _uof.CategoryRepository.GetCategoryPages(categoriesParameters);

                var metadata = new
                {
                    category.TotalCount,
                    category.PageSize,
                    category.CurrentPage,
                    category.HasNext,
                    category.HasPrevious
                };
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                var categoryDto = _mapper.Map<List<CategoryDTO>>(category);
                return categoryDto;

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Database categories access error.");
            }
        }

        /// <summary>
        /// Get a Category by id
        /// </summary>
        /// <param name="id">category code</param>
        /// <returns>Category Object</returns>
        [HttpGet("{id}", Name = "GetCategory")]
        [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDTO>> Get(int id)
        {
            try
            {
                var category = await _uof.CategoryRepository.GetById(c => c.CategoryId == id);

                _logger.LogInformation($"========GET api/categories/id = {id}==========");

                if (category == null)
                {
                    _logger.LogInformation("========GET api/categories/id = {id} NOT FOUND==========");
                    return NotFound($"Category id: {id} was not found.");
                }
                var categoryDto = _mapper.Map<CategoryDTO>(category);
                return categoryDto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Database categories access error.");
            }
        }

        /// <summary>
        /// Include a new category
        /// </summary>
        /// <remarks>
        /// Request example:
        /// 
        ///     POST api/categories
        ///     {
        ///         "categoryId": 1,
        ///         "name": "category1",
        ///         "imageUrl: "http://test.io/1.jpg"
        ///     }
        /// </remarks>
        /// <param name="categoryDto">Category Object</param>
        /// <returns>Return a Category Object</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Post([FromBody] CategoryDTO categoryDto)
        {
            try
            {
                var category = _mapper.Map<Category>(categoryDto);
                _uof.CategoryRepository.Add(category);
                await _uof.Commit();

                var categoryDTO = _mapper.Map<CategoryDTO>(category);
                return new CreatedAtRouteResult("GetCategory", new { id = category.CategoryId }, categoryDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error to create new category.");
            }
        }

        [HttpPut("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions),
            nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> Put(int id, [FromBody] CategoryDTO categoryDto)
        {
            try
            {
                if (id != categoryDto.CategoryId)
                {
                    return BadRequest($"Could not possible save changes to category id: {id}.");
                }

                var category = _mapper.Map<Category>(categoryDto);

                _uof.CategoryRepository.Update(category);
                await _uof.Commit();
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                        $"Error to save changes to category id: {id}.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<CategoryDTO>> Delete(int id)
        {
            try
            {
                var category = await _uof.CategoryRepository.GetById(c => c.CategoryId == id);

                if (category == null)
                {
                    return NotFound($"Category id: {id} was not found.");
                }
                _uof.CategoryRepository.Delete(category);
                await _uof.Commit();

                var categoryDto = _mapper.Map<CategoryDTO>(category);
                return categoryDto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error to delete category id: {id}.");
            }
        }
    }
}