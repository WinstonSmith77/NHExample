using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using NHExample.Domain;

namespace NHExample.Mappings
{
    public class ProductMapping : ClassMap<Product>
    {
        public ProductMapping()
        {
            Id(x => x.Id);
            Map(x => x.Category);
            Map(x => x.Name);
            Map(x => x.Price);
        }
    }
}
