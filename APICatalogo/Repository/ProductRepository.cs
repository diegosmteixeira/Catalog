using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PagedList<Product>> GetProducts(ProductsParameters productsParameters)
        {
            return await PagedList<Product>.ToPagedList(Get().OrderBy(n => n.ProductId),
                productsParameters.PageNumber,
                productsParameters.PageSize);
        }

        public async Task<IEnumerable<Product>> GetProductsByPrice()
        {
            return await Get().OrderBy(c => c.Price).ToListAsync();
        }
    }
}