using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using CGImageFiltering.App.Views.Converters;

namespace CGImageFiltering.App.Models.Editor;

public class FilterPoint : INotifyPropertyChanged
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
            OnPropertyChanged();
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
            OnPropertyChanged();
        } 
    }

    public int ScreenX => (int)CanvasPositionConverter.ToScreen(new Point(X, Y)).X;
    public int ScreenY => (int)CanvasPositionConverter.ToScreen(new Point(X, Y)).Y;
    
    public int EllipseX => ScreenX - Radius;
    public int EllipseY => ScreenY - Radius;
    
    public FilterPoint(int x, int y)
    {
        X = x;
        Y = y;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}