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
                    //Edit categories
                    //Display all Categories in the Categories table (CategoryName and Description)
                    //Display all Categories and their related active (not discontinued) product data (CategoryName, ProductName)
                    //Display a specific Category and its related active product data (CategoryName, ProductName)
                    Console.WriteLine("3) Display Category and related products");
                    Console.WriteLine("4) Display all Categories and their related products");
                    Console.WriteLine("5) Add Product");
                    Console.WriteLine("6) Edit Product");
                    //Display all product records
                    //Display a specific product

                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");
                    if (choice == "1")
                    {
                        var db = new NWConsole_48_IBFContext();
                        var query = db.Categories.OrderBy(p => p.CategoryName);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{query.Count()} records returned");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName} - {item.Description}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (choice == "2")
                    {
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
                            var db = new NWConsole_48_IBFContext();
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
                                db.AddCategory(category);
                                logger.Info("Category added - {name}", category.CategoryName);

                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    }
                    else if (choice == "3")
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
                    else if (choice == "4")
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
                    else if(choice == "5") {

                        var db = new NWConsole_48_IBFContext();
                        Product product = InputProduct(db);
                        if(product != null) {
                            db.AddProduct(product);
                            logger.Info("Product added - {name}", product.ProductName);
                        }

                    }
                    else if(choice == "6") 
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
                                Console.WriteLine("Product updated.");
                            }
                        }

                        //to do: Add in editing of specific product

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
            logger.Error("Invalid Blog Id");
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
                            db.AddProduct(product);
                            logger.Info("Product added - {name}", product.ProductName);
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

    }
}