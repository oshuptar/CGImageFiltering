using System.Linq;
using Avalonia;
using Avalonia.Platform;
using CGImageFiltering.App.Buffers;
using CGImageFiltering.App.ViewModels.Abstractions;
using GCImageFiltering.Core.Buffers;
using GCImageFiltering.Core.Buffers.Enums;
using GCImageFiltering.Core.ColorPicker;
using Color = GCImageFiltering.Core.Buffers.Color;

namespace CGImageFiltering.App.ViewModels;

public class ColorPickerViewModel : ViewModelBase
{
    public int Width { get;set; } = _pixelsPerColor*100;
    public int Height { get; set; } = _pixelsPerColor*100;
    private const int _pixelsPerColor = 3;
    public DirectBitmap? Bitmap
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public Color CurrentColor
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Hue));
            OnPropertyChanged(nameof(Saturation));
            OnPropertyChanged(nameof(Value));
        }
    } = new Color(0, 0, 0);

    public int Hue => CurrentColor.Hue;
    public int Saturation => CurrentColor.Saturation;
    public int Value => CurrentColor.Value;
    
    public ColorPickerViewModel()
    {
        Bitmap = new DirectBitmap(Width,
            Height,
            new Vector(96, 96),
            PixelFormat.Rgba8888,
            new byte[Width * Height * 4]);
        UpdateBitmap(CurrentColor);
    }

    public void SetNewColor(int? value = null, int? saturation = null, int? hue = null)
    {
        Color color = new Color(
            hue ?? CurrentColor.Hue,
            saturation.HasValue ? saturation.Value / _pixelsPerColor : CurrentColor.Saturation,
            value.HasValue ? value.Value / _pixelsPerColor : CurrentColor.Value);
        if (hue.HasValue && hue.Value != CurrentColor.Hue)
        {
            UpdateBitmap(color);
        }
        CurrentColor = color;
    }
    
    public void UpdateBitmap(Color newColor)
    {
        var image = Bitmap;
        var width = image!.Width;
        var height = image!.Height;
        var sourceBytes = image!.Pixels.ToArray();
        var pixelBuffer = new PixelBuffer(
                width,
                height, 
                sourceBytes,
                image.Stride, 
                ColorFormat.Rgba
            );  
        ColorPickerBitmapCalculator bitmapCalculator = new ColorPickerBitmapCalculator();
        var filteredBuffer = bitmapCalculator.Calculate(pixelBuffer, newColor.Hue);
        Bitmap!.Pixels = filteredBuffer.Pixels;
        Bitmap.UpdateBitmap();
        InvalidateImage();
    }
    
    private void InvalidateImage()
    {
        DirectBitmap? current = Bitmap;
        Bitmap = null;
        Bitmap = current;
    }
}