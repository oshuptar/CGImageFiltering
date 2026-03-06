using GCImageFiltering.Core.Buffers;
using GCImageFiltering.Core.FunctionFilters.Interfaces;

namespace GCImageFiltering.Core.FunctionFilters;

public class BrightnessFilter : IFilter
{
    public int Delta { get; set; } 
    
    public BrightnessFilter(int delta)
    {
        Delta = delta;
    }
    
    public PixelBuffer Apply(PixelBuffer buffer)
    {
        for (int y = 0; y < buffer.Height; y++)
        {
            int rowStart = y * buffer.Stride;
            for (int x = 0; x < buffer.Width; x++)
            {
                int i = rowStart + x * buffer.BytesPerPixel;
                buffer.Pixels[i + 0] = ClampToByte(buffer.Pixels[i + 0] + Delta);
                buffer.Pixels[i + 1] = ClampToByte(buffer.Pixels[i + 1] + Delta);
                buffer.Pixels[i + 2] = ClampToByte(buffer.Pixels[i + 2] + Delta);
            }
        }

        return buffer;
    }
    
    private static byte ClampToByte(int value)
    {
        if (value < 0) return 0;
        if (value > 255) return 255;
        return (byte)value;
    }
}