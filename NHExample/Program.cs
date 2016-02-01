using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHExample.Domain;
using NHExample.Interface;
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
                using (var transaction = session.BeginTransaction())
                {
                    doit(session);
                    transaction.Commit();
                }
            }
        }

        static void Main()
        {
            var sessionFactory = Init();

            sessionFactory.DoOnSession(session =>
            {
                var vendors = new List<Vendor>
                {
                    CreateVendor("Apple", "Berlin"),
                    CreateVendor("Banana", "HH"),
                    CreateVendor("Peach", "Köln")
                };

                var random = new Random(45);

                vendors.ForEach(item => session.Save(item));

                Enumerable.Range(1, 10).ForEach(index => session.Save(CreateProduct(index, vendors[random.Next(vendors.Count)])));
            });

           
            DumpAll<Product>(sessionFactory);
            DumpAll<Vendor>(sessionFactory);

            sessionFactory.DoOnSession(session =>
            {
                var allProducts = All<Product>(session);
                allProducts.Single(product => product.Name.EndsWith("5")).Name += "_";
            });

            DumpAll<Product>(sessionFactory);

            // Don't close the application right away, so we can read
            // the output.
            Console.ReadLine();
        }

        private static ISessionFactory Init()
        {
            const string dbName = "products.db";
            return Fluently.Configure()
                .Database(SQLiteConfiguration.Standard.UsingFile(dbName).ShowSql())
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Product>())
                .ExposeConfiguration(BuildSchema)
                .BuildSessionFactory();
        }

        private static void BuildSchema(Configuration config)
        {
            const string DbFile = "nhibernate.db";
            // delete the existing db on each run
            if (File.Exists(DbFile))
            {
                File.Delete(DbFile);
            }

            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            new SchemaExport(config).Create(false, true);
        }

        private static void DumpAll<T>(ISessionFactory sessionFactory) where T : IHasName
        {
            Console.WriteLine();
            sessionFactory.DoOnSession(session =>
            {
                var allProducts = All<T>(session);
                allProducts.ForEach(p => Console.WriteLine(p.Name));
            });
            Console.WriteLine();
        }

        private static List<T> All<T>(ISession session)
        {
            var query = session.CreateQuery("FROM " + typeof(T).Name);
            return query.List<T>().ToList();
        }

        private static Product CreateProduct(int index, Vendor vendor)
        {
            return new Product
            {
                Name = "Some C# Book Volume " + index,
                Price = 500,
                Category = "Books",
                Vendor = vendor
            };
        }

        private static Vendor CreateVendor(string name, string city)
        {
            return new Vendor
            {
                Name = name,
                City = city
            };
        }
    }
}
