using APICatalogo.DTO;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using APICatalogo.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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
    [Route("api/[Controller]")]
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
        public ActionResult<IEnumerable<CategoryDTO>> GetCategoriesProducts()
        {
            _logger.LogInformation("========GET api/categories/products ==========");
            var category = _uof.CategoryRepository.GetCategoryProducts().ToList();
            var categoryDto = _mapper.Map<List<CategoryDTO>>(category);
            return categoryDto;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CategoryDTO>> Get([FromQuery] CategoriesParameters categoriesParameters)
        {
            _logger.LogInformation("========GET api/categories ==========");
            try
            {
                var category = _uof.CategoryRepository.GetCategoryPages(categoriesParameters);

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

        [HttpGet("{id}", Name = "GetCategory")]
        public ActionResult<CategoryDTO> Get(int id)
        {
            try
            {
                var category = _uof.CategoryRepository.Get().FirstOrDefault(p => p.CategoryId == id);

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

        [HttpPost]
        public ActionResult Post([FromBody] CategoryDTO categoryDto)
        {
            try
            {
                var category = _mapper.Map<Category>(categoryDto);
                _uof.CategoryRepository.Add(category);
                _uof.Commit();

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
        public ActionResult Put(int id, [FromBody] CategoryDTO categoryDto)
        {
            try
            {
                if (id != categoryDto.CategoryId)
                {
                    return BadRequest($"Could not possible save changes to category id: {id}.");
                }

                var category = _mapper.Map<Category>(categoryDto);

                _uof.CategoryRepository.Update(category);
                _uof.Commit();
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                        $"Error to save changes to category id: {id}.");
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<CategoryDTO> Delete(int id)
        {
            try
            {
                var category = _uof.CategoryRepository.Get().FirstOrDefault(p => p.CategoryId == id);

                if (category == null)
                {
                    return NotFound($"Category id: {id} was not found.");
                }
                _uof.CategoryRepository.Delete(category);
                _uof.Commit();

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