using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CGImageFiltering.App.Buffers;
using GCImageFiltering.Core.Buffers;
using GCImageFiltering.Core.FunctionFilters;
using GCImageFiltering.Core.FunctionFilters.Interfaces;

namespace CGImageFiltering.App.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private DirectBitmap? _image;
    public DirectBitmap? Image
    {
        get => _image;
        set
        {
            _image = value;
            OnPropertyChanged();
        }
    }
    private DirectBitmap? OriginalImage { get; set; }
    private DirectBitmap? FilteredImage { get; set; }
    private bool IsOriginalDisplayed { get; set; } = false;
    public Commands.RelayCommand OpenFileCommand { get; }
    public Commands.RelayCommand SaveFileCommand { get; }
    public Commands.RelayCommand ToggleImagePreviewCommand { get; }
    public Commands.RelayCommand ApplyBrigthnessFilterCommand { get; } 
    public Commands.RelayCommand ApplyInversionFilterCommand { get; }
    public MainWindowViewModel()
    {
        OpenFileCommand = new Commands.RelayCommand(OpenFileDialog, _ => true);
        SaveFileCommand = new Commands.RelayCommand(SaveFile, _ => Image is not null);
        ToggleImagePreviewCommand =  new Commands.RelayCommand(ToggleImagePreview, _ => Image is not null);
        ApplyBrigthnessFilterCommand = new Commands.RelayCommand(ApplyBrightnessFilter, CanApplyFilter);
        ApplyInversionFilterCommand = new Commands.RelayCommand(ApplyInversionFilter, CanApplyFilter);
    }

    private async void OpenFileDialog(object? parameter)
    {
        if (parameter is not IStorageFile file) return;
        await using var stream = await file.OpenReadAsync();
        Bitmap bitmap = new Bitmap(stream);
        OriginalImage = DirectBitmap.ToRgba(bitmap);
        FilteredImage = DirectBitmap.ToRgba(bitmap);
        Image = FilteredImage;
        RefreshCommands();
    }

    private void SaveFile(object? parameter)
    {
        
    }
    
    private void ToggleImagePreview(object? parameter)
    {
        if(IsOriginalDisplayed)
            Image = FilteredImage;
        else 
            Image = OriginalImage;
        IsOriginalDisplayed = !IsOriginalDisplayed;
        RefreshCommands();
    }

    private void ApplyFilter(IFilter filter)
    {
        if (Image is null) return;
        
        filter.Apply(new PixelBuffer(Image.Width, Image.Height, Image.Stride, Image.Pixels));
        Image.UpdateBitmap();
        InvalidateImage();
    }

    private bool CanApplyFilter(object? parameter)
    {
        return Image is not null && !IsOriginalDisplayed;
    }

    private void ApplyBrightnessFilter(object? parameter)
    {
        IFilter brightnessFilter = new BrightnessFilter(40);
        ApplyFilter(brightnessFilter);
    }
    
    private void ApplyInversionFilter(object? parameter)
    {
        IFilter inversionFilter = new InversionFilter();
        ApplyFilter(inversionFilter);
    }

    private void InvalidateImage()
    {
        // Force to make sure UI redraws updated writeable bitmap. TODO: fix. think about different solution
        DirectBitmap? current = Image;
        Image = null;
        Image = current;
        RefreshCommands();
    }

    private void RefreshCommands()
    {
        SaveFileCommand.RaiseCanExecuteChanged();
        ToggleImagePreviewCommand.RaiseCanExecuteChanged();
        ApplyBrigthnessFilterCommand.RaiseCanExecuteChanged();
        ApplyInversionFilterCommand.RaiseCanExecuteChanged();
    }
}
