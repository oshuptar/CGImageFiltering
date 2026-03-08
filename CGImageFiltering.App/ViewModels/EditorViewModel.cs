using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using CGImageFiltering.App.Models.Editor;
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
    public ObservableCollection<FilterPoint> GraphPoints { get; set; } = [new FilterPoint(0, 0), new FilterPoint(128,128), new FilterPoint(255, 255)]; // this array must be sorted
    public Points PolylinePoints => new Points(GraphPoints.Select(point => new Point(point.X, 255 - point.Y)).ToList());
}