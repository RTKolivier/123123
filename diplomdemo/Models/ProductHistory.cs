using System;
using System.Collections.Generic;

namespace diplomdemo.Models;

public partial class ProductHistory
{
    public int HistoryId { get; set; }

    public int ProductId { get; set; }

    public int ProductQuantity { get; set; }

    public DateTime AddedDate { get; set; }

    public virtual Product Product { get; set; } = null!;
}
