using Avalonia.Controls;
using System;

namespace diplomdemo
{
    public partial class InputDialog : Window
    {
        public string Result { get; private set; }

        public InputDialog()
        {
            InitializeComponent();
        }
        public InputDialog(string prompt)
        {
            InitializeComponent();
            PromptText.Text = prompt;
        }

        private void Ok_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Result = QuantityInput.Text;
            Close(true);
        }

        private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(false);
        }
    }
}