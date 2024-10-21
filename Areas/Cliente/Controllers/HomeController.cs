using E_Commerce_C__ASP.NET.Data;
using E_Commerce_C__ASP.NET.Models;
using E_Commerce_C__ASP.NET.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using System.Diagnostics;

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

        [HttpGet]
        public IActionResult Checkout()
        {
            var produtos = HttpContext.Session.GetObjectFromJson<List<Produto>>("produtos") ?? new List<Produto>();
            if (!produtos.Any())
            {
                TempData["Error"] = "O carrinho está vazio. Adicione produtos antes de finalizar a compra.";
                return RedirectToAction("Carrinho");
            }

            var itensPedido = produtos.Select(p => new ItemPedido
            {
                ProdutoId = p.Id,
                Preco = p.Preco,
                Produto = p,
                Quantidade = (int)p.Quantidade
            }).ToList();

            var pedido = new Pedido
            {
                ItensPedido = itensPedido,
                DataPedido = DateTime.Now,
                NumPedido = Guid.NewGuid().ToString()
            };

            // Passa a chave pública do Stripe para a View através do ViewBag
            ViewBag.StripePublishableKey = _stripeSettings.PublishableKey;

            return View(pedido); // Passa o pedido para a View
        }

        [HttpPost]
        public IActionResult FinalizarPedido(Pedido pedido)
        {
            var produtos = HttpContext.Session.GetObjectFromJson<List<Produto>>("produtos") ?? new List<Produto>();

            if (produtos == null || !produtos.Any())
            {
                TempData["Error"] = "O carrinho está vazio. Adicione produtos antes de finalizar a compra.";
                return RedirectToAction("Carrinho");
            }

            pedido.NumPedido = Guid.NewGuid().ToString(); 
            pedido.DataPedido = DateTime.Now;

            foreach (var produto in produtos)
            {
                pedido.ItensPedido.Add(new ItemPedido
                {
                    ProdutoId = produto.Id,
                    Preco = produto.Preco,
                    Quantidade = (int)produto.Quantidade,
                    
                    
                });
            }

            _context.DbSet_Pedido.Add(pedido);
            _context.SaveChanges();

            HttpContext.Session.Remove("produtos");

            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult ProcessPayment(Pedido pedido, string stripeToken, string paymentMethod, string mbwayNumber = null)
        {
            var produtos = HttpContext.Session.GetObjectFromJson<List<Produto>>("produtos") ?? new List<Produto>();
            if (produtos == null || !produtos.Any())
            {
                TempData["Error"] = "O pedido está vazio.";
                return RedirectToAction("Carrinho");
            }
            else if (pedido.ItensPedido == null || !pedido.ItensPedido.Any())
            {
                pedido.ItensPedido = produtos.Select(produto => new ItemPedido
                {
                    ProdutoId = produto.Id,
                    Preco = produto.Preco,
                    Quantidade = (int)produto.Quantidade
                }).ToList();
            }

            var totalAmount = (long)(pedido.ItensPedido.Sum(item => item.Preco * item.Quantidade) * 100); // Valor em centavos

            // Lógica de acordo com o método de pagamento selecionado
            switch (paymentMethod)
            {
                case "stripe":
                    if (string.IsNullOrEmpty(stripeToken))
                    {
                        TempData["Error"] = "Token de pagamento inválido. Tente novamente.";
                        return RedirectToAction("Checkout");
                    }

                    // Pagamento via Stripe
                    var options = new ChargeCreateOptions
                    {
                        Amount = totalAmount,
                        Currency = "eur",
                        Description = $"Pedido {pedido.NumPedido}",
                        Source = stripeToken,
                    };

                    var service = new ChargeService();
                    Charge charge = service.Create(options);

                    if (charge.Status == "succeeded")
                    {
                        return FinalizarPedidoComSucesso(pedido);
                    }
                    else
                    {
                        TempData["Error"] = "Falha no processamento do pagamento com cartão.";
                        return RedirectToAction("Checkout");
                    }

                case "mbway":
                    if (string.IsNullOrEmpty(mbwayNumber))
                    {
                        TempData["Error"] = "Número MB WAY inválido.";
                        return RedirectToAction("Checkout");
                    }

                    // Lógica para processar MB WAY (simulação de fluxo MB WAY)
                    bool mbwaySuccess = SimularPagamentoMBWay(mbwayNumber, totalAmount);

                    if (mbwaySuccess)
                    {
                        return FinalizarPedidoComSucesso(pedido);
                    }
                    else
                    {
                        TempData["Error"] = "Falha no pagamento via MB WAY.";
                        return RedirectToAction("Checkout");
                    }

                case "referencia":
                    // Pagamento via Referência (gerar entidade e referência fictícia)
                    TempData["Success"] = "Referência gerada com sucesso. Por favor, finalize o pagamento para completar o pedido.";
                    return RedirectToAction("ReferenciaGerada", new { pedidoId = pedido.Id });

                default:
                    TempData["Error"] = "Método de pagamento inválido.";
                    return RedirectToAction("Checkout");
            }
        }


        // Método auxiliar para finalizar o pedido com sucesso
        private IActionResult FinalizarPedidoComSucesso(Pedido pedido)
        {
            var ultimoPedido = _context.DbSet_Pedido.OrderByDescending(p => p.Id).FirstOrDefault();
            int novoNumeroPedido = (ultimoPedido?.NumPedido != null) ? int.Parse(ultimoPedido.NumPedido) + 1 : 1; // Inicia em 1 se não houver pedidos
            pedido.NumPedido = novoNumeroPedido.ToString();
            // Salvar o pedido no banco de dados
            _context.DbSet_Pedido.Add(pedido);
            _context.SaveChanges();

            // Limpar o carrinho após o pagamento bem-sucedido
            HttpContext.Session.Remove("produtos");

            return View("PedidoConfirmado", pedido);
        }

        // Método fictício para simular pagamento MB WAY
        private bool SimularPagamentoMBWay(string mbwayNumber, long amount)
        {
            // Implementar aqui a lógica de comunicação com a API real do MB WAY, caso haja uma
            // Atualmente, simulação de sucesso
            return true; // Retornar false para simular falha
        }



    }
}
