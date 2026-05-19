using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using diplomdemo.Models;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;

namespace diplomdemo;

public partial class ReportShelfWindow : Window
{
    User _user;
    private readonly ObservableCollection<ObservablePoint> _salesData = new();
    private readonly ObservableCollection<SaleTableRow> _salesTable = new();

    public ReportShelfWindow()
    {
        InitializeComponent();
        SalesTable.ItemsSource = _salesTable;
        InitializeChart();
        LoadSalesData();
    }
    public ReportShelfWindow(User user)
    {
        InitializeComponent();
        _user = user;
        SalesTable.ItemsSource = _salesTable;
        InitializeChart();
        LoadSalesData();
    }

    private void InitializeChart()
    {
        SalesChart.Series = new ISeries[]
        {
            new LineSeries<ObservablePoint>
            {
                Values = _salesData,
                Name = "Сумма продаж",
                Stroke = new SolidColorPaint(SKColors.DodgerBlue) { StrokeThickness = 2 },
                Fill = null,
                GeometrySize = 8,
                GeometryStroke = new SolidColorPaint(SKColors.DodgerBlue),
                GeometryFill = new SolidColorPaint(SKColors.DodgerBlue)
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

    private void LoadSalesData()
    {
        _salesData.Clear();
        _salesTable.Clear();

        using var context = new OrbitContext();

        var sales = context.PointSales
            .Include(s => s.Product)
            .OrderByDescending(s => s.SaleTimestamp)
            .ThenBy(s => s.Product.ProductName)
            .ToList();

        foreach (var sale in sales)
        {
            _salesTable.Add(new SaleTableRow
            {
                SaleDate = sale.SaleTimestamp.ToString("dd.MM.yyyy"),
                ProductName = sale.Product.ProductName ?? "—",
                Quantity = sale.PointProductCount,
                TotalAmount = sale.PointProductAmmount
            });
        }

        var salesByDate = sales
            .GroupBy(s => s.SaleTimestamp)
            .Select(g => new
            {
                Date = g.Key,
                Total = g.Sum(s => (double)s.PointProductAmmount)
            })
            .OrderBy(x => x.Date);

        foreach (var item in salesByDate)
        {
            var oaDate = item.Date.ToDateTime(TimeOnly.MinValue).ToOADate();
            _salesData.Add(new ObservablePoint(oaDate, item.Total));
        }
    }

    private void CreateSell(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ListProducts.Text))
            return;

        using var context = new OrbitContext();
        var sellPointId = context.SellingPoints
            .Select(sp => sp.SellPointId)
            .FirstOrDefault();

        if (sellPointId == 0)
            return;

        var lines = ListProducts.Text
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var separatorIndex = line.LastIndexOf('-');
            if (separatorIndex <= 0)
                continue;

            var productName = line[..separatorIndex].Trim();
            if (!int.TryParse(line[(separatorIndex + 1)..].Trim(), out var count) || count <= 0)
                continue;

            var product = context.Products
                .FirstOrDefault(p => p.ProductName == productName);

            if (product is null)
                continue;

            var unitPrice = product.ProductCost ?? 0;

            context.PointSales.Add(new PointSale
            {
                SellPointId = sellPointId,
                ProductId = product.ProductId,
                PointProductCount = count,
                PointProductAmmount = count * unitPrice,
                SaleTimestamp = DateOnly.FromDateTime(DateTime.Today)
            });
        }

        context.SaveChanges();
        ListProducts.Text = string.Empty;
        LoadSalesData();
    }

    private void Back_Click(object? sender, RoutedEventArgs e)
    {
        new ProductWin(_user).Show();
        Close();
    }

    private sealed class SaleTableRow
    {
        public string SaleDate { get; init; } = string.Empty;
        public string ProductName { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public int TotalAmount { get; init; }
    }
}
