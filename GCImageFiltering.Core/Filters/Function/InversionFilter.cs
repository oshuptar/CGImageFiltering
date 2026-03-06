using GCImageFiltering.Core.Buffers;
using GCImageFiltering.Core.Filters.Interfaces;

namespace GCImageFiltering.Core.Filters.Function;

public class InversionFilter : IFilter
{
    public PixelBuffer Apply(PixelBuffer buffer)
    {
        for (int y = 0; y < buffer.Height; y++)
        {
            for (int x = 0; x < buffer.Width; x++)
            {
                int i = y * buffer.Width + x;
                buffer.Pixels[i].R = (byte)(255 - buffer.Pixels[i].R);      
                buffer.Pixels[i].G = (byte)(255 - buffer.Pixels[i].G);//G
                buffer.Pixels[i].B = (byte)(255 - buffer.Pixels[i].B);//B
            }
        }
        return buffer;
    }
}