﻿using CK_ASP_NET_CORE.Models;
using CK_ASP_NET_CORE.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CK_ASP_NET_CORE.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index(string Slug= "")
        {
            CategoryModel category = _dataContext.Categories.Where(c =>c.Slug == Slug).FirstOrDefault();
            if (category == null) return RedirectToAction("Index");
            var productsByCategory = _dataContext.Products.Where(c =>c.CategoryId == category.Id);
            return View(await productsByCategory.OrderByDescending(p=> p.Id).ToListAsync());
        }
    }
}
