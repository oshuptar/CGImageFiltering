using System.Drawing;
using GCImageFiltering.Core.Buffers;
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
        for (int y = 0; y < buffer.Height; y++)
        {
            for (int x = 0; x < buffer.Width; x++)
            {
                int i = y * buffer.Width + x;
                buffer.Pixels[i].R = ClampToByte(buffer.Pixels[i].R + Delta);
                buffer.Pixels[i].G = ClampToByte(buffer.Pixels[i].G + Delta);
                buffer.Pixels[i].B = ClampToByte(buffer.Pixels[i].B + Delta);
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