﻿using Microsoft.AspNetCore.Mvc;

namespace Eshopping.Controllers
{
    public class ProductController : Controller
    {
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult Details()
		{
			return View();
		}
	}
}
