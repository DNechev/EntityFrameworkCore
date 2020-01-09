using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            var userJson = File.ReadAllText(@"C:\Users\dido9\Desktop\CurrentLecture\ProductShop\ProductShop\Datasets\users.json");
            var productsJson = File.ReadAllText(@"C:\Users\dido9\Desktop\CurrentLecture\ProductShop\ProductShop\Datasets\products.json");
            var categoriesJson = File.ReadAllText(@"C:\Users\dido9\Desktop\CurrentLecture\ProductShop\ProductShop\Datasets\categories.json");
            var categoriesProductsJson = File.ReadAllText(@"C:\Users\dido9\Desktop\CurrentLecture\ProductShop\ProductShop\Datasets\categories-products.json");

            //Problem 1
            //string result = ImportUsers(context, userJson);

            //Problem 2
            //string result = ImportProducts(context, productsJson);

            //Problem 3
            //string result = ImportCategories(context, categoriesJson);

            //Problem 4
            //string result = ImportCategoryProducts(context, categoriesProductsJson);

            //Problem 5
            //string result = GetProductsInRange(context);

            //Problem 6
            //string result = GetSoldProducts(context);

            //Problem 7
            string result = GetCategoriesByProductsCount(context);

            //Problem 8
            //string result = GetUsersWithProducts(context);

            Console.WriteLine(result);
        }
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            User[] users = JsonConvert.DeserializeObject<User[]>(inputJson)
                .Where(u => u.LastName != null && u.LastName.Length >= 3)
                .ToArray();

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            Product[] products = JsonConvert.DeserializeObject<Product[]>(inputJson)
                .Where(p => p.Name.Length >= 3 && p.Price != null && p.SellerId != null)
                .ToArray();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count()}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            Category[] categories = JsonConvert.DeserializeObject<Category[]>(inputJson)
                .Where(c => c.Name != null && c.Name.Length >= 3 && c.Name.Length <= 15)
                .ToArray();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            CategoryProduct[] categoriesProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);

            context.CategoryProducts.AddRange(categoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {categoriesProducts.Count()}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(p => new ProductDto
                {
                    Name = p.Name,
                    Price = p.Price,
                    Seller = $"{p.Seller.FirstName} {p.Seller.LastName}"
                }
                )
                .OrderBy(p => p.Price)
                .ToArray();

            var json = JsonConvert.SerializeObject(products, Formatting.Indented);

            return json;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users.Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    soldProducts = x.ProductsSold.Where(p => p.Buyer != null).Select(p => new
                    {
                        name = p.Name,
                        price = p.Price,
                        buyerFirstName = p.Buyer.FirstName,
                        buyerLastName = p.Buyer.LastName
                    })
                }
                ).OrderBy(u => u.lastName).ThenBy(u => u.firstName).ToArray();

            var json = JsonConvert.SerializeObject(users, Formatting.Indented);

            return json;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories.OrderByDescending(c => c.CategoryProducts.Count)
                 .Select(x => new
                 {
                     category = x.Name,
                     productsCount = x.CategoryProducts.Count,
                     AveragePrice = $"{x.CategoryProducts.Average(c => c.Product.Price):F2}",
                     TotalRevenue = $"{x.CategoryProducts.Sum(c => c.Product.Price)}"
                 }).ToArray();

            string json = JsonConvert.SerializeObject(categories,
                new JsonSerializerSettings()
                {
                    ContractResolver = new DefaultContractResolver()
                    {
                        NamingStrategy = new CamelCaseNamingStrategy(),
                    },

                    Formatting = Formatting.Indented
                }
            );

            return json;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var filteredUsers = context.Users.Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderByDescending(u => u.ProductsSold.Where(p => p.Buyer != null).Count())
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    age = x.Age,
                    soldProducts = new
                    {
                        count = x.ProductsSold.Where(p => p.Buyer != null).Count(),
                        products = x.ProductsSold.Where(p => p.Buyer != null)
                        .Select(ps => new
                        {
                            name = ps.Name,
                            price = ps.Price
                        }).ToArray()
                    },
                }
                ).ToArray();

            var result = new
            {
                usersCount = filteredUsers.Length,
                users = filteredUsers,
            };

            var json = JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });

            return json;
        }
    }
}