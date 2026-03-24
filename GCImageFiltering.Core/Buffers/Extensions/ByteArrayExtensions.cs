
namespace GCImageFiltering.Core.Buffers.Extensions;

public static class ByteArrayExtensions
{
    public static byte[] DeepCopy(this byte[] pixels)
    {
        byte[] pixelArray = new byte[pixels.Length];
        for(int i = 0; i < pixels.Length; i++)
            pixelArray[i] = pixels[i];
        return pixelArray;
    }
}