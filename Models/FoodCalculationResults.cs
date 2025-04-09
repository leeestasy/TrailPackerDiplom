namespace WebTrail.Models
{
    public class FoodCalculationResults
    {
        private List<ProductViewModel> products;

        public List<ProductViewModel> Products { get => products; set => products = value; }
        public double TotalWeight { get; set; }

        public FoodCalculationResults() => Products = new List<ProductViewModel>();
    }
}
