using AutoMapper;
using ProductShop.Data;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ProductShop.Dtos.Export;
using System.Text;
using System.Xml;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();

            var usersXml = File.ReadAllText(@"C:\Users\dido9\Desktop\CurrentLecture\ProductShop\ProductShop\Datasets\users.xml");
            var categoriesXml = File.ReadAllText(@"C:\Users\dido9\Desktop\CurrentLecture\ProductShop\ProductShop\Datasets\categories.xml");
            var productsXml = File.ReadAllText(@"C:\Users\dido9\Desktop\CurrentLecture\ProductShop\ProductShop\Datasets\products.xml");
            var categoriesProductsXml = File.ReadAllText(@"C:\Users\dido9\Desktop\CurrentLecture\ProductShop\ProductShop\Datasets\categories-products.xml");


            //using (context)
            //{
            //    context.Database.EnsureDeleted();
            //    context.Database.EnsureCreated();
            //}

            //Problem 1
            //var result = ImportUsers(context, usersXml);

            //Problem 2
            //var result = ImportProducts(context, productsXml);

            //Problem 3
            //var result = ImportCategories(context, categoriesXml);

            //Problem 4
            //var result = ImportCategoryProducts(context, categoriesProductsXml);

            //Problem 5
            //var result = GetProductsInRange(context);

            //Problem 6
            //var result = GetSoldProducts(context);

            //Problem 7
            //var result = GetCategoriesByProductsCount(context);

            //Problem 8
            var result = GetUsersWithProducts(context);

            System.Console.WriteLine(result);
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportUserDto[]), new XmlRootAttribute("Users"));

            var users = (ImportUserDto[])serializer.Deserialize(new StringReader(inputXml));

            var config = new MapperConfiguration(x => x.AddProfile<ProductShopProfile>());

            IMapper mapper = config.CreateMapper();

            foreach (var userDto in users)
            {
                var user = mapper.Map<User>(userDto);

                context.Users.Add(user);
            }


            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }


        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportProductsDto[]), new XmlRootAttribute("Products"));

            var productsDto = (ImportProductsDto[])serializer.Deserialize(new StringReader(inputXml));

            var config = new MapperConfiguration(x => x.AddProfile<ProductShopProfile>());

            IMapper mapper = config.CreateMapper();


            foreach (var productDto in productsDto)
            {
                var product = mapper.Map<Product>(productDto);
                context.Products.Add(product);
            }

            context.SaveChanges();

            return $"Successfully imported {productsDto.Count()}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportCategoriesDto[]), new XmlRootAttribute("Categories"));

            var categoriesDtos = (ImportCategoriesDto[])serializer.Deserialize(new StringReader(inputXml));

            var config = new MapperConfiguration(x => x.AddProfile<ProductShopProfile>());

            IMapper mapper = config.CreateMapper();

            foreach (var dto in categoriesDtos)
            {
                var cat = mapper.Map<Category>(dto);

                context.Categories.Add(cat);
            }

            context.SaveChanges();

            return $"Successfully imported {categoriesDtos.Count()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportProductsCategoriesDto[]), new XmlRootAttribute("CategoryProducts"));

            var categoriesProductsDtos = (ImportProductsCategoriesDto[])serializer.Deserialize(new StringReader(inputXml));

            var config = new MapperConfiguration(x => x.AddProfile<ProductShopProfile>());

            IMapper mapper = config.CreateMapper();

            foreach (var dto in categoriesProductsDtos)
            {
                var catPro = mapper.Map<CategoryProduct>(dto);
                context.CategoryProducts.Add(catPro);
            }

            context.SaveChanges();

            return $"Successfully imported {categoriesProductsDtos.Count()}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products.Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(x => new GetProductsInRangeDto
                {
                    Name = x.Name,
                    Price = x.Price,
                    Buyer = x.Buyer.FirstName + " " + x.Buyer.LastName
                })
                .Take(10)
                .ToArray();

            var sb = new StringBuilder();

            XmlSerializer serializer = new XmlSerializer(typeof(GetProductsInRangeDto[]), new XmlRootAttribute("Products"));

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("", ""),
            });

            serializer.Serialize(new StringWriter(sb), products, namespaces);

            return sb.ToString().Trim();
        }

        //public static string GetSoldProducts(ProductShopContext context)
        //{
        //    var users = context.Users.Where(u => u.ProductsSold.Any())
        //        .OrderBy(u => u.LastName)
        //        .ThenBy(u => u.FirstName)
        //        .Select(x => new GetSoldProductsDto
        //        {
        //            FirstName = x.FirstName,
        //            LastName = x.LastName,
        //            SoldProducts = x.ProductsSold.Select(p => new ProductDto
        //            {
        //                Name = p.Name,
        //                Price = p.Price
        //            }).ToList()
        //        })
        //        .Take(5)
        //        .ToArray();
        //
        //    var sb = new StringBuilder();
        //
        //    XmlSerializer serializer = new XmlSerializer(typeof(GetSoldProductsDto[]), new XmlRootAttribute("Users"));
        //
        //    var namespaces = new XmlSerializerNamespaces(new[]
        //    {
        //        new XmlQualifiedName("", ""),
        //    });
        //
        //    serializer.Serialize(new StringWriter(sb), users, namespaces);
        //
        //    return sb.ToString().Trim();
        //}

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories.Select(x => new CategoryDto
            {
                Name = x.Name,
                Count = x.CategoryProducts.Count(),
                AveragePrice = x.CategoryProducts.Average(cp => cp.Product.Price),
                TotalRevenue = x.CategoryProducts.Sum(cp => cp.Product.Price)
            }).OrderByDescending(p => p.Count)
                .ThenBy(p => p.TotalRevenue)
                .ToArray();

            var sb = new StringBuilder();

            XmlSerializer serializer = new XmlSerializer(typeof(CategoryDto[]), new XmlRootAttribute("Categories"));

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("", ""),
            });

            serializer.Serialize(new StringWriter(sb), categories, namespaces);

            return sb.ToString().Trim();
        }


        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Any())
                .OrderByDescending(ps => ps.ProductsSold.Count())
                .Select(x => new UsersDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age,
                    SoldProducts = new SoldProductsDto
                    {
                        Count = x.ProductsSold.Count(),
                        ProductDtos = x.ProductsSold.Select(ps => new ProductDto
                        {
                            Name = ps.Name,
                            Price = ps.Price
                        }).OrderByDescending(p => p.Price).ToArray()
                    }
                })
                .Take(10)
                .ToArray();

            var usersToPrint = new UsersCountDto
            {
                Count = context.Users.Where(x => x.ProductsSold.Any()).Count(),
                Users = users.ToArray()
            };

            var sb = new StringBuilder();

            XmlSerializer serializer = new XmlSerializer(typeof(UsersCountDto), new XmlRootAttribute("Users"));

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("", ""),
            });

            serializer.Serialize(new StringWriter(sb), usersToPrint, namespaces);

            return sb.ToString().Trim();
        }
    }
}