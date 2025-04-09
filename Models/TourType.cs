using System;
using System.Collections.Generic;

namespace WebTrail.Models;

public partial class TourType
{
    public int TourTypeID { get; set; }

    public string? TypeName { get; set; }

    public int? AvgEnergyConsumption { get; set; }

    public virtual ICollection<Hike> Hikes { get; set; } = new List<Hike>();
}
