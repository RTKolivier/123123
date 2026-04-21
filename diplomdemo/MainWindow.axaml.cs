using System.Linq;
using Avalonia.Controls;
using diplomdemo.Models;
using MsBox.Avalonia;

namespace diplomdemo
{
    /// <summary>
    /// Icon="Icon/icon.png"
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            OrbitContext context = new OrbitContext();
            var user = context.Users.FirstOrDefault(x => x.UserLogin == LoginBox.Text && x.UserPassword == PasswordBox.Text);
            if (user != null)
            {
                ProductWin window = new ProductWin(user);
                window.Show();
                this.Close();
            }
            else
            {
                var message = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Некорректные данные", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
                await message.ShowAsync();
            }
        }
        private void Reg_Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            RegWin window = new RegWin();
            window.Show();
            this.Close();
        }
    }
}