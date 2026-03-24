using GCImageFiltering.Core.Filters.Convolution.Abstractions;

namespace GCImageFiltering.Core.Filters.Convolution;

public class BoxBlurConvolutionFilter : ConvolutionFilterBase
{
    public BoxBlurConvolutionFilter(int kernelSize = 3) : base(kernelSize)
    {
        
    }
    
    public override (double[] kernel, double divisor) GetKernelMatrix()
    {
        double[] kernel = new double[KernelSize * KernelSize];
        Array.Fill(kernel, 1.0);
        double divisor = kernel.ToList().Sum();
        return (kernel, divisor);
    }
}