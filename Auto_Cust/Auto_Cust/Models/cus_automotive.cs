using System;
using System.ComponentModel.DataAnnotations;

namespace Auto_Cust.Models
{
    public class cus_automotive
    {
        [Key]
        public int CustomerAutoID { get; set; }

        [Required]
        [Display(Name = "Automotive Brand")]
        public string CustomerAutoBrand { get; set; }

        [Required]
        [Display(Name = "Automotive Color")]
        public string CustomerAutoColor { get; set; }

        [Required]
        [Display(Name = "Automotive Register Date")]
        [DataType(DataType.Date)]
        public DateTime CustomerAutoRegisterDate { get; set; }
    }
}
