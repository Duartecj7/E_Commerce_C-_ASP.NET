using E_Commerce_C__ASP.NET.Data;
using E_Commerce_C__ASP.NET.Models;
using E_Commerce_C__ASP.NET.Utility;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.Extensions.Options;


namespace E_Commerce_C__ASP.NET.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _context;
        private readonly StripeSettings _stripeSettings;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IOptions<StripeSettings> stripeSettings)
        {
            _logger = logger;
            _context = context;
            _stripeSettings = stripeSettings.Value;
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
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
                                    .FirstOrDefault(p => p.Id == id);
            if (produto == null)
                return NotFound();

            return View(produto);
        }

        // Método para adicionar o produto ao carrinho
        [HttpPost]
        [ActionName("Details")] 
        public IActionResult DetailsProduto(int? id, int quantidade = 1)
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

            var produtoExistente = produtos.FirstOrDefault(p => p.Id == id);

            if (produtoExistente != null)
            {
                if (produtoExistente.Quantidade + quantidade > produto.Stock)
                {
                    TempData["Error"] = "Esgotou o stock deste produto. Apenas pode comprar a quantidade que já está no carrinho.";
                    return RedirectToAction("Carrinho");
                }

                produtoExistente.Quantidade += quantidade;

                produto.Stock -= quantidade;
            }
            else
            {
                if (quantidade > produto.Stock)
                {
                    TempData["Error"] = "Esgotou o stock deste produto. Não pode adicionar mais do que o disponível.";
                    return RedirectToAction("Carrinho");
                }

                produto.Quantidade = quantidade;
                produtos.Add(produto);

                produto.Stock -= quantidade;
            }

            if (produto.Stock < 0)
                produto.Stock = 0;

            _context.Update(produto);
            _context.SaveChanges();  

            HttpContext.Session.SetObjectAsJson("produtos", produtos);

            return RedirectToAction("Details", new { id = produto.Id });
        }



        [ActionName("Remove")]
        public IActionResult RemovedoCarrinho(int? id)
        {
            List<Produto> produtos = HttpContext.Session.GetObjectFromJson<List<Produto>>("produtos");
            if (produtos != null)
            {
                var produto = produtos.FirstOrDefault(p => p.Id == id);
                if (produto != null)
                {
                    var produtoDb = _context.DbSet_Produto.FirstOrDefault(p => p.Id == produto.Id);

                    if (produtoDb != null)
                    {
                        produtoDb.Stock += 1;
                        _context.Update(produtoDb);
                        _context.SaveChanges(); 
                    }

                    produtos.Remove(produto);
                    HttpContext.Session.SetObjectAsJson("produtos", produtos);
                }
            }
            return RedirectToAction("Carrinho");
        }


        [HttpPost]
        public IActionResult Remove(int? id)
        {
            List<Produto> produtos = HttpContext.Session.GetObjectFromJson<List<Produto>>("produtos");
            if (produtos != null)
            {
                var produto = produtos.FirstOrDefault(p => p.Id == id);
                if (produto != null)
                {
                    produtos.Remove(produto);
                    HttpContext.Session.SetObjectAsJson("produtos", produtos);
                }
            }
            return RedirectToAction("Index");
        }
        public IActionResult EmptyCart()
        {
            List<Produto> produtos = HttpContext.Session.GetObjectFromJson<List<Produto>>("produtos");

            if (produtos != null)
            {
                foreach (var produto in produtos)
                {
                    var produtoDb = _context.DbSet_Produto.FirstOrDefault(p => p.Id == produto.Id);

                    if (produtoDb != null)
                    {
                        produtoDb.Stock += (int)produto.Quantidade;

                        _context.Update(produtoDb);
                    }
                }

                _context.SaveChanges();

                HttpContext.Session.Remove("produtos");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ActionName("EmptyCart")]
        public IActionResult EmptyCarrinho()
        {
            List<Produto> produtos = HttpContext.Session.GetObjectFromJson<List<Produto>>("produtos");

            if (produtos != null)
            {
                foreach (var produto in produtos)
                {
                    var produtoDb = _context.DbSet_Produto.FirstOrDefault(p => p.Id == produto.Id);

                    if (produtoDb != null)
                    {
                        produtoDb.Stock += (int)produto.Quantidade;


                        _context.Update(produtoDb); 
                    }
                }

                _context.SaveChanges();

                HttpContext.Session.Remove("produtos");
            }

            return Ok();
        }



        public IActionResult Carrinho()
        {
            List<Produto> produtos = HttpContext.Session.GetObjectFromJson<List<Produto>>("produtos");
            if (produtos == null)
                produtos = new List<Produto>();

            ViewBag.Error = TempData["Error"];
            return View(produtos);
        }

       
       




    }
}
