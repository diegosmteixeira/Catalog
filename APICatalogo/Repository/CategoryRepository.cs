using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace APICatalogo.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }
        public PagedList<Category> GetCategoryPages(CategoriesParameters categoriesParameters)
        {
            return PagedList<Category>.ToPagedList(Get().OrderBy(n => n.CategoryId),
                categoriesParameters.PageNumber, categoriesParameters.PageSize);
        }

        public IEnumerable<Category> GetCategoryProducts()
        {
            return Get().Include(x => x.Products);
        }
    }
}
