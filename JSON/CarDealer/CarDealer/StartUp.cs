using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            CarDealerContext context = new CarDealerContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            var suppliersJson = File.ReadAllText(@"C:\Users\dido9\Desktop\CurrentLecture\CarDealer\CarDealer\Datasets\suppliers.json");
            var carsJson = File.ReadAllText(@"C:\Users\dido9\Desktop\CurrentLecture\CarDealer\CarDealer\Datasets\cars.json");
            var customersJson = File.ReadAllText(@"C:\Users\dido9\Desktop\CurrentLecture\CarDealer\CarDealer\Datasets\customers.json");
            var partsJson = File.ReadAllText(@"C:\Users\dido9\Desktop\CurrentLecture\CarDealer\CarDealer\Datasets\parts.json");
            var salesJson = File.ReadAllText(@"C:\Users\dido9\Desktop\CurrentLecture\CarDealer\CarDealer\Datasets\sales.json");

            //Problem 9
            //string result = ImportSuppliers(context, suppliersJson);

            //Problem 10
            //string result = ImportParts(context, partsJson);

            //Problem 11
            //string result = ImportCars(context, carsJson);

            //Problem 12
            //string result = ImportCustomers(context, customersJson);

            //Problem 13
            //string result = ImportSales(context, salesJson);

            //Problem 14
            //string result = GetOrderedCustomers(context);

            //Problem 15
            //string result = GetCarsFromMakeToyota(context);

            //Problem 16
            //string result = GetLocalSuppliers(context);

            //Problem 17
            //string result = GetCarsWithTheirListOfParts(context);

            //Problem 18
            //string result = GetTotalSalesByCustomer(context);

            //Console.WriteLine(result);
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Count >= 1)
                .Select(x => new
                {
                    fullName = x.Name,
                    boughtCars = x.Sales.Count,
                    spentMoney = x.Sales.Select(s => s.Car.PartCars.Select(pc => pc.Part).Sum(pc => pc.Price)).Sum()
                })
                .OrderByDescending(s => s.spentMoney)
                .ThenByDescending(b => b.boughtCars)
                .ToArray();

            var json = JsonConvert.SerializeObject(customers, Formatting.Indented);
            return json;
        }
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(x =>
            new
            {
                car = new
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance,                    
                },
                parts = x.PartCars.Select(pc => new
                {
                    Name = pc.Part.Name,
                    Price = pc.Part.Price
                }).ToArray()
            }).ToArray();

            var json = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return json;
        }
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(x =>
                new
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count()
                }).ToArray();

            var json = JsonConvert.SerializeObject(suppliers, Formatting.Indented);

            return json;
        }
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(m => m.Model)
                .ThenByDescending(td => td.TravelledDistance)
                .Select(x => new
                {
                    Id = x.Id,
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                }
                ).ToArray();

            var json = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return json;
        }
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers.OrderBy(x => x.BirthDate).ThenBy(x => x.IsYoungDriver).ToArray();

            var json = JsonConvert.SerializeObject(customers, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });

            return json;
        }
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}.";
        }
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}.";
        }
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var cars = JsonConvert.DeserializeObject<Car[]>(inputJson);

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count()}.";
        }
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var suppliers = context.Suppliers.Select(x => x.Id).ToArray();

            Part[] parts = JsonConvert.DeserializeObject<Part[]>(inputJson);

            List<Part> partsToAdd = new List<Part>();

            foreach (var part in parts)
            {
                if (suppliers.Contains(part.SupplierId))
                {
                    partsToAdd.Add(part);
                }
            }

            context.Parts.AddRange(partsToAdd);
            context.SaveChanges();

            return $"Successfully imported {partsToAdd.Count()}.";
        }
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}.";
        }
    }
}