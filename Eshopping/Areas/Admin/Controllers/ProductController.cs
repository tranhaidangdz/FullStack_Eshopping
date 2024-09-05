using Eshopping.Repository;
using Microsoft.AspNetCore.Mvc;

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
		public IActionResult Index()
		{
			//Nếu bạn muốn trả về một view từ vị trí cụ thể, bạn cũng có thể chỉ định đường dẫn view đầy đủ trong controller:
			return View("/Areas/Admin/Views/Product/Index.cshtml");

		}
	}
}
