using Eshopping.Models;
using Eshopping.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Eshopping.Areas.Admin.Controllers
{
	[Area("Admin")]

	public class ProductController:Controller
	{
		private readonly DataContext _dataContext;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductController(DataContext context,IWebHostEnvironment webHostEnvironment)
		{
			_dataContext = context;
			_webHostEnvironment = webHostEnvironment;
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

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ProductModel product)  //lấy ds và thương hiệu của form (nhận từ ng dùng ) sau đó so sánh với các sp đã có trong csdl 
		{
			ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

			if (ModelState.IsValid)
			{
				//code them du lieu san pham:
				//TempData["success"] = "Model ok hết rồi";
				product.Slug = product.Name.Replace(" ", "-");
				var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);

				if (slug != null)
				{
					ModelState.AddModelError("", "Sản phẩm này đã có trong Database");
					return View(product);
				}

				if (product.ImageUpload != null)
				{
					string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
					string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
					string filePath = Path.Combine(uploadsDir, imageName);

					FileStream fs = new FileStream(filePath, FileMode.Create);
					await product.ImageUpload.CopyToAsync(fs);
					fs.Close();
					product.Image = imageName;
				}

				_dataContext.Add(product);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Thêm sản phẩm thành công";
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
			return View(product);
		}
			public async Task<IActionResult> Edit(int Id)
			{
				ProductModel product = await _dataContext.Products.FindAsync(Id);
                ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
                ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
                return View(product);
			}

		//FORM edit:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id,ProductModel product)  //lấy ds và thương hiệu của form (nhận từ ng dùng ) sau đó so sánh với các sp đã có trong csdl 
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            if (ModelState.IsValid)
            {
                //code them du lieu san pham:
                //TempData["success"] = "Model ok hết rồi";
                product.Slug = product.Name.Replace(" ", "-");
				var slug = await _dataContext.Products.FirstOrDefaultAsync(p=>p.Slug==product.Slug);
				if (slug != null) {
					ModelState.AddModelError("","Sản phẩm đã có trong database");
					return View (product);
				}

                if (product.ImageUpload != null)
                {

                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);
                 
					FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
					product.Image = imageName;


                }


                _dataContext.Update(product);

                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật sản phẩm thành công";
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
            return View(product);
        }

        //delete sp: gửi id sp về trang edit :
        public async Task<IActionResult> Delete(int Id)
		{
            ProductModel product = await _dataContext.Products.FindAsync(Id);
			if(!string.Equals(product.Image,"noimage.jpg"))
			{
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
				string oldfileImage = Path.Combine(uploadsDir,product.Image);
				if (System.IO.File.Exists(oldfileImage))
				{
					System.IO.File.Delete(oldfileImage);
				}
            }
				_dataContext.Products.Remove(product);
				await _dataContext.SaveChangesAsync();
				TempData["error"] = "Sản phẩm đã xóa";
				return RedirectToAction("Index");
        }

    }
}
