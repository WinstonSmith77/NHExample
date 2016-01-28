using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NHExample.Domain;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace NHExample
{
    static class Program
    {

        private static void DoOnSession(this ISessionFactory sessionFactory, Action<ISession> doit)
        {
            using (var session = sessionFactory.OpenSession())
            {
                doit(session);
                session.Flush();
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
                var product = CreateProduct();

                // And save it to the database
                session.Save(product);
            });

            sessionFactory.DoOnSession(session =>
            {
                var allProducts = AllProducts(session);
                allProducts.ForEach(p => Console.WriteLine(p.Name));
            });

            // Don't close the application right away, so we can read
            // the output.
            Console.ReadLine();
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

        private static Product CreateProduct()
        {
            return new Product
            {
                Name = "Some C# Book",
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
