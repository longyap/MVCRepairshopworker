using System;
using System.ComponentModel.DataAnnotations;

namespace Auto_Cust.Models
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        [Required]
        public string Booking_CustomerAutoBrand { get; set; }

        [Required]
        public string Booking_CustomerAutoColor { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        public string Services { get; set; }

    }
}
