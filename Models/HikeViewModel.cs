using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebTrail.Models
{
    public class HikeViewModel
    {
        public HikeViewModel()
        {
            Hike = new Hike();
            Recipes = new List<Recipe>();
            //TourTypes = new List<SelectListItem>();
        }
        public Hike Hike { get; set; }
        public List<Recipe> Recipes { get; set; }

    //    public List<SelectListItem> TourTypes { get; set; }
    //    public int? SelectedTourTypeID 
    //{ 
    //    get => Hike.TourTypeID;
    //    set => Hike.TourTypeID = value ?? 0;  }

    }
}
