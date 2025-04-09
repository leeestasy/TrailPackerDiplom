using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection.Metadata;
using WebTrail.Models;

namespace WebTrail.Controllers
{
    public class RecipeController : Controller
    {
        private readonly TrailPackerDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public RecipeController(TrailPackerDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string searchTerm)
        {
            var recipesQuery = _context.Recipes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();

                recipesQuery = recipesQuery.Where(r => r.Recipe_Name.ToLower().Contains(lowerSearchTerm));
            }

            var viewModel = new HikeViewModel
            {
                Recipes = await recipesQuery.ToListAsync() 
            };

            return View(viewModel);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Recipes()
        {
            return View();
        }

        public IActionResult RecipeView()
        {
            return View();
        }
    }

}