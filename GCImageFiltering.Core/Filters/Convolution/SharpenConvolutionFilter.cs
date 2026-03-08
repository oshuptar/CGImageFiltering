using GCImageFiltering.Core.Buffers;
using GCImageFiltering.Core.Buffers.Extensions;
using GCImageFiltering.Core.Filters.Convolution.Abstractions;

namespace GCImageFiltering.Core.Filters.Convolution;

public class SharpenConvolutionFilter : ConvolutionFilterBase
{
    public int A { get; set; }
    public int B { get; set; }
    public SharpenConvolutionFilter(int kernelSize = 3, int a = 1, int b = 5) : base(kernelSize)
    {
        A = a;
        B = b;
    }

    public override PixelBuffer Apply(PixelBuffer readOnlyBuffer)
    {
        int midpoint = KernelSize / 2;
        PixelBuffer outputBuffer = new PixelBuffer(readOnlyBuffer.Width, readOnlyBuffer.Height, readOnlyBuffer.Pixels.DeepCopy());
        (double[] kernel, double divisor) = GetKernelMatrix();
        for (int y = 0; y < readOnlyBuffer.Height; y++)
        {
            for (int x = 0; x < readOnlyBuffer.Width; x++)
            {
                int i = y * readOnlyBuffer.Width + x;
                RgbaPixel output = MultiplyConvolution(readOnlyBuffer, kernel, midpoint, x, y, divisor);
                outputBuffer.Pixels[i] = output;
            }
        }
        return outputBuffer;
    }

    public override (double[] kernel, double divisor) GetKernelMatrix()
    {
        double s = B - 4 * A;
        double[] kernel = [0, - A, 0, -A, B, -A, 0, -A, 0];
        return (kernel, s);
    }
}