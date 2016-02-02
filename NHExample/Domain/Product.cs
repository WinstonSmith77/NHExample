﻿using System;

namespace NHExample.Domain
{
    public class Product 
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
    }
}
