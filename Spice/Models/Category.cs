using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class Category
    {
        [Display(Name ="Category Id")]
        [Key]
        public int CategoryId { get; set; }
        [Display(Name = "Category Name")]
        [Required]
        public string CategoryName { get; set; }
    }
}
