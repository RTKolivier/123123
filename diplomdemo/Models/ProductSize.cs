using System;
using System.Collections.Generic;

namespace diplomdemo.Models;

public partial class ProductSize
{
    public int ProductSizeId { get; set; }

    public string? ProductSizeName { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
