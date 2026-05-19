using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using diplomdemo.Models;
using Microsoft.EntityFrameworkCore;

namespace diplomdemo;

public partial class ProductHistoryWindow : Window
{
    private User _user;
    public ProductHistoryWindow()
    {
        InitializeComponent();
        LoadHistory();
    }
    public ProductHistoryWindow(User user)
    {
        InitializeComponent();
        _user = user;
        LoadHistory();
    }

    private void LoadHistory()
    {
        var context = new OrbitContext();
        var history = context.ProductHistories.Include(x => x.Product).ToList();
        SalesTable.ItemsSource = history;
    }

    private void Back_Click(object? sender, RoutedEventArgs e)
    {
        new ProductWin(_user).Show();
        Close();
    }
}