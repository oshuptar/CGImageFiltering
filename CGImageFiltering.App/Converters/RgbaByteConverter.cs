using CGImageFiltering.App.Converters.Interfaces;
using GCImageFiltering.Core.Buffers;

namespace CGImageFiltering.App.Converters;

public class RgbaByteConverter : IByteConverter
{
    public RgbaPixel[] ConvertToPixel(byte[] pixels, int width, int height)
    {
        int stride = width * RgbaPixel.BytesPerPixel;
        RgbaPixel[] rgbaPixels = new RgbaPixel[height * width];
        for (int y = 0; y < height; y++)
        {
            int rowStart = y * stride;
            for (int x = 0; x < width; x++)
            {
                int i = rowStart + x * RgbaPixel.BytesPerPixel;
                int j = y * width + x;
                rgbaPixels[j] = new RgbaPixel(pixels[i], pixels[i + 1], pixels[i + 2], pixels[i + 3]);
            }
        }
        return rgbaPixels;
    }

    public byte[] ConvertToByte(RgbaPixel[] rgbaPixels, int width, int height)
    {
        int stride = width * RgbaPixel.BytesPerPixel;
        byte[] pixels = new byte[stride * height];
        for (int y = 0; y < height; y++)
        {
            int rowStart = y * stride;
            for (int x = 0; x < width; x++)
            {
                int i = rowStart + x * RgbaPixel.BytesPerPixel;
                int j = y * width + x;
                pixels[i] = rgbaPixels[j].R;
                pixels[i + 1] = rgbaPixels[j].G;
                pixels[i + 2] = rgbaPixels[j].B;
                pixels[i + 3] = rgbaPixels[j].A;
            }
        }
        return pixels;
    }
}