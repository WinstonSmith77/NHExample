using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHExample.Domain
{
    public class Vendor : ICloneable
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string City { get; set; }
        public virtual ISet<Product> Products { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ City.GetHashCode() ^ Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Vendor;
            if (other == null)
            {
                return false;
            }

            return other.Id == Id && other.Name == Name && other.City == City;
        }

        public virtual object Clone()
        {
            return new Vendor
            {
                Id = Id,
                Name = Name,
                City = City,
                Products = Products
            };
        }
    }
}
