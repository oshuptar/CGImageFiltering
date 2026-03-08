using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CGImageFiltering.App.Models.Editor;
using CGImageFiltering.App.Models.Editor.Enums;
using CGImageFiltering.App.ViewModels;
using CGImageFiltering.App.Views.Converters;


namespace CGImageFiltering.App.Views.Controls;

public partial class EditorArea : UserControl
{
    private FilterPoint? _draggedPoint;
    private bool _isDragging;
    
    public EditorArea()
    {
        InitializeComponent();
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if(DataContext is not EditorViewModel viewModel || sender is not Control canvas)
            return;
        
        var screenPosition = e.GetPosition(canvas);
        var logicalPosition = CanvasPositionConverter.ToLogical(screenPosition);
        FilterPoint? point = viewModel.TryFindPointAt(logicalPosition, FilterPoint.Radius);
        switch (viewModel.CurrentMode)
        {
            case EditorMode.Add:
                viewModel.TryAddPoint(logicalPosition);
                break;
            case EditorMode.Remove:
            {
                if (point is not null)
                    viewModel.TryRemovePoint(point);
                break;
            }

            case EditorMode.Drag:
                if (point is not null)
                {
                    _draggedPoint = point;
                    _isDragging = true;
                    e.Pointer.Capture(canvas);
                }
                break;
        }
    }
    
    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDragging || _draggedPoint is null)
            return;
        if (DataContext is not EditorViewModel viewModel || sender is not Control canvas)
            return;
        var position = e.GetPosition(canvas);
        int x = Math.Clamp((int)Math.Round(position.X), 0, 255);
        int y = Math.Clamp(255 - (int)Math.Round(position.Y), 0, 255);
        viewModel.TryMovePoint(_draggedPoint, new Point(x, y));
    }
    
    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
        _draggedPoint = null;
        e.Pointer.Capture(null);
    }
}
