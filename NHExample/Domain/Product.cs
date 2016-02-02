using System;

namespace NHExample.Domain
{
    public class Product : ICloneable
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Category { get; set; }
        public virtual int Price { get; set; }
        public virtual Vendor Vendor { get; set; }


        public override string ToString()
        {
            return Name;
        }


        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Category.GetHashCode() ^ Id.GetHashCode() ^ Price.GetHashCode() ^ Vendor.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Product;
            if (other == null)
            {
                return false;
            }

            return other.Id == Id && other.Name == Name && other.Category == Category && other.Price == Price && other.Vendor == Vendor;
        }

        public virtual object Clone()
        {
            return new Product
            {
                Id = Id,
                Name = Name,
                Category = Category,
                Vendor = (Vendor)Vendor.Clone()
            };
        }
    }
}
