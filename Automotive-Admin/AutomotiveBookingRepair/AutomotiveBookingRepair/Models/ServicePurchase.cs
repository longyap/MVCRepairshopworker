using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomotiveBookingRepair.Models
{
    public class ServicePurchase
    {
        // partition key
        public string CustomerName { get; set; }

        // sort key
        public string TransactionID { get; set; }

        public string PurchaseService { get; set; }

        public decimal PurchaseAmount { get; set; }

        public DateTime PurchaseDate { get; set; }

        public string PurchaseMethod { get; set; }

        public DateTime PaymentDate { get; set; } // optional - flexible
    }
}
