using System;
using System.Collections.Generic;
using System.Linq;
using Curso.Data;
using Curso.Domain;
using Curso.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CursoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            /*using var db = new Curso.Data.ApplicationContext();

            var existe = db.Database.GetAppliedMigrations().Any();
            if(existe){

            }

            Console.WriteLine("Hello World!");
            */

            //InserirDados();
            //InserirDadosEmMassa();
            //InserirDadosEmLista();
            //ConsultarRegistros();
            //CadastrarPedido();
            //ConsultarPedidoCarrementoAdiantado();
            //AtualizarDados();
            RemoveRegistro();//
        }
        private static void InserirDados()
        {
            var produto = new Produto
            {
                CodigoBarras = "1234567778",
                Descricao = "Batat",
                Valor = 10m,
                TipoProduto = TipoProduto.MercadoriaParaRevenda,
                Ativo = true
            };

            using var db = new ApplicationContext();
           
            db.Produtos.Add(produto); // Os indicados
            //db.Set<Produto>().Add(produto); // Os indicados
            //db.Entry(produto).State = EntityState.Added;
            //db.Add(produto);

            var registros = db.SaveChanges();
            Console.WriteLine(registros);
        }

        private static void InserirDadosEmMassa()
        {
            var produto = new Produto
            {
                CodigoBarras = "1234567778",
                Descricao = "Batat",
                Valor = 10m,
                TipoProduto = TipoProduto.MercadoriaParaRevenda,
                Ativo = true
            };

            var cliente = new Cliente
            {
                Nome = "Rioner",
                Telefone = "14988177007",
                CEP = "19620.000",
                Cidade = "Gardênia",
                Estado = "SP"
            };

            using var db = new ApplicationContext();
           
            db.AddRange(produto, cliente);

            var registros = db.SaveChanges();
            Console.WriteLine($"Total Registros: {registros}");
        }

        private static void InserirDadosEmLista()
        {
            var listaCliente = new[]
            {
                new Cliente
                {
                    Nome = "Rogêrio",
                    Telefone = "14988177007",
                    CEP = "19620.000",
                    Cidade = "Gardênia",
                    Estado = "SP"
                },

                new Cliente{
                    Nome = "Débora",
                    Telefone = "18988177007",
                    CEP = "19600.000",
                    Cidade = "Iepe",
                    Estado = "SP"
                },
            };

            using var db = new ApplicationContext();
           
            db.AddRange(listaCliente);

            var registros = db.SaveChanges();
            Console.WriteLine($"Total Registros: {registros}");
        }
    
        private static void ConsultarRegistros()
        {
            var db = new ApplicationContext();

            //var consultaPorSintaxe = from c in db.Clientes where c.Id > 0 select c;
            var consultaPorMetado = db.Clientes.Where(c=> c.Id > 0).ToList();

            foreach (var cliente in consultaPorMetado)
            {
                Console.WriteLine($"Consultado Cliente: {cliente.Id}");
                db.Clientes.FirstOrDefault(p => p.Id == cliente.Id);
            }
        }
    
        private static void CadastrarPedido()
        {
            var db = new ApplicationContext();
            var cliente = db.Clientes.FirstOrDefault();
            var produto = db.Produtos.FirstOrDefault();

            var pedido = new Pedido
            {
                ClienteId = cliente.Id,
                IniciadoEm = DateTime.Now,
                FinalizadoEm = DateTime.Now,
                TipoFrete = TipoFrete.SemFrete,
                StatusPedido = StatusPedido.Entregue,
                Observacao = "Pedido Teste",
                itens = new List<PedidoItem>
                {
                    new PedidoItem
                    {
                        ProdutoId = produto.Id,
                        Desconto = 0,
                        Quantidade = 1,
                        Valor = 10,
                    }
                }
            };

            db.Add(pedido);

            db.SaveChanges();
        }

        private static void ConsultarPedidoCarrementoAdiantado()
        {
            var db = new ApplicationContext();
            var pedido = db.Pedidos
                            .Include(p => p.itens)
                                .ThenInclude(p => p.Produto).ToList();
        }
    
        private static void AtualizarDados(){
            var db = new ApplicationContext();
            //var cliente = db.Clientes.FirstOrDefault();
            //cliente.Nome = "Cliente Atualizado 1";
            
            var cliente = new Cliente
            {
                Id = 1
            };

            var clienteDesconectado = new 
            {
                Nome = "Cliente Atualizado 2",
                Telefone = "12345678"
            };

            db.Attach(cliente);
            db.Entry(cliente).CurrentValues.SetValues(clienteDesconectado);

            db.SaveChanges();
        }
    
        private static void RemoveRegistro(){
            var db = new ApplicationContext();

            //var cliente = db.Clientes.Find(2);
            var cliente = new Cliente {Id = 3};

            //db.Clientes.Remove(cliente);
            //db.Remove(cliente);
            db.Entry(cliente).State = EntityState.Deleted;

            db.SaveChanges();
        }
    }
}