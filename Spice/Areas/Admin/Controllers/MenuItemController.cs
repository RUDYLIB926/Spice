using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models.ViewModels;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        //[BindProperty]
        //private MenuItemViewModel MenuItemViewModel { get; set; }

        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;

            //MenuItemViewModel = new MenuItemViewModel()
            //{
            //    CategoryList = _db.Category.ToList(),
            //    MenuItem = new Models.MenuItem()
            //};
        }

        public async Task<IActionResult> Index()
        {
            var menuItems = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync();
            return View(menuItems);
        }

        // GET - CREATE
        public IActionResult Create()
        {
            var MenuItemViewModel = new MenuItemViewModel()
            {
                CategoryList = _db.Category.ToList(),
                MenuItem = new Models.MenuItem()
            };
            return View(MenuItemViewModel);
        }

        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuItemViewModel model)
        {
            model.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            if (ModelState.IsValid)
            {
                await _db.MenuItem.AddAsync(model.MenuItem);
                await _db.SaveChangesAsync();

                Models.MenuItem menuItemFromDB = await _db.MenuItem.FindAsync(model.MenuItem.Id);
                // Image Saving
                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                var extension = ".png";

                if (files.Count() > 0) // use the uploaded files
                {
                    var uploads = Path.Combine(webRootPath, "images");
                    extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(uploads, model.MenuItem.Id + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                }
                else // no file was uploaded, so use default file
                {
                    var uploads = Path.Combine(webRootPath, @"images\" + StaticDetails.DefaultFoodImage);
                    System.IO.File.Copy(uploads, webRootPath + @"\images\" + model.MenuItem.Id + extension);
                }

                menuItemFromDB.Image = @"\images\" + menuItemFromDB.Id + extension;
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var MenuItemViewModel = new MenuItemViewModel();

            var menuItem = await _db.MenuItem.Include(m=>m.Category).Include(m=>m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            var subCategoryList = await _db.SubCategory.Where(s => s.CategoryId == menuItem.CategoryId).ToListAsync();
            var categoryList = await _db.Category.ToListAsync();

            MenuItemViewModel.MenuItem = menuItem;
            MenuItemViewModel.CategoryList = categoryList;
            MenuItemViewModel.SubCategoryList = subCategoryList;

            if (MenuItemViewModel.MenuItem == null)
            {
                return NotFound();
            }
            return View(MenuItemViewModel);
        }

        //POST - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MenuItemViewModel model)
        {
            if(model == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                model.SubCategoryList = await _db.SubCategory.Where(s => s.CategoryId == model.MenuItem.CategoryId).ToListAsync();
                return View(model);
            }
            model.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            Models.MenuItem menuItemFromDB = await _db.MenuItem.FindAsync(model.MenuItem.Id);
            // Image Saving
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            if (files.Count() > 0) // use the uploaded files
            {
                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);

                // delete original file
                var imagePath = Path.Combine(webRootPath, menuItemFromDB.Image.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                using (var fileStream = new FileStream(Path.Combine(uploads, model.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                menuItemFromDB.Image = @"\images\" + menuItemFromDB.Id + extension;
            }
            menuItemFromDB.Name = model.MenuItem.Name;
            menuItemFromDB.Description = model.MenuItem.Description;
            menuItemFromDB.Price = model.MenuItem.Price;
            menuItemFromDB.CategoryId = model.MenuItem.CategoryId;
            menuItemFromDB.SubCategoryId = model.MenuItem.SubCategoryId;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET - DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var MenuItemViewModel = new MenuItemViewModel();

            var menuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            var subCategoryList = await _db.SubCategory.Where(s => s.CategoryId == menuItem.CategoryId).ToListAsync();
            var categoryList = await _db.Category.ToListAsync();

            MenuItemViewModel.MenuItem = menuItem;
            MenuItemViewModel.CategoryList = categoryList;
            MenuItemViewModel.SubCategoryList = subCategoryList;

            if (MenuItemViewModel.MenuItem == null)
            {
                return NotFound();
            }
            return View(MenuItemViewModel);
        }

        // GET - DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var MenuItemViewModel = new MenuItemViewModel();

            var menuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            var subCategoryList = await _db.SubCategory.Where(s => s.CategoryId == menuItem.CategoryId).ToListAsync();
            var categoryList = await _db.Category.ToListAsync();

            MenuItemViewModel.MenuItem = menuItem;
            MenuItemViewModel.CategoryList = categoryList;
            MenuItemViewModel.SubCategoryList = subCategoryList;

            if (MenuItemViewModel.MenuItem == null)
            {
                return NotFound();
            }
            return View(MenuItemViewModel);
        }

        //POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(MenuItemViewModel model)
        {
            if (model == null)
            {
                return NotFound();
            }

            var menuItem = await _db.MenuItem.FindAsync(model.MenuItem.Id);
            if (menuItem != null)
            {
                // delete original file
                string webRootPath = _hostingEnvironment.WebRootPath;
                var imagePath = Path.Combine(webRootPath, menuItem.Image.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                _db.MenuItem.Remove(menuItem);
                await _db.SaveChangesAsync();
            }

            
            return RedirectToAction(nameof(Index));
        }
    }
}