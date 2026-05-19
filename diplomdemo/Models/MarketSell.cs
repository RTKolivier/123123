using System;
using System.Collections.Generic;

namespace diplomdemo.Models;

public partial class MarketSell
{
    public int MarketId { get; set; }

    public int ProductId { get; set; }

    public int MarketProductCount { get; set; }

    public int MarketProductAmmount { get; set; }

    public virtual Market Market { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
