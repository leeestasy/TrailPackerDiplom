namespace WebTrail.Models
{
    public class HikeViewModel
    {
        public Hike Hike { get; set; } = new Hike();
        public required List<Recipe> Recipes { get; set; }
    }
}
