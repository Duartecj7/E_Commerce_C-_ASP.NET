using E_Commerce_C__ASP.NET.Data;
using E_Commerce_C__ASP.NET.Models;
using E_Commerce_C__ASP.NET.Utility;
using iText.StyledXmlParser.Jsoup.Nodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Stripe;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.IO;
using iText.IO.Image;
using iText.Kernel.Colors;
using Document = iText.Layout.Document;
namespace E_Commerce_C__ASP.NET.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    public class PedidosClienteController : Controller
    {
        private readonly ILogger<PedidosClienteController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly StripeSettings _stripeSettings;


        public PedidosClienteController(ILogger<PedidosClienteController> logger, ApplicationDbContext context, IOptions<StripeSettings> stripeSettings)
        {
            _logger = logger;
            _context = context;
            _stripeSettings = stripeSettings.Value;
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

        }
        [Authorize]
        public IActionResult Index()
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pedidos = _context.DbSet_Pedido
                    .Include(p => p.ItensPedido)
                    .ThenInclude(i => i.Produto)
                    .ToList();
            return View(pedidos);
        }
        //Metodo para registar um pedido
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

                TempData["Success"] = "Pedido finalizado com sucesso!";

                return RedirectToAction("Index");
            }
            catch (DbUpdateException dbEx)
            {
                ModelState.AddModelError("", "Erro ao finalizar o pedido. Por favor, tente novamente.");

                TempData["Error"] = "Erro ao finalizar o pedido. Por favor, tente novamente.";
                return View(novoPedido);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro inesperado. Por favor, tente novamente.");

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
                .FirstOrDefault(p => p.Id == numPedido); 

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido); 
        }
        [HttpPost]
        [Authorize]
        public IActionResult EliminarPedido(int numPedido)
        {
            var pedido = _context.DbSet_Pedido.Find(numPedido);
            if (pedido == null)
            {
                TempData["Error"] = "Pedido não encontrado.";
                return NotFound();
            }

            try
            {
                _context.DbSet_Pedido.Remove(pedido);
                _context.SaveChanges();

                TempData["Success"] = "Pedido eliminado com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Erro ao eliminar o pedido. Por favor, tente novamente.";
            }

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
                    TempData["Error"] = "Pedido não encontrado.";
                    return NotFound();
                }

                try
                {
                    pedido.Nome = pedidoAtualizado.Nome;
                    pedido.NumTel = pedidoAtualizado.NumTel;
                    pedido.Email = pedidoAtualizado.Email;
                    pedido.Morada = pedidoAtualizado.Morada;
                    pedido.CodigoPost = pedidoAtualizado.CodigoPost;
                    pedido.Localidade = pedidoAtualizado.Localidade;
                    pedido.TipoPagamento = pedidoAtualizado.TipoPagamento;
                    pedido.Status = pedidoAtualizado.Status;

                    _context.SaveChanges();

                    TempData["Success"] = "Pedido atualizado com sucesso.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Erro ao atualizar o pedido. Por favor, tente novamente.";
                }
            }
            return View(pedidoAtualizado);
        }
        [HttpGet]
        public IActionResult LoadPagamentoPartial(int id)
        {
            var pedido = _context.DbSet_Pedido.Include(p=> p.ItensPedido).ThenInclude(p => p.Produto).FirstOrDefault(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound();
            }

            return PartialView("_PagamentoPartial", pedido);
        }

        [HttpPost]
        [Authorize]
        public IActionResult PagarPedido(Pedido pedido, string? stripeToken, string? mbwayNumber)
        {

            // Verificar se o pedido existe no banco de dados
            var pedidoExistente = _context.DbSet_Pedido.FirstOrDefault(p => p.Id == pedido.Id);

            if (pedidoExistente == null)
            {
                TempData["Error"] = "Pedido não encontrado.";
                return RedirectToAction("Index");
            }

            try
            {
                ProcessPayment(pedidoExistente.Id,stripeToken,mbwayNumber);
                // Redirecionar para o método ProcessPayment, passando os parâmetros necessários
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao redirecionar para o processamento do pagamento");
                TempData["Error"] = "Erro ao redirecionar para o processamento do pagamento. Por favor, tente novamente.";
                return RedirectToAction("Index");
            }
        }



        // Método auxiliar para finalizar o pedido com sucesso
        private IActionResult FinalizarPedidoComSucesso(Pedido pedido)
        {
            // Verificar se o pedido tem itens
            if (pedido.ItensPedido == null || !pedido.ItensPedido.Any())
            {
                TempData["Error"] = "Não foi possível finalizar o pedido porque não há itens.";
                return RedirectToAction("Carrinho");
            }

            // Calcular o número do pedido
            var ultimoPedido = _context.DbSet_Pedido.OrderByDescending(p => p.Id).FirstOrDefault();
            int novoNumeroPedido = (ultimoPedido?.NumPedido != null) ? int.Parse(ultimoPedido.NumPedido) + 1 : 1;
            pedido.NumPedido = novoNumeroPedido.ToString();
            pedido.DataPedido = DateTime.Now;
            // Calcular total do pedido e a quantidade total de itens
            pedido.PrecoFinal = pedido.ItensPedido.Sum(item => item.Preco * item.Quantidade);
            pedido.QuantItens = pedido.ItensPedido.Sum(item => item.Quantidade);
            pedido.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Verificar se todos os valores estão corretos
            if (pedido.PrecoFinal == 0 || pedido.QuantItens == 0)
            {
                TempData["Error"] = "Erro ao finalizar o pedido. Verifique os itens no carrinho.";
                return RedirectToAction("Carrinho");
            }
            pedido.Status = "Pagamento efetuado com sucesso, a preparar a encomenda!";
            // Salvar o pedido no banco de dados
            _context.DbSet_Pedido.Add(pedido);
            _context.SaveChanges();
            //GerarFatura(pedido);

            // Limpar o carrinho após o pagamento bem-sucedido
            HttpContext.Session.Remove("produtos");

            // Retornar a View de confirmação de pedido com os dados do pedido
            return RedirectToAction("PedidoSucesso", "PedidosCliente", new { id = pedido.Id });
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            ViewBag.StripePublicKey = _stripeSettings.PublishableKey; // Certifique-se de que esta chave existe no appsettings.json
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
                ItensPedido = itensPedido
            };

            return View(pedido);
        }
        [HttpGet]
        public IActionResult Pagamento(int id)
        {
            ViewBag.StripePublishableKey = _stripeSettings.PublishableKey;

            var pedido = _context.DbSet_Pedido
                .Include(p => p.ItensPedido)
                .ThenInclude(i => i.Produto).ThenInclude(p => p.Tag)
                .FirstOrDefault(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }
        [HttpPost]
        [Authorize]
        public IActionResult ProcessPayment(int id, string stripeToken, string mbwayNumber)
        {
            // Obter o pedido a ser processado
            var pedido = _context.DbSet_Pedido
                .Include(p => p.ItensPedido)
                .FirstOrDefault(p => p.Id == id);

            if (pedido == null)
            {
                TempData["Error"] = "Pedido não encontrado.";
                return RedirectToAction("Index");
            }

            try
            {
                // Processar pagamento de acordo com o método selecionado
                switch (pedido.TipoPagamento)
                {
                    case "stripe":
                        // Processar pagamento via Stripe
                        var options = new ChargeCreateOptions
                        {
                            Amount = (long)(pedido.PrecoFinal * 100), // Convertendo para centavos
                            Currency = "eur",
                            Description = $"Pagamento do pedido {pedido.NumPedido}",
                            Source = stripeToken, // Token gerado pelo Stripe.js
                        };

                        var service = new ChargeService();
                        Charge charge = service.Create(options);

                        if (charge.Status != "succeeded")
                        {
                            pedido.Status = "Pagamento efetuado,estamos a preparar o seu pedido!";
                            TempData["Error"] = "Erro ao processar o pagamento. Tente novamente.";
                            return RedirectToAction("Index");
                        }
                        break;

                    case "mbway":
                        // Criar um Payment Intent para MB Way
                        var paymentIntent = CreateMbWayPayment(pedido, mbwayNumber);
                        if (paymentIntent.Status != "succeeded")
                        {
                            TempData["Error"] = "Erro ao processar o pagamento MB Way. Tente novamente.";
                            return RedirectToAction("Index");
                        }

                        // Atualizar o status do pedido
                        pedido.Status = "Pagamento Efetuado";
                        break;

                    case "referencia":
                        // Lógica para processar pagamento com referência multibanco
                        // Aqui você pode apenas registrar o pagamento e informar ao usuário que deve fazer o pagamento
                        break;

                    default:
                        TempData["Error"] = "Método de pagamento desconhecido.";
                        return RedirectToAction("Index");
                }

                // Atualizar o status do pedido para "Pago"
                pedido.Status = "Pagamento Efetuado";
                _context.SaveChanges();

                TempData["Success"] = "Pagamento realizado com sucesso!";
                return RedirectToAction("PedidoSucesso", new { numPedido = pedido.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar o pagamento");
                TempData["Error"] = "Erro inesperado ao processar o pagamento. Por favor, tente novamente.";
                return RedirectToAction("Index");
            }
        }

        private PaymentIntent CreateMbWayPayment(Pedido pedido, string mbwayNumber)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(pedido.PrecoFinal * 100), // Valor em centavos
                Currency = "eur",
                PaymentMethodTypes = new List<string> { "mbway" },
                ReceiptEmail = pedido.Email, // opcional: email para enviar recibo
                Metadata = new Dictionary<string, string>
        {
            { "pedido_numero", pedido.NumPedido }
        },
            };

            var service = new PaymentIntentService();
            return service.Create(options);
        }
        public IActionResult GerarFatura(int id)
        {
            var pedido = _context.DbSet_Pedido
                .Include(p => p.ItensPedido)
                .ThenInclude(i => i.Produto)
                .FirstOrDefault(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound();
            }

            // Define o caminho para salvar a fatura
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Faturas");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath); // Cria o diretório se não existir
            }

            string filePath = Path.Combine(folderPath, $"Fatura_{pedido.NumPedido}.pdf");

            // Verifica se a fatura já existe
            if (System.IO.File.Exists(filePath))
            {
                // Se a fatura já existir, retorna o arquivo existente
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/pdf", $"Fatura_{pedido.NumPedido}.pdf");
            }

            using (var stream = new MemoryStream())
            {
                // Criação do documento PDF
                PdfWriter writer = new PdfWriter(stream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Adicionando logotipo e dados da empresa lado a lado
                Table headerTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 3 })).UseAllAvailableWidth();

                // Adicionando logotipo
                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Imagens/No-Image.png"); // Altere o caminho para o seu logotipo
                Image logo = new Image(ImageDataFactory.Create(logoPath)).SetWidth(100); // Ajuste o tamanho da imagem conforme necessário

                // Dados da empresa
                Cell logoCell = new Cell().Add(logo).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                Cell infoCell = new Cell().Add(new Paragraph("Nome da Empresa\n")
                        .SetBold()
                        .SetFontSize(12)
                        .SetMarginBottom(5))
                    .Add(new Paragraph("Morada da Empresa\n")
                        .SetFontSize(12)
                        .SetMarginBottom(5))
                    .Add(new Paragraph("Contactos: 1234-5678\n")
                        .SetFontSize(12));

                headerTable.AddCell(logoCell);
                headerTable.AddCell(infoCell);

                // Adicionando a tabela de cabeçalho ao documento
                document.Add(headerTable);

                // Título da fatura
                document.Add(new Paragraph("Fatura do Pedido")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(24)
                    .SetBold()
                    .SetMarginBottom(10)
                    .SetFontColor(ColorConstants.BLUE));

                // Informações do pedido
                document.Add(new Paragraph($"Número do Pedido: {pedido.NumPedido}")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(12));
                document.Add(new Paragraph($"Data do Pedido: {pedido.DataPedido.ToShortDateString()}")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(12));
                document.Add(new Paragraph($"Nome: {pedido.Nome}")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(12));
                document.Add(new Paragraph($"Email: {pedido.Email}")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(12));
                document.Add(new Paragraph($"Morada: {pedido.Morada}")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(12));
                document.Add(new Paragraph($"Total: {pedido.PrecoFinal}€")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(12)
                    .SetBold()
                    .SetMarginBottom(20));

                // Tabela estilizada para itens do pedido
                Table table = new Table(UnitValue.CreatePercentArray(new float[] { 4, 2, 2 }))
                    .UseAllAvailableWidth()
                    .SetMarginBottom(20);

                // Adicionando cabeçalho da tabela
                table.AddHeaderCell(new Cell().Add(new Paragraph("Produto").SetBold()).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Quantidade").SetBold()).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Preço").SetBold()).SetBackgroundColor(ColorConstants.LIGHT_GRAY));

                // Adicionando itens à tabela
                foreach (var item in pedido.ItensPedido)
                {
                    table.AddCell(item.Produto.Nome);
                    table.AddCell(item.Quantidade.ToString());
                    table.AddCell($"{item.Preco}€");
                }

                // Adiciona a tabela ao documento
                document.Add(table);

                // Rodapé com o nome da empresa
                document.Add(new Paragraph("Nome da Empresa")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(12)
                    .SetMarginTop(20)
                    .SetBold());

                document.Add(new Paragraph("Obrigado pela sua compra!")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(12)
                    .SetMarginTop(10));

                // Fechar o documento
                document.Close();

                // Salvar o PDF gerado na pasta "Faturas"
                System.IO.File.WriteAllBytes(filePath, stream.ToArray());

                // Retornando o PDF gerado
                return File(stream.ToArray(), "application/pdf", $"Fatura_{pedido.NumPedido}.pdf");
            }
        }



    }
}

