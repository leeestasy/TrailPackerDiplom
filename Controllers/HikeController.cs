using Microsoft.AspNetCore.Mvc;
using System.IO;
using WebTrail.Models;

namespace WebTrail.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HikeController : ControllerBase
    {
        private object CalculateFood(int numDays, int numPeople, string? hikeType, string? dietaryRestrictions)
        {
            // Здесь будет ваша логика расчета
            return new { FoodItems = "Продукты", Quantity = 100 }; // пример возврата
        }
    }
}
