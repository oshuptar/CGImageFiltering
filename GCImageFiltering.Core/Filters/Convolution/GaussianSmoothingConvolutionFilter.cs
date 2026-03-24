using GCImageFiltering.Core.Filters.Convolution.Abstractions;

namespace GCImageFiltering.Core.Filters.Convolution;

public class GaussianSmoothingConvolutionFilter : ConvolutionFilterBase
{
    public GaussianSmoothingConvolutionFilter(int kernelSize = 3) : base(kernelSize)
    {
    }
    

    public override (double[] kernel, double divisor) GetKernelMatrix()
    {
        // Hardcoded for now
        double[] kernel = [0, 1, 0, 1, 4, 1, 0, 1, 0];
        double divisor = kernel.ToList().Sum();
        return (kernel, divisor);
    }
}