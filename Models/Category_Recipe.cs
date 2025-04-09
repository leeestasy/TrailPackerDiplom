using System;
using System.Collections.Generic;

namespace WebTrail.Models;

public partial class Category_Recipe
{
    public int Category_Recipe_ID { get; set; }

    public string Category_Recipe_Name { get; set; } = null!;

    public virtual ICollection<Recipe_Category_A> Recipe_Category_As { get; set; } = new List<Recipe_Category_A>();
}
