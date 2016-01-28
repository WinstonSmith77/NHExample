using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NHExample.Domain;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Linq;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Util;

namespace NHExample
{
    static class Program
    {

        private static void DoOnSession(this ISessionFactory sessionFactory, Action<ISession> doit)
        {
            using (var session = sessionFactory.OpenSession())
            {
                session.FlushMode = FlushMode.Commit;
                using (var transaction = session.Transaction)
                {
                    transaction.Begin();
                    doit(session);
                    transaction.Commit();
                }
            }
        }

        static void Main()
        {
            var cfg = InitNHibernate();

            // Get ourselves an NHibernate Session
            var sessionFactory = cfg.BuildSessionFactory();

            CreateDataBase(cfg);

            sessionFactory.DoOnSession(session =>
            {
                Enumerable.Range(1, 10).ForEach(index => session.Save(CreateProduct(index)));
            });

            DumpAllProcducts(sessionFactory);

            sessionFactory.DoOnSession(session =>
            {
                var allProducts = AllProducts(session);
                allProducts.Single(product => product.Name.EndsWith("5")).Name += "_";
            });

            DumpAllProcducts(sessionFactory);

            // Don't close the application right away, so we can read
            // the output.
            Console.ReadLine();
        }

        private static void DumpAllProcducts(ISessionFactory sessionFactory)
        {
            Console.WriteLine();
            sessionFactory.DoOnSession(session =>
            {
                var allProducts = AllProducts(session);
                allProducts.ForEach(p => Console.WriteLine(p.Name));
            });
            Console.WriteLine();
        }

        private static void CreateDataBase(Configuration cfg)
        {
            // Create the database schema
            File.Delete("nhibernte.db");
            new SchemaExport(cfg).Create(true, true);
        }

        private static List<Product> AllProducts(ISession session)
        {
            var query = session.CreateQuery("FROM Product");
            return query.List<Product>().ToList();
        }

        private static Product CreateProduct(int index)
        {
            return new Product
            {
                Name = "Some C# Book Volume " + index,
                Price = 500,
                Category = "Books"
            };
        }

        private static Configuration InitNHibernate()
        {
            // Initialize NHibernate
            var cfg = new Configuration();
            cfg.Configure();
            cfg.AddAssembly(typeof(Product).Assembly);
            return cfg;
        }
    }
}
