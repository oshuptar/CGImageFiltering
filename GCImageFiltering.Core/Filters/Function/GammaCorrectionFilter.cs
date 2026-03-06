using GCImageFiltering.Core.Buffers;
using GCImageFiltering.Core.Filters.Interfaces;

namespace GCImageFiltering.Core.Filters.Function;

public class GammaCorrectionFilter : IFilter
{
    public double Gamma { get; set; }
    public GammaCorrectionFilter(double gamma = 2.2)
    {
        Gamma = gamma;
    }
    public PixelBuffer Apply(PixelBuffer buffer)
    {
        for (int y = 0; y < buffer.Height; y++)
        {
            for (int x = 0; x < buffer.Width; x++)
            {
                int i = y * buffer.Width + x;
                buffer.Pixels[i].R = GammaCorrection(buffer.Pixels[i].R, Gamma);
                buffer.Pixels[i].G = GammaCorrection(buffer.Pixels[i].G, Gamma);
                buffer.Pixels[i].B = GammaCorrection(buffer.Pixels[i].B, Gamma);
            }
        }
        return buffer;
    }

    private byte GammaCorrection(byte pixel, double gamma) =>
        (byte)(Math.Pow(pixel / 255.0, gamma) * 255);
}