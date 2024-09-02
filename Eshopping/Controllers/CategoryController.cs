using Microsoft.AspNetCore.Mvc;

namespace Eshopping.Controllers
{
    public class CategoryController:Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
