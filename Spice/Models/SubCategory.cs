using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class SubCategory
    {
        [Key]
        [Display(Name ="SubCategoryId")]
        public int SubCategoryId { get; set; }

        [Required]
        [Display(Name ="SubCategoryName")]
        public string SubCategoryName { get; set; }

        [Display(Name = "Category")]
        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
