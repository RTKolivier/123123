using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using diplomdemo.Models;

namespace diplomdemo;

public partial class AddMarketWindow : Window
{
    private User _user;
    public AddMarketWindow()
    {
        InitializeComponent();
    }
    
    public AddMarketWindow(User user)
    {
        InitializeComponent();
        _user = user;
    }

    private void BackClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void SaveClick(object? sender, RoutedEventArgs e)
    {
        var context = new OrbitContext();
        var date = DatePicker.SelectedDate.ToString().Split(" ");

        var newMarket = new Market
        {
            MarketName = MarketBox.Text,
            MarketDate = DateOnly.Parse(date[0]),
            MarketSumm = 0
        };
        
        context.Markets.Add(newMarket);
        context.SaveChanges();
        
        Close();
    }
}