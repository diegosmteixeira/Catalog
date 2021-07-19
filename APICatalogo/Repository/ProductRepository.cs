using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using System.Collections.Generic;
using System.Linq;

namespace APICatalogo.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public PagedList<Product> GetProducts(ProductsParameters productsParameters)
        {
            //return Get()
            //.OrderBy(on => on.Name)
            //.Skip((productsParameters.PageNumber - 1) * productsParameters.PageSize)
            //.Take(productsParameters.PageSize)
            //.ToList();

            return PagedList<Product>.ToPagedList(Get().OrderBy(n => n.ProductId),
                productsParameters.PageNumber, productsParameters.PageSize);
        }

        public IEnumerable<Product> GetProductsByPrice()
        {
            return Get().OrderBy(c => c.Price).ToList();
        }
    }
}
