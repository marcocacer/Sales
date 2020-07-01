
namespace Sales.Backend.Controllers
{
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Net;
    using System.Web.Mvc;
    using Sales.Backend.Models;
    using Sales.Common.Models;
    using Sales.Backend.Helpers;

    [Authorize]
    public class CategoriesController : Controller
    {
        private LocalDataContext db = new LocalDataContext();

        // GET: Categories
        public async Task<ActionResult> Index()
        {
            //return View(await db.Categories.ToListAsync());
            return View(await db.Categories.OrderBy(c => c.Description).ToListAsync());
        }

        
        // GET: Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "CategoryId,Description,ImagePath")] Category category)
        public async Task<ActionResult> Create(CategoryView view)
        {
            if (ModelState.IsValid)
            {
                //db.Categories.Add(category);
                //await db.SaveChangesAsync();
                //return RedirectToAction("Index");
                var pic = string.Empty;
                var folder = "~/Content/Categories";
                if (view.ImageFile != null)
                {
                    pic = FilesHelper.UploadPhoto(view.ImageFile, folder);
                    pic = $"{folder}/{pic}";
                }
                var category = this.ToCategory(view, pic);
                this.db.Categories.Add(category);
                await this.db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(view);
        }

        private Category ToCategory(CategoryView view, string pic)
        {
            return new Category
            {
                CategoryId = view.CategoryId,
                Description = view.Description,
                ImagePath = pic,
            };
        }

        // GET: Categories/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var category = await this.db.Categories.FindAsync(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            var view = this.ToView(category);
            return View(view);
        }

        private CategoryView ToView(Category category)
        {
            return new CategoryView
            {
                CategoryId = category.CategoryId,
                Description = category.Description,
                ImagePath = category.ImagePath,
            };
        }

        // POST: Categories/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CategoryView view)
        {
            if (ModelState.IsValid)
            {
                var pic = view.ImagePath;
                var folder = "~/Content/Categories";
                if (view.ImageFile != null)
                {
                    pic = FilesHelper.UploadPhoto(view.ImageFile, folder);
                    pic = $"{folder}/{pic}";
                }
                var category = this.ToCategory(view, pic);
                this.db.Entry(category).State = EntityState.Modified;
                await this.db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(view);
        }

        // GET: Categories/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = await db.Categories.FindAsync(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Category category = await db.Categories.FindAsync(id);
            db.Categories.Remove(category);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
