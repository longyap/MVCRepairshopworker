using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MVCRepairshopworker.Models;
namespace MVCRepairshopworker.Controllers
{
    public class VehicleController : Controller
    {
      //  public VehicleControllerController( context)
       // {
        //    _context = context;
       // }
        public async Task<IActionResult> processeditPage(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
       //         _context.Flower.Update(flower);
         //       await _context.SaveChangesAsync();
           ///     return RedirectToAction("Index", "Flowers",
              //      new { msg = "Flower information of id" + flower.FlowerID + "has updated" });
            }
           return View("editAction");


        }
        public async Task<IActionResult> processInsert(int VehicleID)
        {
            return RedirectToAction(nameof(deleteAction));
        }
        public async Task<IActionResult> editAction(int VehicleID)
        {
            return RedirectToAction(nameof(deleteAction));
        }
        public async Task<IActionResult> deleteAction(int VehicleID)
        {
            return RedirectToAction(nameof(deleteAction));
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
