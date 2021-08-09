using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarParts.Data;
using CarParts.Models;
using CarParts.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreHero.ToastNotification.Abstractions;
using cloudscribe.Pagination.Models;

namespace CarParts.Controllers
{
    [Authorize(Roles = "مسؤول")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment env;
        private readonly INotyfService notyfService;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment env, INotyfService notyfService)
        {
            _context = context;
            this.env = env;
            this.notyfService = notyfService;
        }


        // GET: Products
        /*public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }*/

        /* Return records with Pagination*/
        public IActionResult Index(int PageNumber = 1, int PageSize = 32)
        {

            var ExcludedRecords = (PageSize * PageNumber) - PageSize;

            var products = _context.Products.Skip(ExcludedRecords).Take(PageSize);

            var resultOfPagination = new PagedResult<Product>
            {
                Data = products.AsNoTracking().ToList(),
                TotalItems = _context.Products.Count(),
                PageNumber = PageNumber,
                PageSize = PageSize

            };

            return View(resultOfPagination);
        }

        // GET: Products/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                notyfService.Error("حدث خطأ");
                return Content("مافيش منتج بالمواصفات دي");
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                notyfService.Error("حدث خطأ");
                return Content("مافيش منتج بالمواصفات دي");
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string UniqueFileName = ProcessUploadedFiles(model);

                Product product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    ProductPhotoPath = UniqueFileName,
                    Price = model.Price,
                    NumberInStock = model.NumberInStock
                };
                _context.Add(product);
                await _context.SaveChangesAsync();
                notyfService.Success("تم اضافة المنتج بنجاح");
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                notyfService.Error("حدث خطأ");
                return Content("مافيش منتج بالمواصفات دي");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                notyfService.Error("حدث خطأ");
                return Content("مافيش منتج بالمواصفات دي");
            }
            ProductEditViewModel model = new ProductEditViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                NumberInStock = product.NumberInStock,
                ExistingPhoto = product.ProductPhotoPath
            };
            return View(model);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductEditViewModel model)
        {
            if (id != model.Id)
            {
                notyfService.Error("حدث خطأ");
                return Content("مافيش منتج بالمواصفات دي");
            }

            if (ModelState.IsValid)
            {
                Product product = _context.Products.Find(model.Id);
                product.Name = model.Name;
                product.Description = model.Description;
                product.NumberInStock = model.NumberInStock;
                product.Price = model.Price;

                if (model.ProductPhoto != null)
                {
                    if(model.ExistingPhoto != null)
                    {
                        string oldFile = model.ExistingPhoto;
                        string filePath = Path.Combine(env.WebRootPath, "images", oldFile);
                        System.GC.Collect();
                        System.GC.WaitForPendingFinalizers();
                        System.IO.File.Delete(filePath);
                    }
                    product.ProductPhotoPath =  ProcessUploadedFiles(model);
                }
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    notyfService.Success("تم تعديل المنتج بنجاح");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        notyfService.Error("حدث خطأ");
                        return Content("مافيش منتج بالمواصفات دي");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        private string ProcessUploadedFiles(ProductCreateViewModel model)
        {
            string UniqueFileName = null;
            if (model.ProductPhoto != null)
            {
                string ImagesFolder = Path.Combine(env.WebRootPath, "images");
                UniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProductPhoto.FileName;
                string FullPath = Path.Combine(ImagesFolder, UniqueFileName);
                model.ProductPhoto.CopyTo(new FileStream(FullPath, FileMode.Create));
            }
            return UniqueFileName;
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                notyfService.Error("حدث خطأ");
                return Content("مافيش منتج بالمواصفات دي");
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                notyfService.Error("حدث خطأ");
                return Content("مافيش منتج بالمواصفات دي");
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if(product.ProductPhotoPath != null)
            {
                string filePath = Path.Combine(env.WebRootPath, "images", product.ProductPhotoPath);
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.IO.File.Delete(filePath);
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            notyfService.Success("تم حذف المنتج بنجاح");
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
