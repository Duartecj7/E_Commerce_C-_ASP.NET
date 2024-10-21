using E_Commerce_C__ASP.NET.Data;
using E_Commerce_C__ASP.NET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_C__ASP.NET.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProdutoController : Controller
    {
        private ApplicationDbContext _context;
        private IWebHostEnvironment _hostEnvironment;
        
        public ProdutoController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View(_context.DbSet_Produto.Include(p=>p.TipoProduto).Include(s=>s.Tag).ToList());
        }

        [HttpPost]
        public IActionResult Index(double lowAmount,double largeAmount)
        {
            var produto = _context.DbSet_Produto.Include(p => p.TipoProduto).Include(s => s.Tag).Where(p => p.Preco >= lowAmount && p.Preco <= largeAmount).ToList();
            if(lowAmount==0 && largeAmount ==0)
                produto = _context.DbSet_Produto.Include(p => p.TipoProduto).Include(s => s.Tag).ToList();
            return View(produto);
        }

        public IActionResult Create()
        {
            ViewData["TipoProdutoId"] = new SelectList(_context.DbSet_TiposProduto.ToList(), "Id", "TipoProduto");
            ViewData["TagId"] = new SelectList(_context.DbSet_Tags.ToList(), "Id", "TagNome");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Produto produto, IFormFile imagem)
        {
            var procuraProduto = _context.DbSet_Produto.FirstOrDefault(c => c.Nome == produto.Nome);
            if(produto!= null)
            {
                SpecialTag tag = _context.DbSet_Tags.FirstOrDefault(i => i.Id == produto.TagId);
                TiposProduto tipo = _context.DbSet_TiposProduto.FirstOrDefault(i => i.Id == produto.TipoProdutoId);
                produto.Tag = tag;
                produto.TipoProduto=tipo;
            }
            ModelState.Remove("Imagem");
            if (ModelState.IsValid)
            {
                if (procuraProduto != null)
                {
                    ViewBag.message = "Produto já existe";
                    ViewData["TipoProdutoId"] = new SelectList(_context.DbSet_TiposProduto.ToList(), "Id", "TipoProduto");
                    ViewData["TagId"] = new SelectList(_context.DbSet_Tags.ToList(), "Id", "TagNome");
                    return View(produto);
                }

                if (imagem != null)
                {
                    var name = Path.Combine(_hostEnvironment.WebRootPath + "/Imagens", Path.GetFileName(imagem.FileName));
                    await imagem.CopyToAsync(new FileStream(name, FileMode.Create));
                    produto.Imagem = "Imagens/" + imagem.FileName;
                }
                else
                {
                    produto.Imagem = "Imagens/No-Image.png";
                }

                _context.DbSet_Produto.Add(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewData["TipoProdutoId"] = new SelectList(_context.DbSet_TiposProduto.ToList(), "Id", "TipoProduto");
            ViewData["TagId"] = new SelectList(_context.DbSet_Tags.ToList(), "Id", "TagNome");
            return View(produto);
        }


        public IActionResult Edit(int? id)
        {
            ViewData["TipoProdutoId"] = new SelectList(_context.DbSet_TiposProduto.ToList(), "Id", "TipoProduto");
            ViewData["TagId"] = new SelectList(_context.DbSet_Tags.ToList(), "Id", "TagNome");
            
            if (id == null)
                return NotFound();
            
            var produto = _context.DbSet_Produto.Include(p => p.TipoProduto).Include(p => p.Tag).FirstOrDefault(p => p.Id == id);
            
            if (produto == null) 
                return NotFound();
            
            return View(produto);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Produto produto, IFormFile imagem)
        {
            if (produto != null)
            {
                SpecialTag tag = _context.DbSet_Tags.FirstOrDefault(i => i.Id == produto.TagId);
                TiposProduto tipo = _context.DbSet_TiposProduto.FirstOrDefault(i => i.Id == produto.TipoProdutoId);
                produto.Tag = tag;
                produto.TipoProduto = tipo;
            }

            ModelState.Remove("Imagem");

            if (ModelState.IsValid)
            {
                if (imagem != null)
                {
                    var name = Path.Combine(_hostEnvironment.WebRootPath + "/Imagens", Path.GetFileName(imagem.FileName));
                    await imagem.CopyToAsync(new FileStream(name, FileMode.Create));
                    produto.Imagem = "Imagens/" + imagem.FileName;
                }
                else if (string.IsNullOrEmpty(produto.Imagem))
                {
                    produto.Imagem = "Imagens/No-Image.png";
                }

                _context.DbSet_Produto.Update(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewData["TipoProdutoId"] = new SelectList(_context.DbSet_TiposProduto.ToList(), "Id", "TipoProduto");
            ViewData["TagId"] = new SelectList(_context.DbSet_Tags.ToList(), "Id", "TagNome");
            return View(produto);
        }


        public IActionResult Details(int? id)
        {
            //if (id == null)
            //    return NotFound();
            //var produto = _context.DbSet_Produto.Include(p => p.TipoProduto).Include(p => p.Tag).FirstOrDefault(c => c.Id == id);
            //if (produto == null)
            //    return NotFound();
            //return View(produto);

            var produto = _context.DbSet_Produto.Find(id);
            if (produto == null)
            {
                return NotFound();
            }
            return View(produto);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var produto = _context.DbSet_Produto.Include(p => p.Tag).Include(p => p.TipoProduto).Where(p => p.Id == id).FirstOrDefault();
            if (produto == null)
                return NotFound();

            return View(produto);
        }
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult>DeleteConfirmation(int? id)
        {
            if (id == null)
                return NotFound();
            var produto = _context.DbSet_Produto.FirstOrDefault(p => p.Id == id);
            if(produto == null)
                return NotFound();
            _context.DbSet_Produto.Remove(produto);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
