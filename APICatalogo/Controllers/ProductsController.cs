using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get()
        {
            return _context.Products.AsNoTracking().ToList();
        }

        [HttpGet("{id}", Name = "GetProduct")]
        public ActionResult<Product> Get(int id)
        {
            var product = _context.Products.AsNoTracking()
                .FirstOrDefault(p => p.ProductId == id);

            if(product == null)
            {
                return NotFound();
            }
            return product;
        }

        [HttpPost]
        public ActionResult Post([FromBody]Product product)
        {
            //[ApiController] validate this
            //disable error messenger: (now stay in api controller)
            //if (!modelstate.isvalid)
            //{
            //    return badrequest(modelstate);
            //}
            _context.Products.Add(product);
            _context.SaveChanges();

            return new CreatedAtRouteResult("GetProduct", new { id = product.ProductId }, product);
        }
    }
}
