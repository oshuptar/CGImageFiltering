using System.Windows.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CGImageFiltering.App.ViewModels.Commands.Menu;

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
    public ICommand OpenFileCommand => new OpenFileCommand(OpenFileDialog, _ => true);
    public ICommand SaveFileCommand => new SaveFileCommand();
    public ICommand ResetFileCommand => new ResetFileCommand();

    private async void OpenFileDialog(object? parameter)
    {
        if (parameter is not IStorageFile file) return;
        await using var stream = await file.OpenReadAsync();
        OriginalImage = new Bitmap(stream);
        Image = OriginalImage;
        ImageWidth = OriginalImage.PixelSize.Width;
        ImageHeight = OriginalImage.PixelSize.Height;
    }
}
