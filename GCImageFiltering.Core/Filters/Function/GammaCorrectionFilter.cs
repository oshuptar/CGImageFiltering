using GCImageFiltering.Core.Buffers;
using GCImageFiltering.Core.Buffers.Enums;
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
        int channels = buffer.ColorFormat == ColorFormat.Rgba ? buffer.BytesPerPixel - 1 : buffer.BytesPerPixel;
        for (int y = 0; y < buffer.Height; y++)
        {
            for (int x = 0; x < buffer.Width; x++)
            {
                int i = y * buffer.Stride + x * buffer.BytesPerPixel;
                for (int k = 0; k < channels; k++)
                    buffer.Pixels[i + k] = GammaCorrection(buffer.Pixels[i + k], Gamma);
            }
        }
        return buffer;
    }

    private byte GammaCorrection(byte pixel, double gamma) =>
        (byte)(Math.Pow(pixel / 255.0, gamma) * 255);
}