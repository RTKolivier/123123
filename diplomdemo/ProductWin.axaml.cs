using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using diplomdemo.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Internal;

namespace diplomdemo;

public partial class ProductWin : Window
{
    User _user;
    public ProductWin()
    {
        InitializeComponent();
        
    }
    public ProductWin(User user)
    {
        InitializeComponent();
        OrbitContext context = new OrbitContext();
        _user = user;
        UserBlock.Text = _user.UserName;
        PullCombo();
        GetInfo();
    }
   
    private void Back_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindow window = new MainWindow();
        window.Show();
        this.Close();
    }
    
    public void GetInfo()
    {
        OrbitContext context = new OrbitContext();
        var product = context.Products
            .Include(x=>x.ProductFandomNavigation)
            .Include(x=>x.ProductSizeNavigation)
            .Include(x=>x.ProductTypeNavigation)
            .ToList();

        if (SearchBox.Text != null)
        {
            product = product.Where(x => x.ProductFandomNavigation.FandomsName.Contains(SearchBox.Text) || 
            x.ProductSizeNavigation.ProductSizeName.Contains(SearchBox.Text) ||
            x.ProductTypeNavigation.ProductTypeName.Contains(SearchBox.Text)||
            x.ProductName.Contains(SearchBox.Text))
            .ToList();
        }
        if (FilterCombo.SelectedIndex != -1 && FilterCombo.SelectedIndex != 0)
        {
            product = product.Where(x => x.ProductFandomNavigation.FandomsName == FilterCombo.SelectedItem!.ToString()).ToList();
        }
        switch (SortingCombo.SelectedIndex)
        {
            case 0:
                product = product.OrderBy(x => x.ProductQuantity).ToList();
                break;
            case 1:
                product = product.OrderByDescending(x => x.ProductQuantity).ToList();
                break;
        }

        ProductList.ItemsSource = product;
    }
    private void AddProduct_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AddProduct window = new AddProduct();
        window.Show();
        this.Close();
    }
    private void SearchBox_KeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        GetInfo();
    }
    private void SortingCombo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        GetInfo();
    }
    private void PullCombo()
    {
        OrbitContext context = new OrbitContext();
        var combos = context.Fandoms.Select(x => x.FandomsName).ToList();
        combos.Add("Все фандомы");

        FilterCombo.ItemsSource = combos.OrderByDescending(x => x == "Все фандомы");
    }
}