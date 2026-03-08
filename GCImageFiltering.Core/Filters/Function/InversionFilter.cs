using System.Drawing;
using GCImageFiltering.Core.Buffers;
using GCImageFiltering.Core.Filters.Interfaces;

namespace GCImageFiltering.Core.Filters.Function;

public class InversionFilter : IFilter, IGraphRepresentable
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

    public IEnumerable<Point> BuildGraphPoints()
    {
        var list = new List<Point>();
        list.Add(new Point(0, 255));
        list.Add(new Point(255, 0));
        list = list.OrderBy(point => point.X).ToList();
        return list;
    }
}