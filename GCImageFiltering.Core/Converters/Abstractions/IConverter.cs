using GCImageFiltering.Core.Buffers;

namespace GCImageFiltering.Core.Converters.Abstractions;

public interface IConverter
{
    PixelBuffer Convert(PixelBuffer buffer);
}