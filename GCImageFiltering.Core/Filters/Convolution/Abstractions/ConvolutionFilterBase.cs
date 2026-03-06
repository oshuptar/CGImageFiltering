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
}