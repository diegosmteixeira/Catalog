using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public CategoriesController(AppDbContext context, IConfiguration config)
        {
            //this dependency injection stay visible to all ActionResult
            _context = context;
            _configuration = config;
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
            return _context.Categories.Include(x => x.Products).ToList();
        }

        [HttpGet]
        public ActionResult<IEnumerable<Category>> Get()
        {
            try
            {
                return _context.Categories.AsNoTracking().ToList();

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
                var category = _context.Categories.AsNoTracking()
                    .FirstOrDefault(p => p.CategoryId == id);

                if (category == null)
                {
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
                _context.Categories.Add(category);
                _context.SaveChanges();

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
                _context.Entry(category).State = EntityState.Modified;
                _context.SaveChanges();
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
                var category = _context.Categories.FirstOrDefault(p => p.CategoryId == id);

                if (category == null)
                {
                    return NotFound($"Category id: {id} was not found.");
                }
                _context.Categories.Remove(category);
                _context.SaveChanges();
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