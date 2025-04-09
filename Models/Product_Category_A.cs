using System;
using System.Collections.Generic;

namespace WebTrail.Models;

public partial class Product_Category_A
{
    public int Product_Category_As_ID { get; set; }

    public int Product_ID { get; set; }

    public int Category_Product_ID { get; set; }

    public virtual Category_Product Category_Product { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
