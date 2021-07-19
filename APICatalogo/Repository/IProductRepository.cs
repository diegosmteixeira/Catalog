using APICatalogo.Models;
using APICatalogo.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APICatalogo.Repository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<PagedList<Product>> GetProducts(ProductsParameters productsParameters);
        Task<IEnumerable<Product>> GetProductsByPrice();
    }
}
