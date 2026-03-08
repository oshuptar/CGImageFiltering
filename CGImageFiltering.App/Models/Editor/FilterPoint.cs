using System;

namespace CGImageFiltering.App.Models.Editor;

public class FilterPoint
{
    private const int Radius = 3;
    public int X
    {
        get => field;
        set
        {
            if (value < 0 || value > 255)
                throw new ArgumentException("Value must be between 0 and 255", nameof(value));
            field = value;
        }
    }
    public int Y 
    { 
        get => field;
        set
        {
            if (value < 0 || value > 255)
                throw new ArgumentException("Value must be between 0 and 255", nameof(value));
            field = value;
        } 
    }

    public int ScreenX => X;
    public int ScreenY => 255 - Y;
    
    public int EllipseX => ScreenX - Radius;
    public int EllipseY => ScreenY - Radius;
    
    public FilterPoint(int x, int y)
    {
        X = x;
        Y = y;
    }
}