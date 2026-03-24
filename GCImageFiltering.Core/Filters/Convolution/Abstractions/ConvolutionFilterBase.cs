using GCImageFiltering.Core.Buffers;
using GCImageFiltering.Core.Buffers.Enums;
using GCImageFiltering.Core.Buffers.Extensions;
using GCImageFiltering.Core.Filters.Interfaces;

namespace GCImageFiltering.Core.Filters.Convolution.Abstractions;

public abstract class ConvolutionFilterBase : IFilter
{
    public int KernelSize { get; set; }
    
    public ConvolutionFilterBase(int kernelSize)
    {
        KernelSize = kernelSize;
    }

    public virtual PixelBuffer Apply(PixelBuffer readOnlyBuffer)
    {
        int channels = readOnlyBuffer.ColorFormat == ColorFormat.Rgba ? readOnlyBuffer.BytesPerPixel - 1 : readOnlyBuffer.BytesPerPixel;
        (double[] kernelMatrix, double divisor) = GetKernelMatrix();
        PixelBuffer outputBuffer = new PixelBuffer(readOnlyBuffer.Width,
            readOnlyBuffer.Height,
            readOnlyBuffer.Pixels.DeepCopy(),
            readOnlyBuffer.Stride,
            readOnlyBuffer.ColorFormat);
        
        for (int y = 0; y < readOnlyBuffer.Height; y++)
        {
            for (int x = 0; x < readOnlyBuffer.Width; x++)
            {
                int midpoint = KernelSize / 2;
                int i = y * readOnlyBuffer.Stride + x * readOnlyBuffer.BytesPerPixel;
                for (int k = 0; k < channels; k++)
                {
                    byte output = MultiplyConvolution(readOnlyBuffer, kernelMatrix, midpoint, x, y, channel: k, divisor);
                    outputBuffer.Pixels[i + k] = output;
                }
            }
        }
        
        return outputBuffer;
    }
    
    // matrix should be filled by row
    public virtual byte MultiplyConvolution(PixelBuffer buffer, double[] matrix, int midpoint, int x, int y, int channel, double divisor = 1, int shift = 0)
    {
        if(matrix.Length != KernelSize * KernelSize)
            throw new ArgumentException("Matrix size does not match kernel size");
        
        int centeredY = y - midpoint;
        double sum = 0;
        for (int i = 0; i < KernelSize; i++, centeredY++)
        {
            int centeredX = x - midpoint;
            for (int j = 0; j < KernelSize; j++, centeredX++)
            {
                int clampedX = Math.Min(Math.Max(centeredX, 0), buffer.Width - 1);
                int clampedY = Math.Min(Math.Max(centeredY, 0), buffer.Height - 1);

                int matrixIndex = i * KernelSize + j;
                int pixelIndex = clampedY * buffer.Stride + clampedX * buffer.BytesPerPixel;
                sum += matrix[matrixIndex] * buffer.Pixels[pixelIndex + channel];
            }
        }

        return (byte)Math.Clamp(Math.Round(sum / divisor) + shift, 0, 255);
    }
    
    public abstract (double[] kernel, double divisor) GetKernelMatrix();
}