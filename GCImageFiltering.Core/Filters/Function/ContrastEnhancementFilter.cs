using System.Drawing;
using GCImageFiltering.Core.Buffers;
using GCImageFiltering.Core.Filters.Interfaces;

namespace GCImageFiltering.Core.Filters.Function;

public class ContrastEnhancementFilter : IFilter, IGraphRepresentable
{
    public double Contrast { get; set; }
    public int Threshold { get; set; } 

    public ContrastEnhancementFilter(double contrast = 1.2, int threshold = 128)
    {
        Contrast = contrast;
    }
    
    // linear contrast adjustment
    public PixelBuffer Apply(PixelBuffer buffer)
    {
        for (int y = 0; y < buffer.Height; y++)
        {
            for (int x = 0; x < buffer.Width; x++)
            {
                int i = y * buffer.Width + x;
                buffer.Pixels[i].R = LinearContrastAdjustment(buffer.Pixels[i].R, Contrast, Threshold);
                buffer.Pixels[i].G = LinearContrastAdjustment(buffer.Pixels[i].G, Contrast, Threshold);
                buffer.Pixels[i].B = LinearContrastAdjustment(buffer.Pixels[i].B, Contrast, Threshold);
            }
        }
        return buffer;
    }

    private byte LinearContrastAdjustment(byte value, double contrastFactor ,int threshold) => 
        (byte)Math.Clamp((value - threshold) * contrastFactor + threshold, 0, 255);

    public IEnumerable<Point> BuildGraphPoints()
    {
        List<Point> list = new List<Point>();
        list.Add(new Point(0, 0));
        list.Add(new Point((int)((Threshold*Contrast - Threshold)/Contrast), 0));
        list.Add(new Point((int)((255 - Threshold + Threshold*Contrast)/Contrast), 255));
        list.Add(new Point(255, 255));
        list = list.OrderBy(point => point.X).ToList();
        return list;
    }
}