using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Reflection.Metadata;
using WebTrail.Models;
using WebTrail.Services;

namespace WebTrail.Controllers
{
    public class HomeController : Controller
    {
        private readonly TrailPackerDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly IFoodCalculationService _foodCalculationService;

        public HomeController(TrailPackerDbContext context, IFoodCalculationService foodCalculationService, ILogger<HomeController> logger)
        {
            _context = context;
            _foodCalculationService = foodCalculationService;
            _logger = logger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Hike hike)
        {
            if (ModelState.IsValid)
            {
                _context.Hikes.Add(hike);
                await _context.SaveChangesAsync();

                var results = _foodCalculationService.CalculateFood(hike);
                SetViewBagMessages(results, "Данные успешно сохранены!");

                return RedirectToAction("ResultFirst");
            }

            SetTripTypes(hike.TourTypeID);
            return View(await CreateHikeViewModelAsync());
        }

        public async Task<IActionResult> Index()
        {
            SetTripTypes();
            return View(await CreateHikeViewModelAsync());
        }

        public async Task<IActionResult> Recipes(string searchTerm = null)
        {
            var viewModel = await GetHikeViewModelAsync(searchTerm);
            return View(viewModel);
        }

        private async Task<HikeViewModel> CreateHikeViewModelAsync()
        {
            return new HikeViewModel
            {
                Hike = new Hike(), 
                Recipes = await _context.Recipes.ToListAsync()
            };
        }

        private void SetTripTypes(int? selectedTripType = null)
        {
            var tripTypes = _context.TourTypes.ToList();
            ViewBag.TripTypes = new SelectList(tripTypes, "TourTypeID", "TypeName", selectedTripType);
            _logger.LogInformation("TripTypes: {@TripTypes}", tripTypes);
        }

        private void SetViewBagMessages(object results, string message)
        {
            _logger.LogInformation("Results: {@Results}", results);
            ViewBag.Results = results;
            ViewBag.Message = message;
        }

        public async Task<HikeViewModel> GetHikeViewModelAsync(string searchTerm)
        {
            var recipesQuery = _context.Recipes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                recipesQuery = recipesQuery.Where(r => r.Recipe_Name.ToLower().Contains(lowerSearchTerm));
            }

            var recipes = await recipesQuery.ToListAsync();

            return new HikeViewModel
            {
                Hike = new Hike(),
                Recipes = recipes
            };
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ResultFirst(Hike_Food_Plan model)
        {
            if (model == null || model.Hike == null)
            {
                _logger.LogError("Данные не переданы.");
                return BadRequest("Неверные данные запроса.");
            }

            var totalFood = _foodCalculationService.CalculateTotalFood(model.Hike.Num_People, model.Hike.Num_Days, model.Hike.TourTypeID);
            var results = new FoodCalculationResults();
            double totalWeightKg = 0; // Общий вес в кг

            var products = _foodCalculationService.GetProducts();
            if (products == null || !products.Any())
            {
                _logger.LogWarning("Список продуктов пуст.");
                ViewBag.Results = results; 
                return View("ResultFirst");
            }

            foreach (var item in totalFood)
            {
                var product = products.FirstOrDefault(p => p.Product_Name == item.Key);
                if (product != null)
                {
                    var productViewModel = new ProductViewModel
                    {
                        Name = item.Key,
                        Quantity = 1,
                        Weight = item.Value / 1000.0 // Преобразуем граммы в кг
                    };
                    results.Products.Add(productViewModel);
                    totalWeightKg += productViewModel.Weight;
                }
                else
                {
                    _logger.LogWarning($"Продукт не найден: {item.Key}");
                }
            }

            results.TotalWeight = totalWeightKg;
            ViewBag.Results = results;

            return View("ResultFirst", results); 
        }

        public IActionResult ResultSecond()
        {
            return View();
        }  

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecipesPost(string searchTerm)
        {
            var viewModel = await GetHikeViewModelAsync(searchTerm);
            return View("Recipes", viewModel);
        }
 
        public IActionResult RecipeView(int id)
        {
            var recipe = _context.Recipes
                .Include(r => r.Recipe_Ingredients)
                .ThenInclude(ri => ri.Product)
                .FirstOrDefault(r => r.Recipe_ID == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        public IActionResult DownloadPdf()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdfDocument = new PdfDocument(writer);
                iText.Layout.Document document = new iText.Layout.Document(pdfDocument);

                string fontPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", "Arial.ttf"); 

                PdfFont font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);

                // Текст сообщения (кириллица)
                string message = "В данный момент приложение находится на стадии разработки.\nСпасибо за интерес к нашему продукту!";

                Paragraph paragraph = new Paragraph(message);
                paragraph.SetFont(font);

                paragraph.SetTextAlignment(TextAlignment.CENTER);

                document.Add(paragraph);

                document.Close();

                string contentType = "application/pdf";
                string fileName = "development.pdf"; 
                return File(ms.ToArray(), contentType, fileName);
            }
        }
    }
}