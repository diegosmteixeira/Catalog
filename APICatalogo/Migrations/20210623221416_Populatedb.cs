using Microsoft.EntityFrameworkCore.Migrations;

namespace APICatalogo.Migrations
{
    public partial class Populatedb : Migration
    {
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("Insert into Category(Name, ImageUrl) Values('Drinks','http://www.macoratti.net/Imagens/1.jpg')");
            mb.Sql("Insert into Category(Name, ImageUrl) Values('Food','http://www.macoratti.net/Imagens/2.jpg')");
            mb.Sql("Insert into Category(Name, ImageUrl) Values('Dessert','http://www.macoratti.net/Imagens/3.jpg')");

            mb.Sql("Insert into Product(Name,Description,Price,ImageUrl,Stock,Date,CategoryId) " +
                               "Values('Coke Diet', 'Soda 350ml',5.45,'http://www.macoratti.net/Imagens/coca.jpg',50,now(),(Select CategoryId from Category where Name='Drinks'))");

            mb.Sql("Insert into Product(Name, Description,Price,ImageUrl,Stock,Date,CategoryId) " +
                                "Values('Sandwich','Tuna sandwich',8.50,'http://www.macoratti.net/Imagens/atum.jpg',10,now(),(Select CategoryId from Category where Name='Food'))");

            mb.Sql("Insert into Product(Name, Description,Price,ImageUrl,Stock,Date,CategoryId) " +
                                "Values('Pudding','Condensed milk flan',6.75,'http://www.macoratti.net/Imagens/pudim.jpg',20,now(),(Select CategoryId from Category where Name='Dessert'))");

        }

        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from Category");
            mb.Sql("Delete from Product");
        }
    }
}
