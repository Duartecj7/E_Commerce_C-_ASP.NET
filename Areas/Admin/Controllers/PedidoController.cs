using E_Commerce_C__ASP.NET.Data;
using E_Commerce_C__ASP.NET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_C__ASP.NET.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PedidoController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PedidoController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var pedidos = _context.DbSet_Pedido
                    .Include(p => p.ItensPedido)      
                    .ThenInclude(i => i.Produto)      
                    .ToList();
            return View(pedidos);
        }
    }
}
