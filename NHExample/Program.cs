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
                Enumerable.Range(1, 10).ForEach(index => session.Save(CreateProduct(index)));
            });

            sessionFactory.DoOnSession(session =>
            {
                session.Save(CreateVendor("Apple"));
                session.Save(CreateVendor("Banana"));
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
            return Fluently.Configure()
                .Database(SQLiteConfiguration.Standard.UsingFile("firstProject.db").ShowSql())
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

        private static Product CreateProduct(int index)
        {
            return new Product
            {
                Name = "Some C# Book Volume " + index,
                Price = 500,
                Category = "Books"
            };
        }

        private static Vendor CreateVendor(string name)
        {
            return new Vendor
            {
                Name = name
            };
        }
    }
}
