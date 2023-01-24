using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Auto_Cust.Data;
using Auto_Cust.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Auto_Cust.Controllers
{
    public class CustAutoController : Controller
    {
        //create a variable to link with db
        private readonly Auto_CustContext _context;

        //constructor to initialize the connection
        public CustAutoController(Auto_CustContext context)
        {
            _context = context;
        }

        //fucntion use to display flower list
        //public IActionResult Index()
        public async Task<IActionResult> Index(string msg = "")
        {
            ViewBag.msg = msg;

            var cusautolist = await _context.CustomerAutomotive.ToListAsync();
            return View(cusautolist);
        }

        //function to load insert page
        public IActionResult AddData()
        {
            return View();
        }

        //function: processInsert data info
        [HttpPost]
        [ValidateAntiForgeryToken] //avoid cross site attack
        public async Task<IActionResult> processInsert(cus_automotive cus_Automotive)
        {
            if (ModelState.IsValid) //if form no issues proceed
            {
                _context.CustomerAutomotive.Add(cus_Automotive); // add operation
                await _context.SaveChangesAsync(); // to save execute operation
                return RedirectToAction("Index", "CustAuto");

            }
            return View("AddData", cus_Automotive); //if form got issues send back adddata page
        }

        public async Task<IActionResult> deleteAction(int CustomerAutoID)
        {
            if (CustomerAutoID == null)
            {
                return NotFound();
            }

            try
            {
                var customerautomotive = await _context.CustomerAutomotive.FindAsync(CustomerAutoID);
                _context.CustomerAutomotive.Remove(customerautomotive);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "cus_automotive",
                    new { msg = "Customer Automotive ID" + CustomerAutoID + "is deleted from the table!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public async Task<IActionResult> editAction(int? CustomerAutoID)
        {
            if (CustomerAutoID == null)
            {
                return NotFound();
            }
            return View(await _context.CustomerAutomotive.FindAsync(CustomerAutoID));//linq
        }

        public async Task<IActionResult> processeditPage(cus_automotive cus_Automotive)
        {
            if (ModelState.IsValid)
            {
                _context.CustomerAutomotive.Update(cus_Automotive);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "cus_automotive",
                    new { msg = "Automotive information of id" + cus_Automotive.CustomerAutoID + "has updated" });
            }
            return View("editAction", cus_Automotive);
        }
    }
}

