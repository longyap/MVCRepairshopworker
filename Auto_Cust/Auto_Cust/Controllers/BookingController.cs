using Auto_Cust.Data;
using Auto_Cust.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Auto_Cust.Controllers
{
    public class BookingController : Controller
    {
        private readonly Auto_CustContext _context;

        //constructor to initialize the connection
        public BookingController(Auto_CustContext context)
        {
            _context = context;
        }

        //display list
        //public IActionResult Index()
        public async Task<IActionResult> Index()
        {
            var bookinglist = await _context.BookingTable.ToListAsync();
            return View(bookinglist);
        }

        public IActionResult AddData()
        {
            return View();
        }

        //function: processInsert data info
        [HttpPost]
        [ValidateAntiForgeryToken] //avoid cross site attack
        public async Task<IActionResult> processInsert(Booking booking)
        {
            if (ModelState.IsValid) //if form no issues proceed
            {
                _context.BookingTable.Add(booking); // add operation
                await _context.SaveChangesAsync(); // to save execute operation
                return RedirectToAction("Index", "Booking");

            }
            return View("AddData", booking); //if form got issues send back adddata page
        }
    }
}
