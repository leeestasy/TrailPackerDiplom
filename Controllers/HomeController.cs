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
            _logger.LogInformation("������ ������ Index (HttpPost)");
            _logger.LogInformation("�������� Hike �� ���������: {@Hike}", hike);

            foreach (var key in ModelState.Keys)
            {
                _logger.LogInformation($"ModelState[{key}].RawValue: {ModelState[key].RawValue}");
            }

            // �������� TourType
            if (hike.TourTypeID.HasValue)
            {
                hike.TourType = await _context.TourTypes.FindAsync(hike.TourTypeID);
                if (hike.TourType == null)
                {
                    ModelState.AddModelError("TourTypeID", "��������� ��� ������ �� ����������.");
                }
            }

            if (ModelState.IsValid)
            {
                _logger.LogInformation("ModelState.IsValid == true");

                _context.Hikes.Add(hike);
                await _context.SaveChangesAsync();

                return RedirectToAction("ResultFirst", new { hikeId = hike.Hike_ID });
            }
            else
            {
                _logger.LogError("ModelState.IsValid == false");
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    if (errors.Any())
                    {
                        _logger.LogError($"Validation errors for key: {key}");
                        foreach (var error in errors)
                        {
                            _logger.LogError($"  - {error.ErrorMessage}");
                        }
                    }
                }
            }

            SetTripTypes(hike.TourTypeID);
            return View(await CreateHikeViewModelAsync(hike));
        }

        [HttpGet]
        public IActionResult ResultFirst(int hikeId)
        {
            var hike = _context.Hikes.FirstOrDefault(h => h.Hike_ID == hikeId);
            if (hike == null)
            {
                _logger.LogError("Hike �� ������.");
                return BadRequest("������ �� �������.");
            }

            var tourTypeID = hike.TourTypeID ?? 0;
            var totalFood = _foodCalculationService.CalculateTotalFood(hike.Num_People, hike.Num_Days, tourTypeID);
            var products = _foodCalculationService.GetProducts();
            var results = new FoodCalculationResults();
            double totalWeightKg = 0;

            if (products == null || !products.Any())
            {
                _logger.LogWarning("������ ��������� ����.");
                return View("ResultFirst", results); // ���������� ������ ������ FoodCalculationResults
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
                        Weight = item.Value / 1000.0 // ����������� ������ � ��
                    };
                    results.Products.Add(productViewModel);
                    totalWeightKg += productViewModel.Weight;
                }
                else
                {
                    _logger.LogWarning($"������� �� ������: {item.Key}");
                }
            }

            results.TotalWeight = totalWeightKg;

            // ������� ������ FoodCalculationResults � �������������
            return View("ResultFirst", results); // ������� ��������� � �������������
        }

        private async Task<HikeViewModel> CreateHikeViewModelAsync(Hike hike = null)
        {
            return new HikeViewModel
            {
                Hike = hike ?? new Hike(),
                Recipes = await _context.Recipes.ToListAsync()
            };
        }

        private async Task<Hike_Food_Plan> CreateHike_Food_Plan_ViewModelAsync(Hike hike)
        {
            return new Hike_Food_Plan
            {
                Hike = hike
            };
        }

        private void SetTripTypes(int? selectedId = null)
        {
            ViewBag.TripTypes = new SelectList(_context.TourTypes, "TourTypeID", "TourType_Name", selectedId);
        }

        public async Task<IActionResult> Index()
        {
            var tripTypes = await _context.TourTypes.ToListAsync(); // �������� ������
            ViewBag.TripTypes = new SelectList(tripTypes, "TourTypeID", "TypeName");
            _logger.LogInformation("TripTypes count: {Count}", tripTypes.Count); // �������� ����������
            return View(await CreateHikeViewModelAsync());
            //SetTripTypes();
            //return View(await CreateHikeViewModelAsync());
        }

        public async Task<IActionResult> Recipes(string searchTerm = null)
        {
            var viewModel = await GetHikeViewModelAsync(searchTerm);
            return View(viewModel);
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

        //public IActionResult ResultFirst(Hike_Food_Plan model)
        //{
        //    if (model == null || model.Hike == null)
        //    {
        //        _logger.LogError("������ �� ��������.");
        //        return BadRequest("�������� ������ �������.");
        //    }
        //    var tourTypeID = model.Hike.TourTypeID ?? 0; // ���������� 0, ���� TourTypeID null

        //    var totalFood = _foodCalculationService.CalculateTotalFood(model.Hike.Num_People, model.Hike.Num_Days, tourTypeID);
        //    var results = new FoodCalculationResults();
        //    double totalWeightKg = 0; // ����� ��� � ��

        //    var products = _foodCalculationService.GetProducts();
        //    if (products == null || !products.Any())
        //    {
        //        _logger.LogWarning("������ ��������� ����.");
        //        ViewBag.Results = results; 
        //        return View("ResultFirst");
        //    }

        //    foreach (var item in totalFood)
        //    {
        //        var product = products.FirstOrDefault(p => p.Product_Name == item.Key);
        //        if (product != null)
        //        {
        //            var productViewModel = new ProductViewModel
        //            {
        //                Name = item.Key,
        //                Quantity = 1,
        //                Weight = item.Value / 1000.0 // ����������� ������ � ��
        //            };
        //            results.Products.Add(productViewModel);
        //            totalWeightKg += productViewModel.Weight;
        //        }
        //        else
        //        {
        //            _logger.LogWarning($"������� �� ������: {item.Key}");
        //        }
        //    }

        //    results.TotalWeight = totalWeightKg;
        //    ViewBag.Results = results;

        //    return View("ResultFirst", results); 
        //}

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

                // ����� ��������� (���������)
                string message = "� ������ ������ ���������� ��������� �� ������ ����������.\n������� �� ������� � ������ ��������!";

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