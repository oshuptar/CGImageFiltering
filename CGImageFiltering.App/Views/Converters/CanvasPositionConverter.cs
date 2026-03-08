using Avalonia;

namespace CGImageFiltering.App.Views.Converters;

public static class CanvasPositionConverter
{
    public static Point ToScreen(Point logicalCoordinates) => new(logicalCoordinates.X, 255 - logicalCoordinates.Y);
    public static Point ToLogical(Point screenCoordinates) => new(screenCoordinates.X, 255 - screenCoordinates.Y);
}