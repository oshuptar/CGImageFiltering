using GCImageFiltering.Core.Buffers;

namespace CGImageFiltering.App.Converters.Interfaces;

public interface IByteConverter
{
    public RgbaPixel[] ConvertToPixel(byte[] pixels, int width, int height);
    public byte[] ConvertToByte(RgbaPixel[] rgbaPixels, int width, int height);
}