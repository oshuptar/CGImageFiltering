using GCImageFiltering.Core.Quantization.Pixel;

namespace GCImageFiltering.Core.Buffers;

public static class ColorConverter
{
    public static RgbaPixel HsvToRgb(Color color, int width, int height)
    {
        int hue = color.Hue;
        double saturation = (double)color.Saturation / (width - 1);
        double value = (double)color.Value / (height - 1);
        double chroma = value * saturation;
        double x = chroma * (1 - Math.Abs(((hue / 60.0) % 2) - 1));
        double m = value - chroma;
        double r_; double g_; double b_;
        if (hue >= 0 && hue < 60)
        {
            r_ = chroma;
            g_ = x;
            b_ = 0;
        }
        else if (hue >= 60 && hue < 120)
        {
            r_ = x;
            g_ = chroma;
            b_ = 0;
        }
        else if (hue >= 120 && hue < 180)
        {
            r_ = 0;
            g_ = chroma;
            b_ = x;
        }
        else if (hue >= 180 && hue < 240)
        {
            r_ = 0;
            g_ = x;
            b_ = chroma;
        }
        else if (hue >= 240 && hue < 300)
        {
            r_ = x;
            g_ = 0;
            b_ = chroma;
        }
        else
        {
            r_ = chroma;
            g_ = 0;
            b_ = x;
        }
        return new RgbaPixel(
            (byte)((r_ + m) * 255),
            (byte)((g_ + m) * 255),
            (byte)((b_ + m) * 255),
            255);
    }
}