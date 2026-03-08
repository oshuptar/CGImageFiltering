namespace CGImageFiltering.App.Models.Editor;

public class FilterPoint
{
    public int X { get; set; }
    public int Y { get; set; }
    
    public FilterPoint(int x, int y)
    {
        X = x;
        Y = y;
    }
}