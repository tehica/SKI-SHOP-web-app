using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

using Core.Entities;

using Microsoft.Extensions.Logging;

namespace Infrastructure.Data
{
    public class ApplicationDbSeed
    {
        public static async Task SeedAsync(ApplicationDbContext db, ILoggerFactory loggerFactory)
        {
            try
            {
                
                if (!db.ProductBrand.Any())
                {
                    var brandsData =
                        File.ReadAllText("../Infrastructure/Data/SeedData/brands.json");

                    var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);

                    foreach (var item in brands)
                    {
                        db.ProductBrand.Add(item);
                    }

                    await db.SaveChangesAsync();
                }

                if (!db.ProductTypes.Any())
                {
                    var typesData =
                        File.ReadAllText("../Infrastructure/Data/SeedData/types.json");

                    var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);

                    foreach (var item in types)
                    {
                        db.ProductTypes.Add(item);
                    }

                    await db.SaveChangesAsync();
                }

                if (!db.Products.Any())
                {
                    var productsData =
                        File.ReadAllText("../Infrastructure/Data/SeedData/products.json");

                    var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                    foreach (var item in products)
                    {
                        db.Products.Add(item);
                    }

                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<ApplicationDbSeed>();
                logger.LogError(ex.Message);
            }
        }
    }
}