using System;
using System.Collections.Generic;

namespace diplomdemo.Models;

public partial class SellingPoint
{
    public int SellPointId { get; set; }

    public string SellPointName { get; set; } = null!;

    public virtual ICollection<PointSale> PointSales { get; set; } = new List<PointSale>();
}
