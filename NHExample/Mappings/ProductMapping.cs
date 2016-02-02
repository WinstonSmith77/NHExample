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
            Id(x => x.Id).GeneratedBy.Native(builder => builder.AddParam("sequence", "SEQ_LIST_DEF"));
            Map(x => x.Category);
            Map(x => x.Name);
            Map(x => x.Price);
            References(x => x.Vendor).Cascade.All();
        }
    }
}
