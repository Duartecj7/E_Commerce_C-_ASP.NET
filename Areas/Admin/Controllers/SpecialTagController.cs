using E_Commerce_C__ASP.NET.Data;
using E_Commerce_C__ASP.NET.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_C__ASP.NET.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SpecialTagController : Controller
    {
        ApplicationDbContext _context;
        public SpecialTagController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var dados = _context.DbSet_Tags.ToList();
            return View(dados);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialTag tag)
        {
            if (ModelState.IsValid)
            {
                _context.DbSet_Tags.Add(tag);
                await _context.SaveChangesAsync();
                TempData["save"] = "A Tag foi guardada com sucesso!";
                return RedirectToAction("Index");
            }
            return View(tag);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var tag = _context.DbSet_Tags.Find(id);
            if (tag == null)
                return NotFound();

            return View(tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SpecialTag tag)
        {
            if (ModelState.IsValid)
            {
                _context.Update(tag);
                await _context.SaveChangesAsync();
                TempData["edit"] = "A Tag foi editada com sucesso!";
                return RedirectToAction("Index");
            }
            return View(tag);
        }


        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            var tag = _context.DbSet_Tags.Find(id);
            if (tag == null)
                return NotFound();

            return View(tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(TiposProduto tiposProduto)
        {
            return RedirectToAction(nameof(Index));

        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var tag = _context.DbSet_Tags.Find(id);
            if (tag == null)
                return NotFound();

            return View(tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id, SpecialTag tag)
        {
            if (id == null)
                return NotFound();

            if (id != tag.Id)
                return NotFound();

            var _tag = _context.DbSet_Tags.Find(id);
            if (_tag == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Remove(_tag);
                await _context.SaveChangesAsync();
                TempData["delete"] = "A Tag foi eliminada com sucesso!";
                return RedirectToAction("Index");
            }
            return View(tag);
        }

    }
}
