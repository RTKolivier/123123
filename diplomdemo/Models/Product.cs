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

    public int? ProductCost { get; set; }

    public int? ProductFandom { get; set; }

    public int? ProductSale { get; set; }
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

    public string? ProductPhoto { get; set; }
    public Bitmap GetPhoto
    {
        get
        {
            if (ProductPhoto != null && ProductPhoto != "")
            {
                return new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "/" + ProductPhoto);
            }
            else
            {
                return new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "/Images/111.png");
            }
        }
    }

    public virtual Fandom? ProductFandomNavigation { get; set; }

    public virtual ProductSize? ProductSizeNavigation { get; set; }

    public virtual ProductType? ProductTypeNavigation { get; set; }
}
