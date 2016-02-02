﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using log4net;
using log4net.Config;
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
        private static void DoOnSession(this ISessionFactory sessionFactory, Action<ISession> doit, string message)
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
            if (!string.IsNullOrEmpty(message))
            {
                const string nhSQL = "NHibernate.SQL";
                var logger = LogManager.GetLogger(nhSQL);

                logger.Debug(message);
            }
        }

        static void Main()
        {
            XmlConfigurator.Configure();

            var vendors = new List<Vendor>();
            var sessionFactory = Init();

            sessionFactory.DoOnSession(session =>
            {
                vendors.Add(CreateVendor("Apple", "Berlin"));
                vendors.Add(CreateVendor("Banana", "HH"));
                vendors.Add(CreateVendor("Peach", "Köln"));

                vendors.ForEach(item => session.Save(item));
            //}, "Create Vendor");


            //sessionFactory.DoOnSession(session =>
            //{
                var random = new Random(45);
                Enumerable.Range(1, 10).ForEach(index => session.Save(CreateProduct(index, vendors[random.Next(vendors.Count)])));
            }, "Create Product and Vendor");

            DumpAll<Product>(sessionFactory);
            DumpAll<Vendor>(sessionFactory);

            sessionFactory.DoOnSession(session =>
            {
                var allProducts = All<Product>(session);
                allProducts.Single(product => product.Name.EndsWith("5")).Name += "_";
            }, "Change");

            DumpAll<Product>(sessionFactory);

            // Don't close the application right away, so we can read
            // the output.
            Console.ReadLine();
        }


        private static ISessionFactory Init()
        {
            const string dbName = "products.db";

            if (File.Exists(dbName))
            {
                File.Delete(dbName);
            }

            return Fluently.Configure()
                .Database(SQLiteConfiguration.Standard.UsingFile(dbName).ShowSql())
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Product>())
                .ExposeConfiguration(config => new SchemaExport(config).Create(false, true))
                .BuildSessionFactory();
        }

        private static void DumpAll<T>(ISessionFactory sessionFactory)
        {
            Console.WriteLine();
            sessionFactory.DoOnSession(session =>
            {
                var allProducts = All<T>(session);
                allProducts.ForEach(p => Console.WriteLine(p.ToString()));
            }, "Dump " + typeof(T).Name);
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
