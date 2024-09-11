using Eshopping.Models;
using Eshopping.Models.ViewModels;
using Eshopping.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

namespace Eshopping.Controllers
{
	public class CheckoutController : Controller
	{
		private readonly DataContext _dataContext;

		public CheckoutController(DataContext context)
		{
			_dataContext = context;
		}

		public async Task<IActionResult> Checkout()
		{
			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			if (userEmail == null)
			{
				return RedirectToAction("Login", "Account");
			}
			else
			{
				// Thêm dữ liệu vào bảng Orders
				var ordercode = Guid.NewGuid().ToString(); // Dùng để random mã đơn hàng 
				var orderItem = new OrderModel();
				orderItem.OrderCode = ordercode;
				orderItem.UserName = userEmail;
				orderItem.Status = 1; // 1 có nghĩa là đơn hàng mới
				orderItem.CreateDate = DateTime.Now;
				_dataContext.Add(orderItem); // Thêm dữ liệu
				_dataContext.SaveChanges();

				List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
				foreach (var cart in cartItems)
				{
					var orderdetails = new OrderDetails();
					orderdetails.UserName = userEmail;
					orderdetails.OrderCode = ordercode;
					orderdetails.ProductId = cart.ProductId;
					orderdetails.Price = cart.Price;
					orderdetails.Quantity = cart.Quantity;

					_dataContext.Add(orderdetails);
					_dataContext.SaveChanges();
				}

				HttpContext.Session.Remove("Cart");
				TempData["success"] = "Checkout thành công, vui lòng chờ đơn hàng được duyệt";
				return View("Index", "Cart");

			}
			return View();
		}
	}
}
