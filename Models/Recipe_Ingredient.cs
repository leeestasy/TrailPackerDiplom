using System;
using System.Collections.Generic;

namespace WebTrail.Models;

public partial class Recipe_Ingredient
{
    public int Recipe_Ingredient_ID { get; set; }

    public int? Recipe_ID { get; set; }

    public int? Product_ID { get; set; }

    public decimal Quantity { get; set; }

    public string? Unit { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Recipe? Recipe { get; set; }
}
