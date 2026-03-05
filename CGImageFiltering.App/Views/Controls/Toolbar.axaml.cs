using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CGImageFiltering.App.ViewModels;

namespace CGImageFiltering.App.Views.Controls;

public partial class Toolbar : UserControl
{
    public Toolbar()
    {
        InitializeComponent();
    }

    private async void OpenClickHandler(object? sender, RoutedEventArgs e)
    {
        var window = TopLevel.GetTopLevel(this) as Window;
        if (window is null) return;

        var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Image",
            AllowMultiple = false,
            FileTypeFilter = [
                new FilePickerFileType("Images")
            {
                Patterns = ["*.jpg", "*.jpeg", "*.png", "*.bmp"]
            }]
        });
        if (files.Count == 0) return;

        MainWindowViewModel? viewModel = window.DataContext as MainWindowViewModel;
        if (viewModel is null) return;
        
        if(viewModel.OpenFileCommand.CanExecute(files[0]))
            viewModel.OpenFileCommand.Execute(files[0]);
    }
}
