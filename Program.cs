using System;
using NLog.Web;
using System.IO;
using System.Linq;
using NorthwindConsole.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace NorthwindConsole
{
    class Program
    {
        // create static instance of Logger
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Info("Program started");
            try
            {
                string choice;
                do
                {
                    Console.WriteLine("1) Display Categories");
                    Console.WriteLine("2) Add Category");
                    Console.WriteLine("3) Edit Category");
                    Console.WriteLine("4) Display Category and related products");
                    Console.WriteLine("5) Display all Categories and their related products");
                    Console.WriteLine("6) Add Product");
                    Console.WriteLine("7) Edit Product");
                    Console.WriteLine("8) Display Products");

                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");
                    if (choice == "1")
                    {
                        var db = new NWConsole_48_IBFContext();
                        var query = db.Categories.OrderBy(p => p.CategoryName);
                        Console.WriteLine($"{query.Count()} records returned");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName} - {item.Description}");
                        }

                        logger.Info($"Displaying all Categories.");

                    }
                    else if (choice == "2")
                    {
                        var db = new NWConsole_48_IBFContext();
                        Category category = InputCategory(db);
                        if(category != null) {
                            db.AddCategory(category);
                            logger.Info($"Category {category.CategoryName} added.");
                        }
                    }
                    else if (choice == "3") 
                    {
                        Console.WriteLine("Choose Category to Edit:");
                        var db = new NWConsole_48_IBFContext();
                        var category = GetCategory(db);
                        
                        if (category != null) {
                            Category UpdatedCategory = InputCategory(db);

                            if(UpdatedCategory != null) {
                                UpdatedCategory.CategoryId = category.CategoryId;
                                db.EditCategory(UpdatedCategory);
                                logger.Info($"Category (ID: {category.CategoryId}) updated.");
                            }
                        }
                    }
                    else if (choice == "4")
                    {
                        var db = new NWConsole_48_IBFContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);
                        Console.WriteLine("Select the category whose products you want to display:");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        int id = int.Parse(Console.ReadLine());

                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        foreach (Product p in category.Products)
                        {
                            Console.WriteLine(p.ProductName);
                        }
                    }
                    else if (choice == "5")
                    {
                        var db = new NWConsole_48_IBFContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Product p in item.Products)
                            {
                                Console.WriteLine($"\t{p.ProductName}");
                            }
                        }
                    }
                    else if(choice == "6") {

                        var db = new NWConsole_48_IBFContext();
                        Product product = InputProduct(db);
                        if(product != null) {
                            db.AddProduct(product);
                            logger.Info($"Product {product.ProductName} added.");
                        }

                    }
                    else if(choice == "7") 
                    {
                        Console.WriteLine("Choose Product to Edit:");
                        var db = new NWConsole_48_IBFContext();
                        var product = GetProduct(db);
                        
                        if (product != null) {
                            Product UpdatedProduct = InputProduct(db);
                            if(UpdatedProduct != null) {
                                UpdatedProduct.ProductId = product.ProductId;
                                db.EditProduct(UpdatedProduct);
                                logger.Info($"Product (ID: {product.ProductId}) updated.");
                            }
                        }

                    }
                    else if(choice == "8") 
                    {
                        var db = new NWConsole_48_IBFContext();
                        Console.WriteLine("Choose products to display:");
                        Console.WriteLine("1) Display all products");
                        Console.WriteLine("2) Display all discontinued products");
                        Console.WriteLine("3) Display all active (non-discontinued) products");
                        Console.WriteLine("4) Display a specific product");
                        choice = Console.ReadLine();

                        Console.WriteLine("\"q\" to quit");

                        if(choice == "1") 
                        {
                            var products = db.Products.OrderBy(p => p.ProductId);
                            foreach (Product p in products)
                            {
                                Console.WriteLine($"{p.ProductName}");
                            }
                            logger.Info($"Displaying all products");
                        }
                        else if(choice == "2") 
                        {
                            var products = db.Products.OrderBy(p => p.ProductId).Where(d => d.Discontinued == true);
                            foreach (Product p in products)
                            {
                                Console.WriteLine($"{p.ProductName}");
                            }
                            logger.Info($"Displaying discontinued products");
                        }
                        else if(choice == "3")
                        {
                            var products = db.Products.OrderBy(p => p.ProductId).Where(d => d.Discontinued == false);
                            foreach (Product p in products)
                            {
                                Console.WriteLine($"{p.ProductName}");
                            }
                            logger.Info($"Displaying all non-discontinued products");
                        }
                        else if(choice == "4") 
                        {
                            Console.WriteLine("Choose a Product to display fully:");
                            var products = db.Products.OrderBy(p => p.ProductId);
                            foreach (Product p in products)
                            {
                                Console.WriteLine($"{p.ProductId}) {p.ProductName}");
                            }
                            int numChoice = Convert.ToInt16(Console.ReadLine());

                            var selection = db.Products.OrderBy(p => p.ProductId).Where(p => p.ProductId == numChoice);
                            foreach (Product s in selection)
                            {
                                Console.WriteLine($"ID:{s.ProductId}\n" + $"Name:{s.ProductName}\n" + $"Supplier ID:{s.SupplierId}\n" +  $"Category ID:{s.CategoryId}\n" + $"Quantity Per Unit:{s.QuantityPerUnit}\n" + $"Unit Price:{s.UnitPrice}\n" + $"Units in Stock:{s.UnitsInStock}\n" + $"Units on Order:{s.UnitsOnOrder}\n" + $"Reorder Level:{s.ReorderLevel}\n" + $"Discontinued:{s.Discontinued}\n");
                                logger.Info($"Displaying {s.ProductName}");
                            }

                        }

                    }
                    Console.WriteLine();

                } while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            logger.Info("Program ended");
        }

        public static Product GetProduct(NWConsole_48_IBFContext db)
        {
            // display all blogs
            var products = db.Products.OrderBy(p => p.ProductId);
            foreach (Product p in products)
            {
                Console.WriteLine($"{p.ProductId}: {p.ProductName}");
            }
            if (int.TryParse(Console.ReadLine(), out int ProductId))
            {
                Product product = db.Products.FirstOrDefault(p => p.ProductId == ProductId);
                if (product != null)
                {
                    return product;
                }
            }
            logger.Error("Invalid Product Id");
            return null;
        }

        public static Category GetCategory(NWConsole_48_IBFContext db) 
        {
            //display all categories
            var categories = db.Categories.OrderBy(c => c.CategoryId);
            foreach (Category c in categories)
            {
                Console.WriteLine($"{c.CategoryId}: {c.CategoryName}");
            }
            if (int.TryParse(Console.ReadLine(), out int CategoryId))
            {
                Category category = db.Categories.FirstOrDefault(c => c.CategoryId == CategoryId);
                if (category != null)
                {
                    return category;
                }
            }
            logger.Error("Invalid Category Id");
            return null;
        }

        public static Product InputProduct(NWConsole_48_IBFContext db) {
            var product = new Product();
            Console.WriteLine("Enter Product Name:");
            product.ProductName = Console.ReadLine();

            Console.WriteLine("Enter Product Category ID:");

            var query = db.Categories.OrderBy(p => p.CategoryId);
            Console.ForegroundColor = ConsoleColor.Blue;
            foreach (var item in query)
            {
            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
            }
            Console.ForegroundColor = ConsoleColor.White;
            product.ProductId = Convert.ToUInt16(Console.ReadLine());

            Console.WriteLine("Enter Supplier ID:");
            var query2 = db.Suppliers.OrderBy(p => p.SupplierId);
            Console.ForegroundColor = ConsoleColor.Blue;
            foreach (var item in query2)
            {
            Console.WriteLine($"{item.SupplierId}) {item.CompanyName}");
            }
            Console.ForegroundColor = ConsoleColor.White;
            product.SupplierId = Convert.ToInt16(Console.ReadLine());

            Console.WriteLine("Enter Unit Price (Decimal):");
            product.UnitPrice = Convert.ToDecimal(Console.ReadLine());

            Console.WriteLine("Enter Quantity Per Unit:");
            product.QuantityPerUnit = Console.ReadLine();

            Console.WriteLine("Enter Units in Stock:");
            product.UnitsInStock = Convert.ToInt16(Console.ReadLine());

            Console.WriteLine("Enter Units On Order:");
            product.UnitsOnOrder = Convert.ToInt16(Console.ReadLine());

            Console.WriteLine("Enter Reorder Level (Multiple of 5)");
            product.ReorderLevel = Convert.ToInt16(Console.ReadLine());

            Console.WriteLine("Is this product discontinued?");
            product.Discontinued = Convert.ToBoolean(Console.ReadLine());

            ValidationContext context = new ValidationContext(product, null, null);
            List<ValidationResult> results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(product, context, results, true);
                if (isValid)
                    {
                        // check for unique name
                        if (db.Products.Any(p => p.ProductName == product.ProductName))
                        {
                            // generate validation error
                            logger.Error("Vaildation error: {ProductName} already exists.", product.ProductName);
                            isValid = false;
                            results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                        }
                        else
                        {
                            logger.Info("Validation passed");
                        }
                    }
                    if (!isValid)
                    {
                        foreach (var result in results)
                        {
                            logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                        }
                } 
            return product;
        }

        public static Category InputCategory(NWConsole_48_IBFContext db) {
            var category = new Category();
            Console.WriteLine("Enter Category Name:");
            category.CategoryName = Console.ReadLine();

            Console.WriteLine("Enter the Category Description:");
            category.Description = Console.ReadLine();

            ValidationContext context = new ValidationContext(category, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(category, context, results, true);
            if (isValid)
            {
                // check for unique name
                if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                {
                // generate validation error
                logger.Error("Vaildation error: {CategoryName} already exists.", category.CategoryName);
                isValid = false;
                results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                }
                else
                {
                logger.Info("Validation passed");
                }
            }
            if (!isValid)

            {
            foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
            return category;

        }

    }
}