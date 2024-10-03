using E_Commerce_C__ASP.NET.Data;
using E_Commerce_C__ASP.NET.Models;
using E_Commerce_C__ASP.NET.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace E_Commerce_C__ASP.NET.Areas.Cliente.Controllers
{
   [Area("Cliente")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.RenderCart = true;
            return View(_context.DbSet_Produto
                                    .Include(p => p.TipoProduto)
                                    .Include(p => p.Tag)
                                    .ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Details(int? id)
        {
            ViewBag.RenderCart = true;
            if (id == null)
                return NotFound();

            var produto = _context.DbSet_Produto
                                    .Include(p => p.TipoProduto)
                                    .Include(p => p.Tag)
                                    .FirstOrDefault(p=>p.Id == id);
            if (produto == null)
                return NotFound();

            return View(produto);
        }
        [HttpPost]
        [ActionName("Details")]
        public IActionResult DetailsProduto(int? id)
        {
            ViewBag.RenderCart = true;
            if (id == null)
                return NotFound();

            var produto = _context.DbSet_Produto
                                   .Include(p => p.TipoProduto)
                                   .Include(p => p.Tag)
                                   .FirstOrDefault(p => p.Id == id);

            if (produto == null)
                return NotFound();

            List<Produto> produtos = HttpContext.Session.GetObjectFromJson<List<Produto>>("produtos") ?? new List<Produto>();

            produtos.Add(produto);

            HttpContext.Session.SetObjectAsJson("produtos", produtos);

            return View(produto);
        }
        [HttpPost]
        public IActionResult Remove(int? id)
        {
            List<Produto> produtos = HttpContext.Session.GetObjectFromJson<List<Produto>>("produtos");
            if(produtos !=null)
            {
                var produto = produtos.FirstOrDefault(p => p.Id == id);
                if(produto != null)
                {
                    produtos.Remove(produto);
                    HttpContext.Session.SetObjectAsJson("produtos", produtos);
                }
                    


            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult EmptyCart()
        {
            HttpContext.Session.Remove("produtos");
            return Ok();
        }

    }
}
