using GCImageFiltering.Core.Buffers;
using GCImageFiltering.Core.Filters.Interfaces;

namespace GCImageFiltering.Core.Filters.Function;

public class FunctionalFilter : IFilter
{
    private byte[] FunctionTable { get; }
    public FunctionalFilter(byte[] functionTable)
    {
        if (functionTable.Length != 256)
            throw new ArgumentException("Function table must contain exactly 256 elements.");
        
        FunctionTable = functionTable;
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
}