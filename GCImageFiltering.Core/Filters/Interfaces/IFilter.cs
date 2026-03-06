using GCImageFiltering.Core.Buffers;

namespace GCImageFiltering.Core.Filters.Interfaces;

public interface IFilter
{
    PixelBuffer Apply(PixelBuffer buffer);
}