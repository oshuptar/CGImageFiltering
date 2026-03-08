using System.Drawing;
using GCImageFiltering.Core.Buffers;
using GCImageFiltering.Core.Filters.Interfaces;

namespace GCImageFiltering.Core.Filters.Function;

public class FunctionalFilter : IFilter, IGraphRepresentable
{
    private byte[] FunctionTable { get; }
    private IReadOnlyList<Point> _graphPoints;
    public FunctionalFilter(IReadOnlyList<Point> graphPoints)
    { 
        _graphPoints = graphPoints;
        FunctionTable = BuildFunctionTable();
    }
    public PixelBuffer Apply(PixelBuffer buffer)
    {
        for (int y = 0; y < buffer.Height; y++)
        {
            for (int x = 0; x < buffer.Width; x++)
            {
                int i = y * buffer.Width + x;
                buffer.Pixels[i].R = FunctionTable[buffer.Pixels[i].R];
                buffer.Pixels[i].G = FunctionTable[buffer.Pixels[i].G];
                buffer.Pixels[i].B = FunctionTable[buffer.Pixels[i].B];
            }
        }
        return buffer;
    }
    
    public byte[] BuildFunctionTable()
    {
        var table = new byte[256];
        for (int segment= 0; segment < _graphPoints.Count - 1; segment++)
        {
            var left = _graphPoints[segment];
            var right = _graphPoints[segment + 1];
            int x1 = left.X;
            int y1 = left.Y;
            int x2 = right.X;
            int y2 = right.Y;
            
            for (int x = x1; x <= x2; x++)
            {
                double alpha = (double)(y2 - y1) / (x2 - x1);
                double y = y1 + alpha * (x - x1);
                table[x] = (byte)Math.Clamp((int)Math.Round(y), 0, 255);
            }
        }
        return table;
    }
    public IEnumerable<Point> BuildGraphPoints() => _graphPoints;
}