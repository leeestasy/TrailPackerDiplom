using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebTrail.Models
{
    public class Hike
    {
        [Key]
        public int Hike_ID { get; set; }

        [Required(ErrorMessage = "Требуется указать количество дней.")]
        [Range(1, 365, ErrorMessage = "Количество дней должно быть между 1 и 365.")]
        [Display(Name = "Количество дней")]
        public int Num_Days { get; set; }

        [Required(ErrorMessage = "Требуется указать количество человек.")]
        [Range(1, 50, ErrorMessage = "Количество человек должно быть между 1 и 50.")]
        [Display(Name = "Количество человек")]
        public int Num_People { get; set; }

        [Required(ErrorMessage = "Требуется указать вид похода.")]
        [Display(Name = "Тип похода")]
        public int? TourTypeID { get; set; }

        [StringLength(200, ErrorMessage = "Пищевые особенности (аллергии) не должны превышать 200 символов.")]
        [Display(Name = "Пищевые особенности (аллергии)")]
        public string? Dietary_Restrictions { get; set; }

        public DateOnly? Generated_At { get; set; }

        public virtual ICollection<Hike_Food_Plan> Hike_Food_Plans { get; set; } = new List<Hike_Food_Plan>();

        public virtual ICollection<Hike_Recipe_Suggestion> Hike_Recipe_Suggestions { get; set; } = new List<Hike_Recipe_Suggestion>();

        [ValidateNever]
        public virtual TourType? TourType { get; set; }
    }
}