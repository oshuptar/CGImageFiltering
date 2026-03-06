using GCImageFiltering.Core.Buffers;
using GCImageFiltering.Core.FunctionFilters.Interfaces;

namespace GCImageFiltering.Core.FunctionFilters;

public class InversionFilter : IFilter
{
    public PixelBuffer Apply(PixelBuffer buffer)
    {
        for (int y = 0; y < buffer.Height; y++)
        {
            int rowStart = y * buffer.Stride;
            for (int x = 0; x < buffer.Width; x++)
            {
                int i = rowStart + x * buffer.BytesPerPixel;
                buffer.Pixels[i] = (byte)(255 - buffer.Pixels[i]); // R         
                buffer.Pixels[i + 1] = (byte)(255 - buffer.Pixels[i + 1]);//G
                buffer.Pixels[i + 2] = (byte)(255 - buffer.Pixels[i + 2]);//B
            }
        }
        
        return buffer;
    }
}