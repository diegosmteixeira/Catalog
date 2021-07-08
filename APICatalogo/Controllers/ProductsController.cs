using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    public class ProductsController : ControllerBase
    {
        private readonly IUnityOfWork _uof;
        public ProductsController(IUnityOfWork uof)
        {
            _uof = uof;
        }

        [HttpGet("lowestprice")]
        public ActionResult<IEnumerable<Product>> GetProductsByPrice()
        {
            return _uof.ProductRepository.GetProductsByPrice().ToList();
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<Product>> Get()
        {
            return _uof.ProductRepository.Get().ToList();
        }

        [HttpGet("{id}", Name = "GetProduct")]
        public ActionResult<Product> Get(int id)
        {

            var product = _uof.ProductRepository.GetById(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }
            return product;
        }

        [HttpPost]
        public ActionResult Post([FromBody] Product product)
        {

            _uof.ProductRepository.Add(product);
            _uof.Commit();

            return new CreatedAtRouteResult("GetProduct", new { id = product.ProductId }, product);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Product product)
        {

            if (id != product.ProductId)
            {
                return BadRequest();
            }
            _uof.ProductRepository.Update(product);
            _uof.Commit();
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult<Product> Delete(int id)
        {
            var product = _uof.ProductRepository.GetById(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }
            _uof.ProductRepository.Delete(product);
            _uof.Commit();
            return product;
        }
    }
}
