using System;
using System.Collections.Generic;

namespace WebTrail.Models;

public partial class Category_Product
{
    public int Category_Product_ID { get; set; }

    public string Category_Product_Name { get; set; } = null!;

    public virtual ICollection<Product_Category_A> Product_Category_As { get; set; } = new List<Product_Category_A>();
}
