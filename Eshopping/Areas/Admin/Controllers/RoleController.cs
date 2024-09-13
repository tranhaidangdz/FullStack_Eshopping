using Eshopping.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eshopping.Areas.Admin.Controllers
{
        [Area("Admin")]
        [Authorize("Admin/Role")]
        [Authorize(Roles = "Admin")]

    public class RoleController : Controller
    {
        private readonly DataContext _dataContext;
        public RoleController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Roles.OrderByDescending(p=>p.Id).ToListAsync());
        }
    }
}
