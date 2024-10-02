using E_Commerce_C__ASP.NET.Data;
using E_Commerce_C__ASP.NET.Models;
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
            return View(_context.DbSet_Produto.Include(p => p.TipoProduto).Include(p => p.Tag).ToList());
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
            if (id == null)
                return NotFound();
            var produto = _context.DbSet_Produto.Include(p => p.TipoProduto).Include(p => p.Tag).FirstOrDefault(p=>p.Id == id);
            if (produto == null)
                return NotFound();
            return View(produto);
        }
    }
}
