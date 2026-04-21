using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using diplomdemo.Models;

namespace diplomdemo;

public partial class RegWin : Window
{
    public RegWin()
    {
        InitializeComponent();
        PullComboBox();
    }
    private void Save_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        OrbitContext context = new OrbitContext();
        var newUser = new User
        {
            UserRole = context.Roles.Where(x=>x.RoleName == RoleBox.SelectedItem.ToString()).Select(x=>x.RoleId).FirstOrDefault(),
            UserName = NameBox.Text,
            UserPhone = PhoneBox.Text,
            UserLogin = LoginBox.Text,
            UserPassword = PasswordBox.Text
        };

        context.Users.Add(newUser);
        context.SaveChanges();

        MainWindow window = new MainWindow();
        window.Show();
        this.Close();
    }
    private void PullComboBox()
    {
        OrbitContext context = new OrbitContext();
        var role = context.Roles.Select(x => x.RoleName).ToList();

        RoleBox.ItemsSource = role;
    }
}