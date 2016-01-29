using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using NHExample.Domain;

namespace NHExample.Mappings
{
    public class VendorMapping : ClassMap<Vendor>
    {
        public VendorMapping()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.City);
        }
    }
}
