using E_Commerce_C__ASP.NET.Data;
using E_Commerce_C__ASP.NET.Models;
using E_Commerce_C__ASP.NET.Utility;
using iText.StyledXmlParser.Jsoup.Nodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Linq;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace E_Commerce_C__ASP.NET.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    public class PedidosClienteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PedidosClienteController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        public IActionResult Index()
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pedidos = _context.DbSet_Pedido
                .Where(p => p.UserId == usuarioId)
                .ToList();
            return View(pedidos);
        }
        [HttpPost]
        [Authorize]
        public IActionResult FinalizarPedido(ICollection<ItemPedido> items, string Nome, string NumTel, string Email, string Morada, string codigoPost, string Localidade, string TipoPagamento)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Random random = new Random();
            string numPedido = $"PED{random.Next(100000, 999999)}";
            double totalProdutos = items.Sum(item => item.Preco * item.Quantidade);

            Pedido novoPedido = new Pedido
            {
                ItensPedido = items,
                Nome = Nome,
                NumTel = NumTel,
                Email = Email,
                Morada = Morada,
                CodigoPost = codigoPost,
                Localidade = Localidade,
                Status = "Pagamento Pendente",
                DataPedido = DateTime.Now,
                NumPedido = numPedido,
                TipoPagamento = TipoPagamento,
                UserId = userId,
                PrecoFinal = totalProdutos
            };

            try
            {
                if (novoPedido.ItensPedido == null || !novoPedido.ItensPedido.Any())
                {
                    ModelState.AddModelError("", "Não há itens no pedido.");
                    return View(novoPedido);
                }

                _context.DbSet_Pedido.Add(novoPedido);
                _context.SaveChanges();
                HttpContext.Session.Remove("produtos");

                // Configura uma mensagem de sucesso no TempData
                TempData["Success"] = "Pedido finalizado com sucesso!";

                return RedirectToAction("Index");
            }
            catch (DbUpdateException dbEx)
            {
                ModelState.AddModelError("", "Erro ao finalizar o pedido. Por favor, tente novamente.");

                // Configura uma mensagem de erro no TempData
                TempData["Error"] = "Erro ao finalizar o pedido. Por favor, tente novamente.";
                return View(novoPedido);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro inesperado. Por favor, tente novamente.");

                // Configura uma mensagem de erro no TempData
                TempData["Error"] = "Erro inesperado. Por favor, tente novamente.";
                return View(novoPedido);
            }
        }


        // Mostrar detalhes de um pedido específico
        [HttpGet]
        public IActionResult PedidoSucesso(int numPedido)
        {
            var pedido = _context.DbSet_Pedido
                .Include(p => p.ItensPedido)
                .ThenInclude(i => i.Produto)
                .FirstOrDefault(p => p.Id == numPedido); // Obtendo apenas um pedido

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido); // Passa um único pedido para a View
        }
        [HttpPost]
        [Authorize]
        public IActionResult EliminarPedido(int numPedido)
        {
            var pedido = _context.DbSet_Pedido.Find(numPedido);
            if (pedido == null)
            {
                // Configura uma mensagem de erro no TempData
                TempData["Error"] = "Pedido não encontrado.";
                return NotFound();
            }

            try
            {
                _context.DbSet_Pedido.Remove(pedido);
                _context.SaveChanges();

                // Configura uma mensagem de sucesso no TempData
                TempData["Success"] = "Pedido eliminado com sucesso.";
            }
            catch (Exception ex)
            {
                // Configura uma mensagem de erro no TempData
                TempData["Error"] = "Erro ao eliminar o pedido. Por favor, tente novamente.";
            }

            // Redireciona de volta para a lista de pedidos
            return RedirectToAction("Index");
        }

        // Método para mostrar o formulário de edição
        [HttpGet]
        [Authorize]
        public IActionResult EditarPedido(int numPedido)
        {
            var pedido = _context.DbSet_Pedido.Find(numPedido);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // Método para processar a atualização do pedido
        [HttpPost]
        [Authorize]
        public IActionResult EditarPedido(int numPedido, string userId, Pedido pedidoAtualizado)
        {
            if (ModelState.IsValid)
            {
                var pedido = _context.DbSet_Pedido
                           .FirstOrDefault(p => p.Id == numPedido && p.UserId == userId);
                if (pedido == null)
                {
                    // Configura uma mensagem de erro no TempData
                    TempData["Error"] = "Pedido não encontrado.";
                    return NotFound();
                }

                try
                {
                    // Atualizar as propriedades do pedido
                    pedido.Nome = pedidoAtualizado.Nome;
                    pedido.NumTel = pedidoAtualizado.NumTel;
                    pedido.Email = pedidoAtualizado.Email;
                    pedido.Morada = pedidoAtualizado.Morada;
                    pedido.CodigoPost = pedidoAtualizado.CodigoPost;
                    pedido.Localidade = pedidoAtualizado.Localidade;
                    pedido.TipoPagamento = pedidoAtualizado.TipoPagamento;
                    pedido.Status = pedidoAtualizado.Status;

                    // Salvar as mudanças no banco de dados
                    _context.SaveChanges();

                    // Configura uma mensagem de sucesso no TempData
                    TempData["Success"] = "Pedido atualizado com sucesso.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Configura uma mensagem de erro no TempData
                    TempData["Error"] = "Erro ao atualizar o pedido. Por favor, tente novamente.";
                }
            }
            // Retornar ao formulário se o modelo não for válido
            return View(pedidoAtualizado);
        }
        [HttpGet]
        public IActionResult LoadPagamentoPartial(int id)
        {
            var pedido = _context.DbSet_Pedido.FirstOrDefault(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound();
            }

            return PartialView("_PagamentoPartial", pedido);
        }

        [HttpPost]
        [Authorize]
        public IActionResult PagarPedido(Pedido pedido)
        {
            // Aqui você pode tratar o pedido com as informações que foram submetidas
            // Exemplo: salvar os dados de pagamento, atualizar o status do pedido, etc.

            try
            {
                // Lógica de processamento do pedido e pagamento
                // Adicione lógica para validar os dados do pedido

                TempData["Success"] = "Pagamento realizado com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Erro ao processar o pagamento. Por favor, tente novamente.";
                return RedirectToAction("Index");
            }
        }
    }
}
