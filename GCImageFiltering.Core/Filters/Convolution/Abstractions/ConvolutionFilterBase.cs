using GCImageFiltering.Core.Buffers;
using GCImageFiltering.Core.Filters.Interfaces;

namespace GCImageFiltering.Core.Filters.Convolution.Abstractions;

public abstract class ConvolutionFilterBase : IFilter
{
    public int KernelSize { get; set; }
    
    public ConvolutionFilterBase(int kernelSize)
    {
        KernelSize = kernelSize;
    }

    public abstract PixelBuffer Apply(PixelBuffer buffer);
    
    // matrix should be filled by row
    public virtual RgbaPixel MultiplyConvolution(PixelBuffer pixelBuffer, double[] matrix, int midpoint, int x, int y, double divisor = 1, int shift = 0)
    {
        if(matrix.Length != KernelSize * KernelSize)
            throw new ArgumentException("Matrix size does not match kernel size");
        
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
            (byte)Math.Clamp(Math.Round(sumR/divisor) + shift, 0, 255),
            (byte)Math.Clamp(Math.Round(sumG/divisor) + shift, 0, 255),
            (byte)Math.Clamp(Math.Round(sumB/divisor) + shift, 0, 255),
            pixelBuffer.Pixels[y * pixelBuffer.Width + x].A);
    }
    
    public abstract (double[] kernel, double divisor) GetKernelMatrix();
}