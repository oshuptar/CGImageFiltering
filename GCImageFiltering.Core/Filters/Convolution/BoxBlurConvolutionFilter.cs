using GCImageFiltering.Core.Buffers;
using GCImageFiltering.Core.Buffers.Extensions;
using GCImageFiltering.Core.Filters.Convolution.Abstractions;

namespace GCImageFiltering.Core.Filters.Convolution;

public class BoxBlurConvolutionFilter : ConvolutionFilterBase
{
    public BoxBlurConvolutionFilter(int kernelSize = 3) : base(kernelSize)
    {
        
    }

    public override PixelBuffer Apply(PixelBuffer readOnlyBuffer)
    {
        int divisor = KernelSize * KernelSize;
        PixelBuffer outputBuffer = new PixelBuffer(readOnlyBuffer.Width, readOnlyBuffer.Height, readOnlyBuffer.Pixels.DeepCopy());
        for (int y = 0; y < readOnlyBuffer.Height; y++)
        {
            for (int x = 0; x < readOnlyBuffer.Width; x++)
            {
                int midpoint = KernelSize / 2;
                int i = y * readOnlyBuffer.Width + x;

                double[] kernelMatrix = GetKernelMatrix(KernelSize);
                RgbaPixel output = MultiplyConvolution(readOnlyBuffer, kernelMatrix, midpoint, x, y, divisor);
                outputBuffer.Pixels[i] = output;
            }
        }
        
        return outputBuffer;
    }

    // matrix should be filled by row
    private RgbaPixel MultiplyConvolution(PixelBuffer pixelBuffer, double[] matrix, int midpoint, int x, int y, double divisor)
    {
        int centeredY = y - midpoint;
        double sumR = 0; double sumG = 0; double sumB = 0;
        for (int i = 0; i < KernelSize; i++, centeredY++)
        {
            int centeredX = x - midpoint;
            for (int j = 0; j < KernelSize; j++, centeredX++)
            {
                int clampedX = Math.Min(Math.Max(centeredX, 0), pixelBuffer.Width - 1);
                int clampedY = Math.Min(Math.Max(centeredY, 0), pixelBuffer.Height - 1);

                int matrixIndex = i * KernelSize + j;
                int pixelIndex = clampedY * pixelBuffer.Width + clampedX;
                sumR += matrix[matrixIndex] * pixelBuffer.Pixels[pixelIndex].R;
                sumG += matrix[matrixIndex] * pixelBuffer.Pixels[pixelIndex].G;
                sumB += matrix[matrixIndex] * pixelBuffer.Pixels[pixelIndex].B;
            }
        }
        return new RgbaPixel(
            (byte)Math.Clamp(Math.Round(sumR/divisor), 0, 255),
            (byte)Math.Clamp(Math.Round(sumG/divisor), 0, 255),
            (byte)Math.Clamp(Math.Round(sumB / divisor), 0, 255),
            pixelBuffer.Pixels[y * pixelBuffer.Width + x].A);
    }

    private double[] GetKernelMatrix(int kernelSize)
    {
        double[] kernel = new double[kernelSize * kernelSize];
        Array.Fill(kernel, 1.0);
        return kernel;
    }
}