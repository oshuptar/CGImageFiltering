using GCImageFiltering.Core.Buffers;
using GCImageFiltering.Core.Filters.Interfaces;

namespace GCImageFiltering.Core.Filters.Function;

public class ContrastEnhancementFilter : IFilter
{
    public double Contrast { get; set; }

    public ContrastEnhancementFilter(double contrast = 1.2)
    {
        Contrast = contrast;
    }
    
    // linear contrast adjustment
    public PixelBuffer Apply(PixelBuffer buffer)
    {
        int midpoint = 128;
        for (int y = 0; y < buffer.Height; y++)
        {
            for (int x = 0; x < buffer.Width; x++)
            {
                int i = y * buffer.Width + x;
                buffer.Pixels[i].R = LinearContrastAdjustment(buffer.Pixels[i].R, Contrast, midpoint);
                buffer.Pixels[i].G = LinearContrastAdjustment(buffer.Pixels[i].G, Contrast, midpoint);
                buffer.Pixels[i].B = LinearContrastAdjustment(buffer.Pixels[i].B, Contrast, midpoint);
            }
        }
        return buffer;
    }

    private byte LinearContrastAdjustment(byte value, double contrastFactor ,int threshold) => 
        (byte)Math.Clamp((value - threshold) * contrastFactor + threshold, 0, 255);
    
}