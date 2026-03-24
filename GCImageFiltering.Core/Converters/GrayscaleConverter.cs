using GCImageFiltering.Core.Buffers;
using GCImageFiltering.Core.Buffers.Enums;
using GCImageFiltering.Core.Converters.Abstractions;

namespace GCImageFiltering.Core.Converters;

public class GrayscaleConverter : IConverter
{
    public PixelBuffer Convert(PixelBuffer buffer)
    {
        if(buffer.ColorFormat == ColorFormat.Grayscale)
            return buffer;
        
        int stride = 1;
        byte[] outputBuffer = new byte[buffer.Height * buffer.Width];
        for (int y = 0; y < buffer.Height; y++)
        {
            for (int x = 0; x < buffer.Width; x++)
            {
                int outputIndex = y * buffer.Width + x * stride;
                int inputIndex = y * buffer.Stride + x * buffer.BytesPerPixel;
                outputBuffer[outputIndex] = (byte)Math.Clamp(0.299*buffer.Pixels[inputIndex] 
                                            + 0.587*buffer.Pixels[inputIndex + 1]
                                            + 0.114*buffer.Pixels[inputIndex + 2], 0, 255);
            }
        }

        return new PixelBuffer(buffer.Width,
            buffer.Height,
            outputBuffer,
            stride,
            ColorFormat.Grayscale);
    }
}