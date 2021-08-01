using APICatalogo.Context;
using APICatalogo.Models;

namespace ApiCatalogoxUnitTests
{
    public class DBUnitTestsMockInitializer
    {
        public DBUnitTestsMockInitializer()
        { }
        public void Seed(AppDbContext context)
        {
            context.Categories.Add
            (new Category { CategoryId = 999, Name = "Drinks999", ImageUrl = "drinks999.jpg" });

            context.Categories.Add
            (new Category { CategoryId = 2, Name = "Juices", ImageUrl = "juices1.jpg" });

            context.Categories.Add
            (new Category { CategoryId = 3, Name = "Sweets", ImageUrl = "sweets1.jpg" });

            context.Categories.Add
            (new Category { CategoryId = 4, Name = "Pretzels", ImageUrl = "pretzels1.jpg" });

            context.Categories.Add
            (new Category { CategoryId = 5, Name = "Pies", ImageUrl = "pies1.jpg" });

            context.Categories.Add
            (new Category { CategoryId = 6, Name = "Cakes", ImageUrl = "cakes1.jpg" });

            context.Categories.Add
            (new Category { CategoryId = 7, Name = "Lanches", ImageUrl = "snacks.jpg" });

            context.SaveChanges();
        }

    }
}