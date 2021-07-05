using APICatalogo.Models;
using System.Collections.Generic;

namespace APICatalogo.Repository
{
    public interface IProductRepository : IRepository<Product>
    {
        IEnumerable<Product> GetProductsByPrice();
    }
}
