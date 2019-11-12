 using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class Coupon
    {
        [Key]
        [Display(Name = "Id")]
        public int CouponId { get; set; }

        [Display(Name = "Coupon Name")]
        [Required]
        public string CouponName { get; set; }
        
        [Display(Name = "Coupon Type")]
        [Required]
        public string CouponType { get; set; }
        public enum ECouponType {Percent = 0, Dollar = 1 }
        [Required]
        public double Discount { get; set; }

        [Display(Name = "Minimum Amount")]
        [Required]
        public double MinimumAmount { get; set; }
        public byte[] CouponImage { get; set; }
        public bool IsActive { get; set; }

    }
}
