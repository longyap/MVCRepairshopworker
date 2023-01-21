using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AutomotiveBookingRepair.Models
{
    public class UserData
    {
        [Key]
        public int CustomerID { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string CustomerEmail { get; set; }

        [Required]
        public string CustomerAddress { get; set; }

        [Required]
        public DateTime CustomerDOB { get; set; }
    }
}
