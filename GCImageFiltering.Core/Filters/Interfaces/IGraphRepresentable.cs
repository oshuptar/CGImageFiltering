using System.Drawing;

namespace GCImageFiltering.Core.Filters.Interfaces;

public interface IGraphRepresentable
{
    IEnumerable<Point> BuildGraphPoints();
}