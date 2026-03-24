using GCImageFiltering.Core.Filters.Convolution.Abstractions;

namespace GCImageFiltering.Core.Filters.Convolution;

public class EastEmbossConvolutionFilter : ConvolutionFilterBase
{
    public EastEmbossConvolutionFilter(int kernelSize = 3) : base(kernelSize)
    {
    }

    public override (double[] kernel, double divisor) GetKernelMatrix()
    {
        double divisor = 1.0;
        double[] kernel = [-1, 0, 1, -1, 1, 1, -1, 0, 1];
        return (kernel, divisor);
    }
}