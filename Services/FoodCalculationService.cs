using WebTrail.Models;

namespace WebTrail.Services
{
    public class FoodCalculationService : IFoodCalculationService
    {
        public List<Product> _products;
        private TrailPackerDbContext dbContext;

        public FoodCalculationService(TrailPackerDbContext dbContext)
        {
            this.dbContext = dbContext;
            _products = dbContext.Products.ToList();
        }

        public class FoodRationItem
        {
            public int Category_Product_ID { get; set; }
            public int GramsPerPerson { get; set; }
        }

        public List<Product> GetProducts()
        {
            return _products;
        }

        public class RationCalculator
        {
            public List<FoodRationItem> CalculateRation(int numPeople, int numDays, int caloriesPerDay)
            {
                var baseRation = new List<FoodRationItem>();

                for (int i = 0; i < numDays; i++)
                {
                    if (caloriesPerDay <= 2000)
                    {
                        AddItems(baseRation, numPeople, [100, 0, 0, 30, 50, 30, 50, 50, 0, 20, 200, 0, 0, 0, 0, 200, 0, 0]);
                    }
                    else if (caloriesPerDay <= 2200)
                    {
                        AddItems(baseRation, numPeople, [120, 25, 20, 35, 60, 35, 60, 60, 25, 25, 250, 0, 1, 0, 0, 250, 0, 0]);
                    }
                    else if (caloriesPerDay <= 2400)
                    {
                        AddItems(baseRation, numPeople, [140, 30, 25, 40, 65, 45, 65, 65, 30, 28, 300, 0, 1, 0, 0, 280, 0, 0]);
                    }
                    else if (caloriesPerDay <= 2500)
                    {
                        AddItems(baseRation, numPeople, [150, 30, 30, 40, 70, 50, 70, 70, 30, 30, 300, 0, 1, 0, 0, 300, 0, 0]);
                    }
                    else if (caloriesPerDay <= 2800)
                    {
                        AddItems(baseRation, numPeople, [180, 40, 35, 50, 85, 60, 90, 90, 40, 35, 350, 0, 1, 0, 0, 350, 0, 0]);
                    }
                    else if (caloriesPerDay <= 3000)
                    {
                        AddItems(baseRation, numPeople, [200, 40, 40, 60, 90, 70, 100, 100, 40, 40, 400, 0, 1, 0, 0, 400, 0, 0]);
                    }
                    else
                    {
                        AddItems(baseRation, numPeople, [250, 50, 50, 100, 110, 90, 130, 130, 50, 50, 500, 0, 1, 0, 0, 500, 0, 0]);
                    }
                }

                return baseRation;
            }

            private void AddItems(List<FoodRationItem> ration, int numPeople, int[] gramsPerPerson)
            {
                int[] productIds = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };

                for (int i = 0; i < productIds.Length; i++)
                {
                    ration.Add(new FoodRationItem { Category_Product_ID = productIds[i], GramsPerPerson = gramsPerPerson[i] });
                }
            }
        }

        public Dictionary<string, double> CalculateTotalFood(int numPeople, int numDays, int typeHikeId)
        {
            double caloriesPerPersonPerDay = GetCaloriesByTypeHike(typeHikeId);
            RationCalculator calculator = new RationCalculator();
            List<FoodRationItem> baseRation = calculator.CalculateRation(numPeople, numDays, (int)caloriesPerPersonPerDay);

            var totalFood = new Dictionary<string, double>();

            foreach (var rationItem in baseRation)
            {
                var product = _products.FirstOrDefault(p => p.Category_Product_ID == rationItem.Category_Product_ID);
                if (product != null)
                {
                    string productName = product.Product_Name;
                    double totalGrams = rationItem.GramsPerPerson * numPeople * numDays; // Умножаем на количество людей и дней

                    if (totalFood.ContainsKey(productName))
                    {
                        totalFood[productName] += totalGrams;
                    }
                    else
                    {
                        totalFood[productName] = totalGrams;
                    }
                }
            }

            totalFood["Соль"] = 50; // грамм (на всю группу)
            totalFood["Чай"] = 30; // грамм (на всю группу)
            totalFood["Специи"] = 20; // грамм (на всю группу)

            return totalFood;
        }

        private double GetCaloriesByTypeHike(int typeHikeId)
        {
            Dictionary<int, double> hikeTypeCalories = new Dictionary<int, double>()
            {
                { 1, 2500 }, // Пеший
                { 2, 3000 }, // Горный
                { 3, 2000 }, // Водный
                { 4, 2200 }, // Велосипедный
                { 5, 2800 }, // Лыжный
                { 6, 2400 }, // Кемпинг
                { 7, 3500 }  // Экстремальный
            };

            if (hikeTypeCalories.ContainsKey(typeHikeId))
            {
                return hikeTypeCalories[typeHikeId];
            }
            else
            {
                return 2500; 
            }
        }

        public double CalculateTotalWeight(List<FoodRationItem> ration)
        {
            double totalWeight = 0;

            foreach (var item in ration)
            {
                totalWeight += item.GramsPerPerson;
            }

            return totalWeight;
        }

        public Dictionary<string, object> CalculateFood(Hike hike)
        {
            var results = new Dictionary<string, object>();
            results["totalDays"] = hike.Num_Days;
            results["totalPeople"] = hike.Num_People;
            results["tourType"] = hike.TourTypeID;
            results["dietaryRestrictions"] = hike.Dietary_Restrictions ?? "";

            double totalWeight = CalculateTotalWeight(GetBaseRation(GetCaloriesByTypeHike(hike.TourTypeID)));
            results["totalWeight"] = totalWeight;

            return results;
        }
        private List<FoodRationItem> GetBaseRation(double caloriesPerPersonPerDay)
        {
            // Базовая раскладка для определенной калорийности
            var baseRation = new List<FoodRationItem>();

            if (caloriesPerPersonPerDay <= 2200)
            {
                baseRation.Add(new FoodRationItem { Category_Product_ID = 28, GramsPerPerson = 80 }); // Овсянка (ID 28)
                baseRation.Add(new FoodRationItem { Category_Product_ID = 48, GramsPerPerson = 20 }); // Сухое молоко (ID 48)
                baseRation.Add(new FoodRationItem { Category_Product_ID = 8, GramsPerPerson = 20 }); // Сахар (ID 8)
                baseRation.Add(new FoodRationItem { Category_Product_ID = 22, GramsPerPerson = 30 }); // Изюм (ID 22)
                baseRation.Add(new FoodRationItem { Category_Product_ID = 4, GramsPerPerson = 50 }); // Галеты (ID 4)
                baseRation.Add(new FoodRationItem { Category_Product_ID = 47, GramsPerPerson = 30 }); // Сыр (ID 47)
                baseRation.Add(new FoodRationItem { Category_Product_ID = 41, GramsPerPerson = 30 }); // Колбаса (ID 41)
                baseRation.Add(new FoodRationItem { Category_Product_ID = 24, GramsPerPerson = 30 }); // Орехи грецкие (ID 24)
                baseRation.Add(new FoodRationItem { Category_Product_ID = 15, GramsPerPerson = 20 }); // Шоколад (ID 15)
                baseRation.Add(new FoodRationItem { Category_Product_ID = 29, GramsPerPerson = 100 }); // Рис (ID 29)
                baseRation.Add(new FoodRationItem { Category_Product_ID = 39, GramsPerPerson = 40 }); // Мясо сублимированное (ID 39)
                baseRation.Add(new FoodRationItem { Category_Product_ID = 60, GramsPerPerson = 10 }); // Грибы сушеные (ID 60)

            }
            else
            {
                baseRation.Add(new FoodRationItem { Category_Product_ID = 28, GramsPerPerson = 120 }); // Овсянка (ID 28)
                baseRation.Add(new FoodRationItem { Category_Product_ID = 48, GramsPerPerson = 30 }); // Сухое молоко (ID 48)
                baseRation.Add(new FoodRationItem { Category_Product_ID = 8, GramsPerPerson = 30 }); // Сахар (ID 8)
                baseRation.Add(new FoodRationItem { Category_Product_ID = 22, GramsPerPerson = 50 }); // Изюм (ID 22)
                baseRation.Add(new FoodRationItem { Category_Product_ID = 4, GramsPerPerson = 120 }); // Галеты (ID 4)
                baseRation.Add(new FoodRationItem { Category_Product_ID = 47, GramsPerPerson = 40 }); // Сыр (ID 47)
                baseRation.Add(new FoodRationItem { Category_Product_ID = 41, GramsPerPerson = 40 }); // Колбаса (ID 41)
                baseRation.Add(new FoodRationItem { Category_Product_ID = 24, GramsPerPerson = 40 }); // Орехи грецкие (ID 24)
                baseRation.Add(new FoodRationItem { Category_Product_ID = 15, GramsPerPerson = 30 }); // Шоколад (ID 15)
                baseRation.Add(new FoodRationItem { Category_Product_ID = 29, GramsPerPerson = 150 }); // Рис (ID 29)
                baseRation.Add(new FoodRationItem { Category_Product_ID = 39, GramsPerPerson = 60 }); // Мясо сублимированное (ID 39)
                baseRation.Add(new FoodRationItem { Category_Product_ID = 60, GramsPerPerson = 20 }); // Грибы сушеные (ID 60)
            }

            return baseRation;
        }

    }
}
