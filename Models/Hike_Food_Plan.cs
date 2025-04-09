using System;
using System.Collections.Generic;

namespace WebTrail.Models;

public partial class Hike_Food_Plan
{
    public int Hike_Food_Plan_ID { get; set; }

    public int? Hike_ID { get; set; }

    public int? Product_ID { get; set; }

    public decimal Quantity { get; set; }

    public string? Unit { get; set; }

    public virtual Hike? Hike { get; set; }

    public virtual Product? Product { get; set; }
}
