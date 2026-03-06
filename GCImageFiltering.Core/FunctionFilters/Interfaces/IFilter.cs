using GCImageFiltering.Core.Buffers;

namespace GCImageFiltering.Core.FunctionFilters.Interfaces;

public interface IFilter
{
    PixelBuffer Apply(PixelBuffer buffer);
}