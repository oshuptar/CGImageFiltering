using System.Windows.Input;
using Avalonia.Media.Imaging;
using CGImageFiltering.App.ViewModels.Commands.Menu;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CGImageFiltering.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private Bitmap? _image;
    public Bitmap? Image
    {
        get => _image;
        set
        {
            _image = value;
            OnPropertyChanged();
        }
    }
    public int ImageHeight { get; set; }
    public int ImageWidth { get; set; }
    public Bitmap? OriginalImage { get; set; }
    public Bitmap? FilteredImage { get; set; }
    
    public ICommand OpenFileCommand => new OpenFileCommand();
    public ICommand SaveFileCommand => new SaveFileCommand();
    public ICommand ResetFileCommand => new ResetFileCommand();
    public MainWindowViewModel()
    {
        
    }
}
