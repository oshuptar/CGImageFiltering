namespace GCImageFiltering.Core.Buffers;

public class PixelBuffer
{
    public int Width { get; }
    public int Height { get; }
    public int Stride { get; }
    public byte[] Pixels { get; }
    public int BytesPerPixel => Stride/Width; 

    public PixelBuffer(int width, int height, int stride, byte[] pixels)
    {
        Width = width;
        Height = height;
        Stride = stride;
        Pixels = pixels;
    }
}