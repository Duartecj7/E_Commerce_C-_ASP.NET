using E_Commerce_C__ASP.NET.Data;
using E_Commerce_C__ASP.NET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace E_Commerce_C__ASP.NET.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TiposProdutoController : Controller
    {
        ApplicationDbContext _context;
        public TiposProdutoController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var dados = _context.DbSet_TiposProduto.ToList();
            return View(dados);
        }

        public IActionResult Create() 
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TiposProduto tiposProduto)
        {
            if (ModelState.IsValid)
            {
                _context.DbSet_TiposProduto.Add(tiposProduto);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tiposProduto);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var tipoProduto = _context.DbSet_TiposProduto.Find(id);
            if(tipoProduto==null)
                return NotFound();

            return View(tipoProduto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TiposProduto tiposProduto)
        {
            if (ModelState.IsValid)
            {
                _context.Update(tiposProduto);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tiposProduto);
        }


        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            var tipoProduto = _context.DbSet_TiposProduto.Find(id);
            if (tipoProduto == null)
                return NotFound();

            return View(tipoProduto);
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

            var tipoProduto = _context.DbSet_TiposProduto.Find(id);
            if (tipoProduto == null)
                return NotFound();

            return View(tipoProduto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id,TiposProduto tiposProduto)
        {
            if(id==null)
                return NotFound();

            if(id!= tiposProduto.Id)
                return NotFound();

            var tipoProduto = _context.DbSet_TiposProduto.Find(id);
            if (tiposProduto == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Remove(tipoProduto);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tiposProduto);
        }



    }
}
