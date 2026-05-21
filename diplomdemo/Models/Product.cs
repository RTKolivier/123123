using System;
using System.Collections.Generic;
using Avalonia.Media.Imaging;

namespace diplomdemo.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public int? ProductType { get; set; }

    public int? ProductSize { get; set; }

    public string? ProductName { get; set; }

    public int? ProductQuantity { get; set; }

    public int? ProductCost { get; set; }

    public int? ProductFandom { get; set; }

    public int? ProductSale { get; set; }

    public string? ProductPhoto { get; set; }
    
    public string ColorQuantity
    {
        get
        {
            if (ProductQuantity == 0)
            {
                return "#8ac8ff";
            }
            else
            {
                return "";
            }
        }
    }
    
    public string ColorDiscount
    {
        get
        {
            if (ProductSale > 15)
            {
                return "#2E8B57";
            }
            else
            {
                return "";
            }
        }
    }
    
    public Bitmap GetPhoto
    {
        get
        {
            if (ProductPhoto != null && ProductPhoto != "")
            {
                return new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "/Images/" + ProductPhoto);
            }
            else
            {
                return new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "/Images/111.png");
            }
        }
    }

    public virtual ICollection<MarketSell> MarketSells { get; set; } = new List<MarketSell>();

    public virtual ICollection<PointSale> PointSales { get; set; } = new List<PointSale>();

    public virtual Fandom? ProductFandomNavigation { get; set; }

    public virtual ICollection<ProductHistory> ProductHistories { get; set; } = new List<ProductHistory>();

    public virtual ProductSize? ProductSizeNavigation { get; set; }

    public virtual ProductType? ProductTypeNavigation { get; set; }
}
