﻿using E_Commerce_C__ASP.NET.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_C__ASP.NET.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<TiposProduto> DbSet_TiposProduto { get; set;}
        public DbSet<SpecialTag> DbSet_Tags { get; set;}
        public DbSet<Produto> DbSet_Produto { get; set; }
        public DbSet<Pedido> DbSet_Pedido { get; set; }
        public DbSet<ItemPedido> DbSet_ItemPedido { get; set; }
        public DbSet<Pagamento> DbSet_Pagamento{ get; set; }
        
        

    }
}
