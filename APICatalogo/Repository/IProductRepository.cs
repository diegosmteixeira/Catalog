using APICatalogo.Models;
using APICatalogo.Pagination;
using System.Collections.Generic;

namespace APICatalogo.Repository
{
    public interface IProductRepository : IRepository<Product>
    {
        PagedList<Product> GetProducts(ProductsParameters productsParameters);
        IEnumerable<Product> GetProductsByPrice();
    }
}
