using System.Drawing;
using GCImageFiltering.Core.Buffers;
using GCImageFiltering.Core.Buffers.Enums;
using GCImageFiltering.Core.Filters.Interfaces;

namespace GCImageFiltering.Core.Filters.Function;

public class BrightnessFilter : IFilter, IGraphRepresentable
{
    public int Delta { get; set; } 
    
    public BrightnessFilter(int delta = 20)
    {
        Delta = delta;
    }
    
    public PixelBuffer Apply(PixelBuffer buffer)
    {
        int channels = buffer.ColorFormat == ColorFormat.Rgba ? buffer.BytesPerPixel - 1 : buffer.BytesPerPixel;
        for (int y = 0; y < buffer.Height; y++)
        {
            for (int x = 0; x < buffer.Width; x++)
            {
                int i = y * buffer.Stride + x * buffer.BytesPerPixel;
                for (int k = 0; k < channels; k++)
                    buffer.Pixels[i + k] = ClampToByte(buffer.Pixels[i + k] + Delta);
            }
        }
        return buffer;
    }
    private static byte ClampToByte(int value) => (byte)Math.Clamp(value, 0, 255);
    public IEnumerable<Point> BuildGraphPoints()
    {
        var list = new List<Point>();
        list.Add(new Point(0, Delta));
        list.Add(new Point(255 - Delta, 255));
        list.Add(new Point(255, 255));
        list = list.OrderBy(point => point.X).ToList();
        return list;
    }
}