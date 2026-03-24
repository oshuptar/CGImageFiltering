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

    public override (double[] kernel, double divisor) GetKernelMatrix()
    {
        double s = B - 4 * A;
        double[] kernel = [0, - A, 0, -A, B, -A, 0, -A, 0];
        return (kernel, s);
    }
}