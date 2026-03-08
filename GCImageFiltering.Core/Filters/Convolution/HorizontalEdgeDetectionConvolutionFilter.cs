using GCImageFiltering.Core.Buffers;
using GCImageFiltering.Core.Buffers.Extensions;
using GCImageFiltering.Core.Filters.Convolution.Abstractions;

namespace GCImageFiltering.Core.Filters.Convolution;

public class HorizontalEdgeDetectionConvolutionFilter : ConvolutionFilterBase
{
    public HorizontalEdgeDetectionConvolutionFilter(int kernelSize = 3) : base(kernelSize)
    {
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
                RgbaPixel output = MultiplyConvolution(readOnlyBuffer, kernel, midpoint, x, y, divisor, 128);
                outputBuffer.Pixels[i] = output;
            }
        }
        return outputBuffer;
    }

    public override (double[] kernel, double divisor) GetKernelMatrix()
    {
        double[] kernel = [0, -1, 0, 0, 1, 0, 0, 0, 0];
        double divisor = 1.0;
        return (kernel, divisor);
    }
}