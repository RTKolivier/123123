using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using diplomdemo.Models;
using MsBox.Avalonia;

namespace diplomdemo;

public partial class AddProductWindow : Window
{
    private List<string> ProductTypes;
    private List<string> Fandoms;
    private string Photo;
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
        ProductTypes = context.ProductTypes.Select(x => x.ProductTypeName).ToList();
        Fandoms = context.Fandoms.Select(x => x.FandomsName).ToList();
        
        ProductType.ItemsSource = ProductTypes;
        ProductSize.ItemsSource = context.ProductSizes.Select(x => x.ProductSizeName).ToList();
        ProductFandom.ItemsSource = Fandoms;
    }

    private void AddProduct_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var context = new OrbitContext();
        if (!ProductTypes.Contains(ProductType.Text))
        {
            var newType = new ProductType
            {
                ProductTypeName = ProductType.Text
            };
            
            context.ProductTypes.Add(newType);
            context.SaveChanges();
        }

        if (!Fandoms.Contains(ProductFandom.Text))
        {
            var newFandom = new Fandom
            {
                FandomsName = ProductFandom.Text
            };
            context.Fandoms.Add(newFandom);
            context.SaveChanges();
        }
        
        var product = new Product
        {
            ProductName = ProductName.Text,
            ProductType = context.ProductTypes.First(x => x.ProductTypeName == ProductType.Text).ProductTypeId,
            ProductSize = context.ProductSizes.First(x => x.ProductSizeName == ProductSize.SelectedItem.ToString()).ProductSizeId,
            ProductQuantity = int.Parse(ProductQuantity.Text),
            ProductCost = int.Parse(ProductCost.Text),
            ProductFandom = context.Fandoms.First(x => x.FandomsName == ProductFandom.Text).FandomsId,
            ProductSale = int.Parse(ProductSale.Text)
        };
        
        if (Photo != null) product.ProductPhoto = Photo;
        
        context.Products.Add(product);
        context.SaveChanges();

        new ProductWin(_user).Show();
        Close();
    }

    private async void AddPhotoClick(object? sender, RoutedEventArgs e)
    {
        var topLevels = TopLevel.GetTopLevel(this);
        
        var files = await topLevels.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Image File",
            AllowMultiple = false
        });

        PhotoImage.Source = new Bitmap(files[0].Path.LocalPath);

        Photo = Guid.NewGuid().ToString("N") + ".png";
        File.Copy(files[0].Path.LocalPath, AppDomain.CurrentDomain.BaseDirectory + "/Images/" + Photo);
    }
}