using AspNetCoreHero.ToastNotification.Abstractions;
using CarParts.Data;
using CarParts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using cloudscribe.Pagination.Models;
using Microsoft.EntityFrameworkCore;

namespace CarParts.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext db;
        private readonly INotyfService _notyfService;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext db, INotyfService notyfService)
        {
            _logger = logger;
            _notyfService = notyfService;
            this.db = db;
        }


        /* Return all records with out Pagination*/
        /*public IActionResult Index()
        {
            return View(db.Products.ToList());
        }*/


        /* Return records with Pagination*/
        public IActionResult Index(int PageNumber = 1, int PageSize = 32)
        {
            
            var ExcludedRecords = (PageSize * PageNumber) - PageSize;

            var products = db.Products.Skip(ExcludedRecords).Take(PageSize);

            var resultOfPagination = new PagedResult<Product>
            {
                Data = products.AsNoTracking().ToList(),
                TotalItems = db.Products.Count(),
                PageNumber = PageNumber,
                PageSize = PageSize

            };

            return View(resultOfPagination);
        }



        [HttpPost]
        public IActionResult Search(string searchQuery)
        {
            if (searchQuery == null || searchQuery == "")
            {
                _notyfService.Error("لازم تكتب كلمة في البحث عشان ندورلك عليها");
                return RedirectToAction("Index");
            }

            var products = db.Products.Where(p => p.Name.Contains(searchQuery) 
            || p.Description.Contains(searchQuery)
            || p.Price.Contains(searchQuery)).ToList();

            return View(products);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
