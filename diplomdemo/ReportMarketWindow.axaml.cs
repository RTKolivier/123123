using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using diplomdemo.Models;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using LiveChartsCore.Kernel;
using SkiaSharp;

namespace diplomdemo;

public partial class ReportMarketWindow : Window
{
    private User? _user;
    private readonly ObservableCollection<ObservablePoint> _salesData = new();
    private readonly ObservableCollection<SaleTableRow> _salesTable = new();
    private readonly List<Market> _markets = new();
    private Product? _selectedProduct;

    public ReportMarketWindow()
    {
        InitializeComponent();
        SalesTable.ItemsSource = _salesTable;
        InitializeChart();
        LoadMarkets();
        LoadGroupedProducts();
    }

    public ReportMarketWindow(User user) : this()
    {
        _user = user;
    }

    private void InitializeChart()
    {
        SalesChart.Series = new ISeries[]
        {
            new LineSeries<ObservablePoint>
            {
                Values = _salesData,
                Name = "Сумма продаж по маркетам",
                Stroke = new SolidColorPaint(SKColors.Orange) { StrokeThickness = 2 },
                Fill = null,
                GeometrySize = 8,
                GeometryStroke = new SolidColorPaint(SKColors.Orange),
                GeometryFill = new SolidColorPaint(SKColors.Orange),
                XToolTipLabelFormatter = (chartPoint) =>
                {
                    if (chartPoint?.Context?.Entity is not ObservablePoint point) 
                        return string.Empty;
                
                    var date = DateTime.FromOADate((double)point.X);
                    var market = _markets.FirstOrDefault(m => 
                        m.MarketDate?.ToDateTime(TimeOnly.MinValue).ToOADate() == point.X);
                
                    return market?.MarketName ?? date.ToString("dd.MM.yyyy");
                },
                YToolTipLabelFormatter = (chartPoint) =>
                {
                    if (chartPoint?.Context?.Entity is not ObservablePoint point)
                        return string.Empty;
                    
                    return $"Сумма: {point.Y:F2} руб.";
                }
            }
        };

        SalesChart.XAxes = new Axis[]
        {
            new Axis
            {
                Labeler = value => DateTime.FromOADate(value).ToString("dd.MM.yy"),
                TextSize = 12,
                Name = "Дата"
            }
        };

        SalesChart.YAxes = new Axis[]
        {
            new Axis
            {
                TextSize = 12,
                Name = "Сумма продаж, руб."
            }
        };

        SalesChart.LegendPosition = LegendPosition.Right;
    }

    private Market? GetSelectedMarket()
    {
        if (MarketCombo.SelectedIndex < 0 || MarketCombo.SelectedIndex >= _markets.Count)
            return null;

        return _markets[MarketCombo.SelectedIndex];
    }

    private void LoadMarkets()
    {
        using var context = new OrbitContext();
        _markets.Clear();
        _markets.AddRange(context.Markets.OrderByDescending(m => m.MarketDate).ToList());
        MarketCombo.ItemsSource = _markets.Select(m => m.MarketName).ToList();

        if (_markets.Count > 0)
        {
            MarketCombo.SelectedIndex = 0;
            UpdateMarketInfo(_markets[0]);
        }
        else
        {
            UpdateMarketInfo(null);
            LoadSalesData(null);
        }
    }

    private void UpdateMarketInfo(Market? market)
    {
        if (market is null)
        {
            MarketDateText.Text = string.Empty;
            return;
        }

        var dateText = market.MarketDate?.ToString("dd.MM.yyyy") ?? "дата не указана";
        MarketDateText.Text = $"Дата мероприятия: {dateText}";
    }

    private void LoadGroupedProducts()
    {
        using var context = new OrbitContext();
        var search = ProductSearchBox.Text?.Trim() ?? string.Empty;

        var products = context.Products
            .Include(p => p.ProductTypeNavigation)
            .Include(p => p.ProductFandomNavigation)
            .AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            products = products.Where(p =>
                (p.ProductName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (p.ProductTypeNavigation?.ProductTypeName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (p.ProductFandomNavigation?.FandomsName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        var groups = products
            .GroupBy(p => p.ProductTypeNavigation?.ProductTypeName ?? "Прочее")
            .OrderBy(g => g.Key)
            .Select(g => new ProductGroupView
            {
                GroupName = g.Key,
                Products = g.OrderBy(p => p.ProductName).ToList()
            })
            .ToList();

        GroupedProducts.ItemsSource = groups;
    }

    private void LoadChartData()
    {
        _salesData.Clear();

        using var context = new OrbitContext();

        var markets = context.Markets
            .Where(m => m.MarketDate != null)
            .OrderBy(m => m.MarketDate)
            .ToList();

        foreach (var market in markets)
        {
            var total = context.MarketSells
                .Where(ms => ms.MarketId == market.MarketId)
                .Sum(ms => (double)ms.MarketProductAmmount);

            if (total <= 0)
                continue;

            var oaDate = market.MarketDate!.Value.ToDateTime(TimeOnly.MinValue).ToOADate();
            _salesData.Add(new ObservablePoint(oaDate, total));
        }
    }

    private void LoadTableData(int? marketId)
    {
        _salesTable.Clear();

        using var context = new OrbitContext();

        var query = context.MarketSells
            .Include(ms => ms.Market)
            .Include(ms => ms.Product)
            .AsQueryable();

        if (marketId is not null)
            query = query.Where(ms => ms.MarketId == marketId);

        var sales = query
            .OrderByDescending(ms => ms.Market.MarketDate)
            .ThenBy(ms => ms.Product.ProductName)
            .ToList();

        foreach (var sale in sales)
        {
            _salesTable.Add(new SaleTableRow
            {
                SaleDate = sale.Market.MarketDate?.ToString("dd.MM.yyyy") ?? "—",
                MarketName = sale.Market.MarketName,
                ProductName = sale.Product.ProductName ?? "—",
                Quantity = sale.MarketProductCount,
                TotalAmount = sale.MarketProductAmmount
            });
        }
    }

    private void LoadSalesData(int? marketId)
    {
        LoadChartData();
        LoadTableData(marketId);
    }

    private void MarketCombo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var market = GetSelectedMarket();
        UpdateMarketInfo(market);
        LoadSalesData(market?.MarketId);
    }

    private void ProductSearchBox_KeyUp(object? sender, KeyEventArgs e) => LoadGroupedProducts();

    private void ProductList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListBox listBox || listBox.SelectedItem is not Product product)
            return;

        _selectedProduct = product;
        SelectedProductText.Text = $"Выбран: {product.ProductName}";
    }

    private void AddMarketSell_Click(object? sender, RoutedEventArgs e)
    {
        var market = GetSelectedMarket();
        if (market is null)
            return;

        if (_selectedProduct is null)
            return;

        if (!int.TryParse(QuantityBox.Text?.Trim(), out var count) || count <= 0)
            return;

        using var context = new OrbitContext();
        var product = context.Products.FirstOrDefault(p => p.ProductId == _selectedProduct.ProductId);
        var marketEntity = context.Markets.FirstOrDefault(m => m.MarketId == market.MarketId);

        if (product is null || marketEntity is null)
            return;

        var unitPrice = product.ProductCost ?? 0;
        var totalAmount = count * unitPrice;

        var existing = context.MarketSells
            .FirstOrDefault(ms => ms.MarketId == market.MarketId && ms.ProductId == product.ProductId);

        if (existing is not null)
        {
            existing.MarketProductCount += count;
            existing.MarketProductAmmount += totalAmount;
        }
        else
        {
            context.MarketSells.Add(new MarketSell
            {
                MarketId = market.MarketId,
                ProductId = product.ProductId,
                MarketProductCount = count,
                MarketProductAmmount = totalAmount
            });
        }

        marketEntity.MarketSumm = context.MarketSells
            .Where(ms => ms.MarketId == market.MarketId)
            .Sum(ms => ms.MarketProductAmmount);

        context.SaveChanges();

        QuantityBox.Text = string.Empty;
        _selectedProduct = null;
        SelectedProductText.Text = "Товар не выбран";
        LoadSalesData(market.MarketId);
    }

    private void Back_Click(object? sender, RoutedEventArgs e)
    {
        if (_user is not null)
            new ProductWin(_user).Show();
        else
            new ProductWin().Show();
        Close();
    }

    private sealed class SaleTableRow
    {
        public string SaleDate { get; init; } = string.Empty;
        public string MarketName { get; init; } = string.Empty;
        public string ProductName { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public int TotalAmount { get; init; }
    }

    private sealed class ProductGroupView
    {
        public string GroupName { get; init; } = string.Empty;
        public List<Product> Products { get; init; } = new();
    }

    private void CreateMarketClick(object? sender, RoutedEventArgs e)
    {
        new AddMarketWindow(_user).ShowDialog(this);
        
        LoadMarkets();
        
    }
}
