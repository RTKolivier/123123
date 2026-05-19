using System;
using System.Collections.Generic;

namespace diplomdemo.Models;

public partial class PointSale
{
    public int SaleId { get; set; }

    public int SellPointId { get; set; }

    public int ProductId { get; set; }

    public int PointProductCount { get; set; }

    public int PointProductAmmount { get; set; }

    public DateOnly SaleTimestamp { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual SellingPoint SellPoint { get; set; } = null!;
}
