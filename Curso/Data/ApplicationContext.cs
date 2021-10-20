using System;
using Curso.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Curso.Data
{
    public class ApplicationContext : DbContext
    {
        private static readonly ILoggerFactory _logger = LoggerFactory.Create(p => p.AddConsole());
        public DbSet<Pedido> Pedidos {get; set;}
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                         .UseLoggerFactory(_logger)
                         .EnableSensitiveDataLogging()
                         .UseSqlServer("Data source=(localdb)\\mssqllocaldb; Initial Catalog=CursoEFCore;Integrated Security=true",
                         p => p.EnableRetryOnFailure
                         (
                             maxRetryCount: 2,
                             maxRetryDelay:
                             TimeSpan.FromSeconds(5),
                             errorNumbersToAdd: null).MigrationsHistoryTable("curso_ef_core")
                         );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>(p=>
            {
                p.ToTable("Cliente"); // Nome Tabela
                p.HasKey(p => p.Id);  // Chave da Tabela
                // Campos e tipo da tabela
                p.Property(p=> p.Nome).HasColumnType("VARCHAR(60)").IsRequired();
                p.Property(p=> p.Telefone).HasColumnType("CHAR(11)");
                p.Property(p=> p.CEP).HasColumnType("CHAR(9)").IsRequired();
                p.Property(p=> p.Cidade).HasMaxLength(60).IsRequired();
                p.Property(p=> p.Estado).HasColumnType("CHAR(2)").IsRequired();

                p.HasIndex(i=> i.Telefone).HasDatabaseName("idx_cliente_telefone");
            });

            modelBuilder.Entity<Produto>(p=>
            {
                p.ToTable("Produto");
                p.HasKey(p=> p.Id);

                p.Property(p=> p.CodigoBarras).HasColumnType("VARCHAR(14)").IsRequired();
                p.Property(p=> p.Descricao).HasMaxLength(60).IsRequired();
                p.Property(p=> p.Valor).IsRequired();
                p.Property(p=> p.TipoProduto).HasConversion<string>();
                p.Property(p=> p.Ativo);
            });

            modelBuilder.Entity<Pedido>(p=>
            {
                p.ToTable("Pedido");
                p.HasKey(p=> p.Id);

                p.Property(p=> p.IniciadoEm).HasDefaultValueSql("GETDATE()").ValueGeneratedOnAdd();
                p.Property(p=> p.StatusPedido).HasConversion<string>();
                p.Property(p=> p.TipoFrete).HasConversion<int>();
                p.Property(p=> p.Observacao).HasColumnType("VARCHAR(512)");

                p.HasMany(p => p.itens)
                    .WithOne(p => p.Pedido)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PedidoItem>(p => 
            {
                p.ToTable("PedidoItem");
                p.HasKey(p => p.Id);
                p.Property(p => p.Quantidade).HasDefaultValue(1).IsRequired();
                p.Property(p => p.Valor).IsRequired();
                p.Property(p => p.Desconto).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}