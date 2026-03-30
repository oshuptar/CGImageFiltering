using GCImageFiltering.Core.Buffers;
using GCImageFiltering.Core.Quantization.Pixel;

namespace GCImageFiltering.Core.ColorPicker;

public class ColorPickerBitmapCalculator
{
    public PixelBuffer Calculate(PixelBuffer buffer, int hue)
    {
        for (int y = 0; y < buffer.Height; y++)
        {
            for (int x = 0; x < buffer.Width; x++)
            {
                int i = y * buffer.Stride + x * buffer.BytesPerPixel;
                Color color = new Color(hue, value: y, saturation: x);
                RgbaPixel pixel = ColorConverter.HsvToRgb(color, buffer.Width, buffer.Height);
                buffer.Pixels[i] = pixel.R;
                buffer.Pixels[i + 1] = pixel.G;
                buffer.Pixels[i + 2] = pixel.B;
                buffer.Pixels[i + 3] = pixel.A;
            }
        }
        return buffer;
    }
}