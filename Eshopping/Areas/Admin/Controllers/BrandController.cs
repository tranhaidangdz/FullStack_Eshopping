using Eshopping.Models;
using Eshopping.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//NÓI CHUNG LÀ PHẦN THÊM SỬA XÓA THƯƠNG HIỆU TA COPY NGUYÊN TỪ CATEGORY SANG, THAY MỖI CATEGORY THÀNH "BRAND"
namespace Eshopping.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize]
	public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Brands.OrderByDescending(p => p.Id).ToListAsync());
        }
        public async Task<IActionResult> Edit(int Id)
        {
            BrandModel brand=await _dataContext.Brands.FindAsync(Id);
            return View(brand);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }
        //form create thuong hieu:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandModel brand)  //lấy ds và thương hiệu của form (nhận từ ng dùng ) sau đó so sánh với các sp đã có trong csdl 
        {

            if (ModelState.IsValid)
            {
                //code them du lieu san pham:
                //TempData["success"] = "Model ok hết rồi";
                brand.Slug = brand.Name.Replace(" ", "-");
                var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == brand.Slug);

                if (slug != null)
                {
                    ModelState.AddModelError("", "Danh mục này đã có trong Database");
                    return View(brand);
                }

                _dataContext.Add(brand);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm thương hiệu thành công";
                return RedirectToAction("Index");

            }
            else
            {
                TempData["error"] = "Model có 1 vài thứ đang bị lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            return View(brand);
        }

        //DELETE:
        public async Task<IActionResult> Delete(int Id)
        {
            BrandModel brand = await _dataContext.Brands.FindAsync(Id);

            _dataContext.Brands.Remove(brand);
            //sau khi xóa đi category ta phải gọi hàm save change thì nó mới lưu sự thay đổi CSDL
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "thương hiệu đã xóa";
            return RedirectToAction("Index");
        }

        //EDIT brand =============================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BrandModel brand)  //lấy ds và thương hiệu của form (nhận từ ng dùng ) sau đó so sánh với các sp đã có trong csdl 
        {

            if (ModelState.IsValid)
            {
                //code them du lieu san pham:
                //TempData["success"] = "Model ok hết rồi";
                brand.Slug = brand.Name.Replace(" ", "-");
                var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == brand.Slug);

                if (slug != null)
                {
                    ModelState.AddModelError("", "Thương hiệu này đã có trong Database");
                    return View(brand);
                }

                _dataContext.Update(brand);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật thương hiệu thành công";
                return RedirectToAction("Index");

            }
            else
            {
                TempData["error"] = "Model có 1 vài thứ đang bị lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            return View(brand);
        }

    }
}
