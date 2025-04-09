using System;
using System.Collections.Generic;

namespace WebTrail.Models;

public partial class Product
{
    public int Product_ID { get; set; }

    public string Product_Name { get; set; } = null!;

    public int Category_Product_ID { get; set; }

    public decimal? Calories_Per100g { get; set; }

    public decimal? Protein_Per100g { get; set; }

    public decimal? Fat_Per100g { get; set; }

    public decimal? Carbs_Per100g { get; set; }

    public int? Shelf_LifeDays { get; set; }

    public string? Notes { get; set; }

    public bool? Is_Vegetarian { get; set; }

    public bool? Contains_Gluten { get; set; }

    public bool? Contains_Lactose { get; set; }

    public bool? Contains_Nuts { get; set; }

    public bool? Contains_Eggs { get; set; }

    public virtual ICollection<Hike_Food_Plan> Hike_Food_Plans { get; set; } = new List<Hike_Food_Plan>();

    public virtual ICollection<Product_Category_A> Product_Category_As { get; set; } = new List<Product_Category_A>();

    public virtual ICollection<Recipe_Ingredient> Recipe_Ingredients { get; set; } = new List<Recipe_Ingredient>();
}
