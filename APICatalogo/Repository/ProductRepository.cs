using APICatalogo.Context;
using APICatalogo.Models;
using System.Collections.Generic;
using System.Linq;

namespace APICatalogo.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public IEnumerable<Product> GetProductsByPrice()
        {
            return Get().OrderBy(c => c.Price).ToList();
        }
    }
}
