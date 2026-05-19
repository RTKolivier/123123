using System;
using System.Collections.Generic;

namespace diplomdemo.Models;

public partial class Market
{
    public int MarketId { get; set; }

    public string MarketName { get; set; } = null!;

    public DateOnly? MarketDate { get; set; }

    public int MarketSumm { get; set; }

    public virtual ICollection<MarketSell> MarketSells { get; set; } = new List<MarketSell>();
}
