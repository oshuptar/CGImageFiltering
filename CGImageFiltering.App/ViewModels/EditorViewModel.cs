using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using Avalonia;
using CGImageFiltering.App.Models.Editor;
using CGImageFiltering.App.Models.Editor.Enums;
using CGImageFiltering.App.Models.Editor.Interfaces;
using CGImageFiltering.App.ViewModels.Abstractions;
using GCImageFiltering.Core.Filters.Convolution;
using GCImageFiltering.Core.Filters.Dithering;
using GCImageFiltering.Core.Filters.Function;
using GCImageFiltering.Core.Filters.Interfaces;
using GCImageFiltering.Core.Quantization;

namespace CGImageFiltering.App.ViewModels;

public class EditorViewModel : ViewModelBase
{
    public ObservableCollection<IFilterOption> Filters { get; } =
    [
        new FilterOption("Brightness", 20, "Delta" , min: 1, max: 255, increment: 1, (parameter) => parameter == null ? new BrightnessFilter() : new BrightnessFilter((int)parameter.Value)),
        new FilterOption("Inversion",_ => new InversionFilter()),
        new FilterOption("Contrast Enhancement", 1.2, "Contrast", min: 1, max: 2, increment: 0.1, (parameter) => parameter == null ? new ContrastEnhancementFilter() : new ContrastEnhancementFilter(parameter.Value)),
        new FilterOption("Gamma Correction", _ => new GammaCorrectionFilter()),
        new FilterOption("Blur", _ => new BoxBlurConvolutionFilter()),
        new FilterOption("Gaussian Smoothing", _ => new GaussianSmoothingConvolutionFilter()),
        new FilterOption("Sharpen", _ => new SharpenConvolutionFilter()),
        new FilterOption("Horizontal Edge Detection", _ => new HorizontalEdgeDetectionConvolutionFilter()),
        new FilterOption("East Emboss", _ => new EastEmbossConvolutionFilter()),
        new FilterOption("Random Dithering", 4, "Color Levels", min: 2, max: 256, increment: 1,(parameter) => parameter == null ? new RandomDithering() : new RandomDithering((int)parameter.Value)),
        new FilterOption("Octree Quantization", parameter: 256, "Colors", min: 1, max: 256 * 256 * 256, increment: 1, (parameter) => parameter == null ? new OctreeQuantization() : new OctreeQuantization((int)parameter.Value))
    ];

    public IFilterOption? SelectedFilter
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged();
            EditGraphCommand.RaiseCanExecuteChanged();
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
    public Commands.RelayCommand NewGraphCommand { get; }
    public Commands.RelayCommand EditGraphCommand { get; }
    public Commands.RelayCommand SaveGraphCommand { get; }
    public ObservableCollection<FilterPoint> GraphPoints { get; set; } = new(); // this array must be sorted
    public Points PolylinePoints => new Points(GraphPoints.Select(point => new Point(point.ScreenX, point.ScreenY)).ToList());
    public EditorViewModel()
    {
        SetAddModeCommand = new Commands.RelayCommand(_ => ChangeEditorMode(EditorMode.Add), _ => true);
        SetDragModeCommand = new Commands.RelayCommand(_ => ChangeEditorMode(EditorMode.Drag), _ => true);
        SetRemoveModeCommand = new Commands.RelayCommand(_ => ChangeEditorMode(EditorMode.Remove), _ => true);
        NewGraphCommand = new Commands.RelayCommand(_ => NewGraph(), _ => true);
        EditGraphCommand = new Commands.RelayCommand(_ => EditFilter(), _ => SelectedFilter is not null);
        SaveGraphCommand = new Commands.RelayCommand(_ => SaveFilter(), _ => IsFilterSet());
        GraphPoints.CollectionChanged += OnGraphPointsCollectionChanged;
    }
    
    private string _filterName = string.Empty;
    public string FilterName
    {
        get => _filterName;
        set
        {
            if (_filterName == value) return;
            _filterName = value;
            OnPropertyChanged();
        }
    }

    private bool _isFilterNameEditable = true;
    public bool IsFilterNameEditable
    {
        get => _isFilterNameEditable;
        set
        {
            if (_isFilterNameEditable == value) return;
            _isFilterNameEditable = value;
            OnPropertyChanged();
        }
    }

    private bool IsFilterSet() => GraphPoints.Count >= 2;

    private void ChangeEditorMode(EditorMode mode)
    {
        if(CurrentMode == mode)
            CurrentMode = EditorMode.None;
        else
            CurrentMode = mode;
    }

    public bool TryAddPoint(Point position)
    {
        if(!IsFilterSet())
            return false;
        
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
        if (index <= 0 || index == GraphPoints.Count - 1)
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
        SaveGraphCommand.RaiseCanExecuteChanged();
    }

    private void OnGraphPointPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(PolylinePoints));
    }

    private void NewGraph()
    {
        FilterName = string.Empty;
        IsFilterNameEditable = true;
        ResetGraph();
    }
    
    private void ResetGraph()
    {
        GraphPoints.Clear();
        GraphPoints.Add(new FilterPoint(0, 0));
        GraphPoints.Add(new FilterPoint(255, 255));
       OnPropertyChanged(nameof(GraphPoints));
    }

    private void SaveFilter()
    {
        if (string.IsNullOrWhiteSpace(FilterName))
            return;
        
        var savedPoints = GraphPoints
            .Select(point => new System.Drawing.Point(point.X, point.Y))
            .ToList();
        Func<double?, IFilter> factory = _ => new FunctionalFilter(savedPoints.ToList());
        var existing = Filters.FirstOrDefault(x => x.Name == FilterName);
        if (existing is null)
            Filters.Add(new FilterOption(FilterName, factory));
        else
            existing.FilterFactory = factory;
       FilterName = string.Empty;
       IsFilterNameEditable = true;
       GraphPoints.Clear();
    }

    private void EditFilter()
    {
        IFilter? filter = SelectedFilter?.FilterFactory.Invoke(SelectedFilter.Parameter);
        if (filter is not IGraphRepresentable graphRepresentable)
            return;
        
        IsFilterNameEditable = false;
        FilterName = SelectedFilter?.Name ?? string.Empty;
        
        var points = graphRepresentable.BuildGraphPoints().ToList();
        GraphPoints.Clear();
        foreach (var point in points)
            GraphPoints.Add(new FilterPoint(point.X, point.Y));
    }
}