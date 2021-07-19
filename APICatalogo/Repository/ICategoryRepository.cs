using APICatalogo.Models;
using APICatalogo.Pagination;
using System.Collections.Generic;

namespace APICatalogo.Repository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        PagedList<Category> GetCategoryPages(CategoriesParameters categoriesParameters);
        IEnumerable<Category> GetCategoryProducts();
    }
}
