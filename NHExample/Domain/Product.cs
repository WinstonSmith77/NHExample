using System;
using NHExample.Interface;

namespace NHExample.Domain
{
    public class Product : IHasID, IHasName
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Category { get; set; }
        public virtual int Price { get; set; }
    }
}
