using System;
using System.Collections.Generic;

namespace diplomdemo.Models;

public partial class Fandom
{
    public int FandomsId { get; set; }

    public string? FandomsName { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
