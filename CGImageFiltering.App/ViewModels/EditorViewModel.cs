using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using CGImageFiltering.App.Models.Editor;
using CGImageFiltering.App.Models.Editor.Enums;
using CGImageFiltering.App.Models.Editor.Interfaces;
using CGImageFiltering.App.ViewModels.Abstractions;
using CGImageFiltering.App.Views.Converters;
using GCImageFiltering.Core.Filters.Convolution;
using GCImageFiltering.Core.Filters.Function;

namespace CGImageFiltering.App.ViewModels;

public class EditorViewModel : ViewModelBase
{
    public ObservableCollection<IFilterOption> Filters { get; } =
    [
        new FilterOption("Brightness", () => new BrightnessFilter()),
        new FilterOption("Inversion", () => new InversionFilter()),
        new FilterOption("Contrast Enhancement", () => new ContrastEnhancementFilter()),
        new FilterOption("Gamma Correction", () => new GammaCorrectionFilter()),
        new FilterOption("Blur", () => new BoxBlurConvolutionFilter()),
        new FilterOption("Gaussian Smoothing", () => new GaussianSmoothingConvolutionFilter()),
        new FilterOption("Sharpen", () => new SharpenConvolutionFilter()),
        new FilterOption("Horizontal Edge Detection", () => new HorizontalEdgeDetectionConvolutionFilter()),
        new FilterOption("East Emboss", () => new EastEmbossConvolutionFilter())
    ];

    public IFilterOption? SelectedFilter
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    private EditorMode _currentMode = EditorMode.None;
    public EditorMode CurrentMode
    {
        get => _currentMode;
        set
        {
            if (_currentMode == value) return;
            _currentMode = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsAddMode));
            OnPropertyChanged(nameof(IsDragMode));
            OnPropertyChanged(nameof(IsRemoveMode));
        }
    }

    public bool IsAddMode => CurrentMode == EditorMode.Add;
    public bool IsDragMode => CurrentMode == EditorMode.Drag;
    public bool IsRemoveMode => CurrentMode == EditorMode.Remove;
    
    public Commands.RelayCommand SetAddModeCommand { get; }
    public Commands.RelayCommand SetDragModeCommand { get; }
    public Commands.RelayCommand SetRemoveModeCommand { get; }
    
    public ObservableCollection<FilterPoint> GraphPoints { get; set; } = [
        new FilterPoint(0, 0),
        new FilterPoint(128,128),
        new FilterPoint(255, 255)
    ]; // this array must be sorted
    public Points PolylinePoints => new Points(GraphPoints.Select(point => CanvasPositionConverter.ToScreen(new Point(point.X, point.Y))).ToList());

    public EditorViewModel()
    {
        SetAddModeCommand = new Commands.RelayCommand(_ => ChangeEditorMode(EditorMode.Add), _ => true);
        SetDragModeCommand = new Commands.RelayCommand(_ => ChangeEditorMode(EditorMode.Drag), _ => true);
        SetRemoveModeCommand = new Commands.RelayCommand(_ => ChangeEditorMode(EditorMode.Remove), _ => true);
    }

    private void ChangeEditorMode(EditorMode mode)
    {
        if(CurrentMode == mode)
            CurrentMode = EditorMode.None;
        else
            CurrentMode = mode;
    }

    public bool TryAddPoint(int x, int y)
    {
        if (x <= 0 || x >= 255 || y < 0 || y > 255)
            return false;

        if (GraphPoints.Any(p => p.X == x))
            return false;

        var point = new FilterPoint(x, y);

        InsertPoint(point);
        return true;
    }

    public FilterPoint? TryFindPointAt(int x, int y)
    {
        throw new NotImplementedException();
    }

    public bool TryRemovePoint(FilterPoint point)
    {
        int index = GraphPoints.IndexOf(point);
        if (index == 0 || index == GraphPoints.Count - 1)
            return false;

        bool removed = GraphPoints.Remove(point);
        if (removed)
            OnPropertyChanged(nameof(PolylinePoints));
        return removed;
    }

    public void MovePoint(FilterPoint point, int x, int y)
    {
        throw new NotImplementedException();
    }

    private void InsertPoint(FilterPoint point)
    {
        int insertIndex = 0;
        while (insertIndex < GraphPoints.Count && GraphPoints[insertIndex].X < point.X)
            insertIndex++;

        GraphPoints.Insert(insertIndex, point);
        OnPropertyChanged(nameof(PolylinePoints));
    }
}