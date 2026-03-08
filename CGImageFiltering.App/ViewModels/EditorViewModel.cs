using System.Collections.ObjectModel;
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

    public ObservableCollection<Point> GraphPoints { get; set; } = [new Point(0, 255), new Point(255, 0)];
}