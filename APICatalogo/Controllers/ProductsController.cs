using APICatalogo.DTO;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private readonly IMapper _mapper;
        public ProductsController(IUnityOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }

        [HttpGet("lowestprice")]
        public ActionResult<IEnumerable<ProductDTO>> GetProductsByPrice()
        {
            var products = _uof.ProductRepository.GetProductsByPrice().ToList();
            var productsDto = _mapper.Map<List<ProductDTO>>(products);
            return productsDto;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<ProductDTO>> Get([FromQuery] ProductsParameters productsParameters)
        {
            var products = _uof.ProductRepository.GetProducts(productsParameters);
            var metadata = new
            {
                products.TotalCount,
                products.PageSize,
                products.CurrentPage,
                products.TotalPages,
                products.HasNext,
                products.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var productsDto = _mapper.Map<List<ProductDTO>>(products);
            return productsDto;
        }

        [HttpGet("{id}", Name = "GetProduct")]
        public ActionResult<ProductDTO> Get(int id)
        {
            var product = _uof.ProductRepository.GetById(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }
            var productDto = _mapper.Map<ProductDTO>(product);
            return productDto;
        }

        [HttpPost]
        public ActionResult Post([FromBody] ProductDTO productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            _uof.ProductRepository.Add(product);
            _uof.Commit();

            var productDTO = _mapper.Map<ProductDTO>(product);

            return new CreatedAtRouteResult("GetProduct", new { id = product.ProductId }, productDTO);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] ProductDTO productDto)
        {

            if (id != productDto.ProductId)
            {
                return BadRequest();
            }

            var product = _mapper.Map<Product>(productDto);

            _uof.ProductRepository.Update(product);
            _uof.Commit();
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult<ProductDTO> Delete(int id)
        {
            var product = _uof.ProductRepository.GetById(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }
            _uof.ProductRepository.Delete(product);
            _uof.Commit();

            var productDTO = _mapper.Map<ProductDTO>(product);
            return productDTO;
        }
    }
}
