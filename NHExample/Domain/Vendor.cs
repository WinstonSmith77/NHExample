using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHExample.Interface;

namespace NHExample.Domain
{
    public class Vendor : IHasID, IHasName
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string City { get; set; }
        public IList<Product> Products { get; set; }
    }
}
