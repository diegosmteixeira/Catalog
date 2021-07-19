using APICatalogo.Models;
using APICatalogo.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APICatalogo.Repository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<PagedList<Category>> GetCategoryPages(CategoriesParameters categoriesParameters);
        Task<IEnumerable<Category>> GetCategoryProducts();
    }
}