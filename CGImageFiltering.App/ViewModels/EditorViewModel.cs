using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using CGImageFiltering.App.Models.Editor;
using CGImageFiltering.App.Models.Editor.Enums;
using CGImageFiltering.App.Models.Editor.Interfaces;
using CGImageFiltering.App.ViewModels.Abstractions;
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
        new FilterPoint(255, 255)
    ]; // this array must be sorted
    public Points PolylinePoints => new Points(GraphPoints.Select(point => new Point(point.ScreenX, point.ScreenY)).ToList());

    public EditorViewModel()
    {
        SetAddModeCommand = new Commands.RelayCommand(_ => ChangeEditorMode(EditorMode.Add), _ => true);
        SetDragModeCommand = new Commands.RelayCommand(_ => ChangeEditorMode(EditorMode.Drag), _ => true);
        SetRemoveModeCommand = new Commands.RelayCommand(_ => ChangeEditorMode(EditorMode.Remove), _ => true);
        GraphPoints.CollectionChanged += OnGraphPointsCollectionChanged;
        foreach (var point in GraphPoints)
            point.PropertyChanged += OnGraphPointPropertyChanged;
    }

    private void ChangeEditorMode(EditorMode mode)
    {
        if(CurrentMode == mode)
            CurrentMode = EditorMode.None;
        else
            CurrentMode = mode;
    }

    public bool TryAddPoint(Point position)
    {
        int x = (int)position.X; int y = (int)position.Y;
        if (CurrentMode != EditorMode.Add)
            return false;
        if (x<= 0 || x >= 255 || y < 0 || y > 255)
            return false;
        if (GraphPoints.Any(p => p.X == x))
            return false;

        var newPoint = new FilterPoint(x, y);
        InsertPoint(newPoint);
        return true;
    }

    public FilterPoint? TryFindPointAt(Point position, int radius)
    {
        return GraphPoints.Select(p => new
            {
                Point = p,
                DistanceSquared = Math.Pow((p.X - (int)position.X), 2) +Math.Pow((p.Y - (int)position.Y),2)
            })
            .Where(x => x.DistanceSquared <= radius * radius)
            .OrderBy(x => x.DistanceSquared)
            .Select(x => x.Point)
            .FirstOrDefault();
    }

    public bool TryRemovePoint(FilterPoint point)
    {
        int index = GraphPoints.IndexOf(point);
        if (index == 0 || index == GraphPoints.Count - 1)
            return false;

        return GraphPoints.Remove(point);
    }

    public bool TryMovePoint(FilterPoint point, Point position)
    {
        int index = GraphPoints.IndexOf(point);
        if (index < 0)
            return false;
        
        int newY = Math.Clamp((int)position.Y, 0, 255);
        bool isEndpoint = index == 0 || index == GraphPoints.Count - 1;
        if (isEndpoint)
        {
            point.Y = newY;
            return true;
        }
        int minX = GraphPoints[index - 1].X + 1;
        int maxX = GraphPoints[index + 1].X - 1;
        int newX = Math.Clamp((int)position.X, minX, maxX);
        point.X = newX;
        point.Y = newY;
        return true;
    }

    private void InsertPoint(FilterPoint point)
    {
        int insertIndex = 0;
        while (insertIndex < GraphPoints.Count && GraphPoints[insertIndex].X < point.X)
            insertIndex++;

        GraphPoints.Insert(insertIndex, point);
    }
    
    private void OnGraphPointsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            foreach (FilterPoint point in e.NewItems)
                point.PropertyChanged += OnGraphPointPropertyChanged;
        }
        if (e.OldItems is not null)
        {
            foreach (FilterPoint point in e.OldItems)
                point.PropertyChanged -= OnGraphPointPropertyChanged;
        }
        OnPropertyChanged(nameof(PolylinePoints));
    }

    private void OnGraphPointPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(PolylinePoints));
    }
}