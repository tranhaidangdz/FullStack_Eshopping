using Eshopping.Models;
using Eshopping.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Eshopping.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ProductController:Controller
	{
		private readonly DataContext _dataContext;
		public ProductController(DataContext context)
		{
			_dataContext = context;
		}
		public async Task<IActionResult> Index()
		{
			return View(await _dataContext.Products.OrderByDescending(P=>P.Id).Include(p=>p.Category).Include(p => p.Brand).ToListAsync() );

		}
		[HttpGet]
		public IActionResult Create()  //lấy ra ds danh mục và thương hiệu sp
		{
			ViewBag.Categories = new SelectList(_dataContext.Categories,"Id","Name");
			ViewBag.Brands = new SelectList(_dataContext.Brands,"Id","Name");
			return View();
		}
		public async Task<IActionResult> Create(ProductModel product)  //lấy ds và thương hiệu của form (nhận từ ng dùng ) sau đó so sánh với các sp đã có trong csdl 
		{
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name",product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name",product.BrandId);
            return View(product);
        }
	}
}
