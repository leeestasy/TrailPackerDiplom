using WebTrail.Models;

namespace WebTrail.Services
{
    public interface IFoodCalculationService
    {
        Dictionary<string, double> CalculateTotalFood(int numPeople, int numDays, int typeHikeId);
        Dictionary<string, object> CalculateFood(Hike hike);
        List<Product> GetProducts();
    }
}
