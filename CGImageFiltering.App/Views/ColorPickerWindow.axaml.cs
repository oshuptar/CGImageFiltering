using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using CGImageFiltering.App.ViewModels;

namespace CGImageFiltering.App.Views;

public partial class ColorPickerWindow : Window
{
    private bool _isDragging { get; set; }
    public ColorPickerWindow()
    {
        DataContext = new ColorPickerViewModel();
        InitializeComponent();
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control image) return;
        _isDragging = true;
        e.Pointer.Capture(image);
    }
    
    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDragging) return;
        if (DataContext is not ColorPickerViewModel viewModel || sender is not Control image) return;
        var position = e.GetPosition(image);
        int x = Math.Clamp((int)Math.Round(position.X), 0, viewModel.Width);
        int y = Math.Clamp(viewModel.Height - (int)Math.Round(position.Y), 0, viewModel.Height);
        viewModel.SetNewColor(saturation: x, value: y);
    }
    
    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
        e.Pointer.Capture(null);
    }

    private void RangeBase_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        double newValue = e.NewValue;
        if (DataContext is not ColorPickerViewModel viewModel) return;
        viewModel.SetNewColor(hue: (int)newValue);
    }
}