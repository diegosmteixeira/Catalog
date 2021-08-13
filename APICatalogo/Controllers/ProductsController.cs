using APICatalogo.DTO;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Newtonsoft.Json;
using System.Collections.Generic;
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
    [EnableQuery]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/[controller]")]
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
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByPrice()
        {
            var products = await _uof.ProductRepository.GetProductsByPrice();
            var productsDto = _mapper.Map<List<ProductDTO>>(products);
            return productsDto;
        }

        /// <summary>
        /// Show all products
        /// </summary>
        /// <returns>Return a product object list</returns>
        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> Get([FromQuery] ProductsParameters productsParameters)
        {
            var products = await _uof.ProductRepository.GetProducts(productsParameters);
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

        /// <summary>
        /// Get a product by your product identifierId
        /// </summary>
        /// <param name="id">product code</param>
        /// <returns>Product Object</returns>
        [HttpGet("{id}", Name = "GetProduct")]
        public async Task<ActionResult<ProductDTO>> Get(int id)
        {
            var product = await _uof.ProductRepository.GetById(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }
            var productDto = _mapper.Map<ProductDTO>(product);
            return productDto;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ProductDTO productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            _uof.ProductRepository.Add(product);
            await _uof.Commit();

            var productDTO = _mapper.Map<ProductDTO>(product);

            return new CreatedAtRouteResult("GetProduct", new { id = product.ProductId }, productDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] ProductDTO productDto)
        {

            if (id != productDto.ProductId)
            {
                return BadRequest();
            }

            var product = _mapper.Map<Product>(productDto);

            _uof.ProductRepository.Update(product);
            await _uof.Commit();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductDTO>> Delete(int id)
        {
            var product = await _uof.ProductRepository.GetById(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }
            _uof.ProductRepository.Delete(product);
            await _uof.Commit();

            var productDTO = _mapper.Map<ProductDTO>(product);
            return productDTO;
        }
    }
}
