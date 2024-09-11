using Eshopping.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Eshopping.Controllers
{
	public class AccountController : Controller
	{
		private UserManager<AppUserModel> _userManage;
		private SignInManager<AppUserModel> _signInManager;
		public AccountController(SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManage)
		{
			_signInManager = signInManager;
			_userManage = userManage;
		}
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult Create()
		{
			return View();
		}

		public async Task<IActionResult> Login()
		{
			return View();
		}

		[HttpPost]
		
		public async Task<IActionResult> Create(UserModel user)
		{
			if (ModelState.IsValid)
			{
				AppUserModel newUser = new AppUserModel { UserName = user.Username, Email = user.Email };
				IdentityResult result = await _userManage.CreateAsync(newUser);
				if(result.Succeeded)
				{
					return Redirect("/admin/products");  //hoặc NẾU ĐĂNG NHẬP THÀNH CÔNG TRẢ VỀ TRANG CHỦ :"/views/home/index.cshtml"
				}
				foreach (IdentityError error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}
			return View(user);
		}
	}
}
