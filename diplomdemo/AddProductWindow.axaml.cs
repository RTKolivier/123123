using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using diplomdemo.Models;

namespace diplomdemo;

public partial class AddProductWindow : Window
{
    public AddProductWindow()
    {
        InitializeComponent();
    }
    
    private User _user;
    public AddProductWindow(User user)
    {
        InitializeComponent();
        _user = user;
        LoadData();
    }

    private void LoadData()
    {
        var context = new OrbitContext();
        ProductType.ItemsSource = context.ProductTypes.Select(x => x.ProductTypeName).ToList();
        ProductSize.ItemsSource = context.ProductSizes.Select(x => x.ProductSizeName).ToList();
        ProductFandom.ItemsSource = context.Fandoms.Select(x => x.FandomsName).ToList();
    }

    private void CreateFandom_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var newFandom = new Fandom { FandomsName = "Новый фандом" };
        var context = new OrbitContext();
        context.Fandoms.Add(newFandom);
        context.SaveChanges();
        ProductFandom.ItemsSource = context.Fandoms.Select(x => x.FandomsName).ToList();
    }

    private void AddProduct_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var context = new OrbitContext();
        var product = new Product
        {
            ProductName = ProductName.Text,
            ProductType = context.ProductTypes.First(x => x.ProductTypeName == ProductType.SelectedItem.ToString()).ProductTypeId,
            ProductSize = context.ProductSizes.First(x => x.ProductSizeName == ProductSize.SelectedItem.ToString()).ProductSizeId,
            ProductQuantity = int.Parse(ProductQuantity.Text),
            ProductCost = int.Parse(ProductCost.Text),
            ProductFandom = context.Fandoms.First(x => x.FandomsName == ProductFandom.SelectedItem.ToString()).FandomsId,
            ProductSale = int.Parse(ProductSale.Text)
        };

        context.Products.Add(product);
        context.SaveChanges();

        new ProductWin(_user).Show();
        Close();
    }
}