using GCImageFiltering.Core.Buffers;
using GCImageFiltering.Core.Buffers.Enums;
using GCImageFiltering.Core.Filters.Interfaces;

namespace GCImageFiltering.Core.Filters.Dithering;

public class RandomDithering: IFilter
{
    public int ColorLevels { get; private set; }
    
    public RandomDithering(int colorLevels = 4)
    {
        ColorLevels = colorLevels;
    }
    public PixelBuffer Apply(PixelBuffer buffer)
    {
        int channels = buffer.ColorFormat == ColorFormat.Rgba ? buffer.BytesPerPixel - 1 : buffer.BytesPerPixel;
        Random random = new Random();
        for (int y = 0; y < buffer.Height; y++)
        {
            for (int x = 0; x < buffer.Width; x++)
            {
                for (int k = 0; k < channels; k++)
                {
                    int index = y * buffer.Stride + x * buffer.BytesPerPixel + k;
                    buffer.Pixels[index] = Dither(buffer.Pixels[index], ColorLevels, random);
                }
            }
        }
        return buffer;
    }
    
    private byte Dither(byte pixel, int colorLevels, Random random)
    {
        int step = 256 / (colorLevels - 1);
        int leftIndex = pixel / step;
        int rightIndex = pixel / step + 1;
        int leftBound = leftIndex * step;
        int rightBound = rightIndex * step - 1;
        double threshold = (double)(pixel - leftBound)/(rightBound - leftBound);
        return random.NextDouble() < threshold ? (byte)rightBound : (byte)leftBound;
    }
}