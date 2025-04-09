using System;
using System.Collections.Generic;

namespace WebTrail.Models;

public partial class Recipe_Category_A
{
    public int Recipe_Category_As_ID { get; set; }

    public int Category_Recipe_ID { get; set; }

    public int Recipe_ID { get; set; }

    public virtual Category_Recipe Category_Recipe { get; set; } = null!;

    public virtual Recipe Recipe { get; set; } = null!;
}
