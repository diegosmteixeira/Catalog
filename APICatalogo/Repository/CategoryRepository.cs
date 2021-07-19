using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<PagedList<Category>> GetCategoryPages(CategoriesParameters categoriesParameters)
        {
            return await PagedList<Category>.ToPagedList(Get().OrderBy(n => n.Name),
                categoriesParameters.PageNumber,
                categoriesParameters.PageSize);
        }

        public async Task<IEnumerable<Category>> GetCategoryProducts()
        {
            return await Get().Include(x => x.Products).ToListAsync();
        }
    }
}