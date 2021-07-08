using APICatalogo.Models;
using APICatalogo.Repository;
using APICatalogo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        public CategoriesController(IUnityOfWork uof, IConfiguration config, ILogger<CategoriesController> logger)
        {
            //this dependency injection stay visible to all ActionResult
            _uof = uof;
            _configuration = config;
            _logger = logger;
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
        public ActionResult<IEnumerable<Category>> GetCategoriesProducts()
        {
            _logger.LogInformation("========GET api/categories/products ==========");
            return _uof.CategoryRepository.GetCategoryProducts().ToList();
        }

        [HttpGet]
        public ActionResult<IEnumerable<Category>> Get()
        {
            _logger.LogInformation("========GET api/categories ==========");
            try
            {
                return _uof.CategoryRepository.Get().ToList();

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Database categories access error.");
            }
        }

        [HttpGet("{id}", Name = "GetCategory")]
        public ActionResult<Category> Get(int id)
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
                return category;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Database categories access error.");
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody] Category category)
        {
            try
            {
                _uof.CategoryRepository.Add(category);
                _uof.Commit();

                return new CreatedAtRouteResult("GetCategory", new { id = category.CategoryId }, category);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error to create new category.");
            }
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Category category)
        {
            try
            {
                if (id != category.CategoryId)
                {
                    return BadRequest($"Could not possible save changes to category id: {id}.");
                }
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
        public ActionResult<Category> Delete(int id)
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
                return category;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error to delete category id: {id}.");
            }
        }
    }
}