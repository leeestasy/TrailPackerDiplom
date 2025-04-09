using System;
using System.Collections.Generic;

namespace WebTrail.Models;

public partial class Hike_Recipe_Suggestion
{
    public int Hike_Recipe_Suggestions_ID { get; set; }

    public int? Hike_ID { get; set; }

    public int? Recipe_ID { get; set; }

    public int? Day { get; set; }

    public string? Meal_Type { get; set; }

    public virtual Hike? Hike { get; set; }

    public virtual Recipe? Recipe { get; set; }
}
