using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHExample.Domain
{
    public class Vendor 
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string City { get; set; }
        public virtual IList<Product> Products { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
