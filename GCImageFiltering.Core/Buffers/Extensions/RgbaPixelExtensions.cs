namespace GCImageFiltering.Core.Buffers.Extensions;

public static class RgbaPixelExtensions
{
    public static RgbaPixel[] DeepCopy(this RgbaPixel[] pixels)
    {
        RgbaPixel[] pixelArray = new RgbaPixel[pixels.Length];
        for(int i = 0; i < pixels.Length; i++)
            pixelArray[i] = new RgbaPixel(pixels[i].R, pixels[i].G, pixels[i].B, pixels[i].A);
        return pixelArray;
    }
}