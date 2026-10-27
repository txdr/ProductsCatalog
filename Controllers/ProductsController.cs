using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Models;
using System.Text.Json;

namespace ProductCatalog.Controllers
{
    public class ProductsController : Controller
    {
        // Class Variables
        private readonly string _dataDir;
        private readonly string _dataFile;
        private readonly JsonSerializerOptions _json = new() { WriteIndented = true };

        // Constructor
        public ProductsController(IWebHostEnvironment env)
        {
            _dataDir = Path.Combine(env.ContentRootPath, "data");
            _dataFile = Path.Combine(_dataDir, "products.json");
            Directory.CreateDirectory(_dataDir);
        }

        // List of Products
        private List<Product> Load()
        {
            if (!System.IO.File.Exists(_dataFile)) return new();
            var json = System.IO.File.ReadAllText(_dataFile);
            return JsonSerializer.Deserialize<List<Product>>(json) ?? new();
        }

        // Method to Save
        private void Save(List<Product> products)
        {
            var json = JsonSerializer.Serialize(products);
            System.IO.File.WriteAllText(_dataFile, json);
        }

        // Method to Index
        public IActionResult Index()
        {
            var products = Load();
            return View(products);
        }

        // Details
        public IActionResult Details(int id)
        {
            var product = Load().FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        // Create Method
        [HttpGet]
        public IActionResult Create() => View();

        public IActionResult Create(Product product)
        {
            // Validate the Model
            if (!ModelState.IsValid) return View(product);

            // Add to the list and the JSON
            var products = Load();
            product.Id = products.Count == 0 ? 1 : products.Max(p => p.Id) + 1;
            products.Add(product);
            Save(products);

            // Return to the Index
            return RedirectToAction(nameof(Index));
        }

    }
}
