using GCImageFiltering.Core.Quantization.Pixel;

namespace GCImageFiltering.Core.Buffers;

public class Color
{
    public int Hue { get; set; }
    public int Saturation { get; set; }
    public int Value { get; set; }

    public Color(int hue, int saturation, int value)
    {
        Hue = hue;
        Saturation = saturation;
        Value = value;
    }
}