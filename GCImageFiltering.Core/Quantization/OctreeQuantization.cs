using GCImageFiltering.Core.Buffers;
using GCImageFiltering.Core.Buffers.Enums;
using GCImageFiltering.Core.Filters.Interfaces;
using GCImageFiltering.Core.Quantization.DataStructure;
using GCImageFiltering.Core.Quantization.Pixel;

namespace GCImageFiltering.Core.Quantization;

public class OctreeQuantization : IFilter
{
    public int MaxColors { get; set; }
    private Octree? Octree { get; set; } = null;
    
    public OctreeQuantization(int maxColors = 256)
    {
        MaxColors = maxColors;
    }
    
    public PixelBuffer Apply(PixelBuffer buffer)
    {
        if (buffer.ColorFormat != ColorFormat.Rgba) return buffer;
        int channels = buffer.BytesPerPixel - 1;
        Octree = new Octree(MaxColors);
        for (int y = 0; y < buffer.Height; y++)
        {
            for (int x = 0; x < buffer.Width; x++)
            {
                int index = y * buffer.Stride + x * buffer.BytesPerPixel;
                RgbaPixel pixel = new RgbaPixel(buffer.Pixels[index],
                    buffer.Pixels[index + 1],
                    buffer.Pixels[index + 2],
                    buffer.Pixels[index + 3]);
                Octree.Insert(pixel);
            }
        }
        
        for (int y = 0; y < buffer.Height; y++)
        {
            for (int x = 0; x < buffer.Width; x++)
            {
                int index = y * buffer.Stride + x * buffer.BytesPerPixel;
                RgbaPixel pixel = new RgbaPixel(buffer.Pixels[index],
                    buffer.Pixels[index + 1],
                    buffer.Pixels[index + 2],
                    buffer.Pixels[index + 3]);
                RgbaPixel resultingPixel = Octree.FindColor(pixel);
                buffer.Pixels[index] = resultingPixel.R;
                buffer.Pixels[index + 1] = resultingPixel.G;
                buffer.Pixels[index + 2] = resultingPixel.B;
                buffer.Pixels[index + 3] = resultingPixel.A;
            }
        }
        return buffer;
    }
}