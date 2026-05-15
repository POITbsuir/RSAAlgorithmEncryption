using System;
using System.Windows;
using System.Windows.Input;
using ThirdTaskApplication.ViewModels;

namespace ThirdTaskApplication
{
    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ApplicationMain();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !long.TryParse(e.Text, out _);
        }

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
