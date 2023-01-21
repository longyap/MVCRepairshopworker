using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutomotiveBookingRepair.Data;
using AutomotiveBookingRepair.Models; // because i want to use the structure
using Microsoft.EntityFrameworkCore; // for async 
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace AutomotiveBookingRepair.Controllers
{
    public class CustomerListController : Controller
    {
        // create a readonly context
        // _context link with database
        private readonly AutomotiveBookingRepairContext _context;

        //constructor, to link with my context
        public CustomerListController(AutomotiveBookingRepairContext context)
        {
            // for connection
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string SearchCustomerName,string CustomerName, string msg="")
        {
            ViewBag.msg = msg; // if receive delete msg, then display

            // find the distinct customer ID to add into the listbox
            IQueryable<string> query = from m in _context.UserData
                                       orderby m.CustomerName
                                       select m.CustomerName;
            IEnumerable<SelectListItem> list = new SelectList(await query.Distinct().ToListAsync());
            ViewBag.CustomerName = list; // to help you to bring something to frontend

            //var User = await _context.UserData.ToListAsync();
            var User = from m in _context.UserData // Query
                       select m;

            // if customer name is not empty, then filter the customerlist based on SearchCustomerName
            if (!string.IsNullOrEmpty(SearchCustomerName))
            {
                // here use .Contain --> means , during search, if contain atleast one part, then show
                User = User.Where(s => s.CustomerName.Contains(SearchCustomerName)); // filterlist

            }
            if (!string.IsNullOrEmpty(CustomerName))
            {
                // here use .Contain --> means , during search, if contain atleast one part, then show
                User = User.Where(s => s.CustomerName.Equals(CustomerName)); // filterlist

            }
            return View(await User.ToListAsync()); // bring user to frontend
        }

        [Authorize(Roles = "Admin")]
        //function: load the insert data page
        public IActionResult AddData()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken] // to avoid attack
        // use async to send multiple request to server at a time
        public async Task <IActionResult> ProcessInsert(UserData userdata1)
        {
            if (ModelState.IsValid) // if form is valid
            {
                _context.UserData.Add(userdata1);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "CustomerList");
            }
            return View(userdata1);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> deletePage(int CustomerID)
        {
            try
            {
                var User = await _context.UserData.FindAsync(CustomerID);
                _context.UserData.Remove(User);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "CustomerList", new { msg = "Customer ID" + CustomerID + "is deleted from the table now!"});
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "CustomerList", new { msg = " Error in deleting!" });
            }
        }

        [Authorize(Roles = "Admin")]
        // function: create a edit page
        public async Task<IActionResult> editPage(int ? CustomerID) // ? represent that you receive customer id or not
        {
            if(CustomerID == null)
            {
                return NotFound("Customer ID not found!");
            }
            return View(await _context.UserData.FindAsync(CustomerID));
        }

        //function: update the edit information
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> processEditPage(UserData userData1)
        {
            if (ModelState.IsValid)
            {
                _context.UserData.Update(userData1);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "CustomerList", new { msg = "Customer of ID " + userData1.CustomerID + " is updated"});
            }
            return View("editPage", userData1);
        }
    }
}
