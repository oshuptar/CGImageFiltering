namespace GCImageFiltering.Core.Buffers;

public class PixelBuffer
{
    public int Width { get; }
    public int Height { get; }
    public RgbaPixel[] Pixels { get; }

    public PixelBuffer(int width, int height, RgbaPixel[] pixels)
    {
        Width = width;
        Height = height;
        Pixels = pixels;
    }
}