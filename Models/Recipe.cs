using System;
using System.Collections.Generic;

namespace WebTrail.Models;

public partial class Recipe
{
    public int Recipe_ID { get; set; }

    public string Recipe_Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Instructions { get; set; }

    public int? Category_Recipe_ID { get; set; }

    public bool? Is_Vegetarian { get; set; }

    public bool? Is_Gluten_Free { get; set; }

    public bool? Is_Lactose_Free { get; set; }

    public bool? Contains_Nuts { get; set; }

    public bool? Contains_Eggs { get; set; }

    public virtual ICollection<Hike_Recipe_Suggestion> Hike_Recipe_Suggestions { get; set; } = new List<Hike_Recipe_Suggestion>();

    public virtual ICollection<Recipe_Category_A> Recipe_Category_As { get; set; } = new List<Recipe_Category_A>();

    public virtual ICollection<Recipe_Ingredient> Recipe_Ingredients { get; set; } = new List<Recipe_Ingredient>();
}
