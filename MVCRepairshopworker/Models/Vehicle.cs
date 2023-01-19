using System.ComponentModel.DataAnnotations;
using System;

namespace MVCRepairshopworker.Models
{
    public class Vehicle
    {
        [Key]
        public int VehicleID { get; set; }

        public string VehicleName { get; set; }

        public string VehicleType { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal RepairCost { get; set; }
    
    }
}
