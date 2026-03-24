using GCImageFiltering.Core.Filters.Convolution.Abstractions;

namespace GCImageFiltering.Core.Filters.Convolution;

public class HorizontalEdgeDetectionConvolutionFilter : ConvolutionFilterBase
{
    public HorizontalEdgeDetectionConvolutionFilter(int kernelSize = 3) : base(kernelSize)
    {
    }
    public override (double[] kernel, double divisor) GetKernelMatrix()
    {
        double[] kernel = [0, -1, 0, 0, 1, 0, 0, 0, 0];
        double divisor = 1.0;
        return (kernel, divisor);
    }
}