﻿using Eshopping.Areas.Admin.Repository;
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
		private readonly IEmailSender _emailSender;  //gọi đối tg này từ lớp IEmailSender để ta gửi xd phương thức gửi mail 

		public CheckoutController(IEmailSender emailSender,DataContext context)
		{
			_dataContext = context;
			_emailSender = emailSender;
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
				//sau khi đặt hàng và bấm checkout , ta sẽ xóa đi đơn hàng đã đặt và gửi mail cho khách hàng bằng cái email trandang ở file Emailseder 
				var receiver = userEmail;  //email nhận, nó đc gửi đi từ email trandang211, sau này ta sẽ thay email nhận này = email ng dùng nhập vào 
				var subject = "Đặt hàng thành công";  //tiêu đề
				var message = "Đặt hàng thành công, trải nghiệm dịch vụ nhé ";  //ND thông báo khi chạy ctrinh, nó gửi mail với ND này cho ng đã mua hàng 

				await _emailSender.SendEmailAsync(receiver, subject, message);

				TempData["success"] = "Checkout thành công, vui lòng chờ đơn hàng được duyệt";
				return View("Index", "Cart");

			}
			return View();
		}
	}
}
