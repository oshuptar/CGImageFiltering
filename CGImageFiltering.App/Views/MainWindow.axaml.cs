using Avalonia.Controls;
using CGImageFiltering.App.ViewModels;

namespace CGImageFiltering.App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        DataContext = new MainWindowViewModel();
        InitializeComponent();
    }
}