namespace GCImageFiltering.Core.Buffers;

public class RgbaPixel
{
    public const int BytesPerPixel = 4;
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; }
    public RgbaPixel(byte r, byte g, byte b, byte a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }
}