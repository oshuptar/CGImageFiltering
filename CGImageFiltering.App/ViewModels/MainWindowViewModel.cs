    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Avalonia.Media.Imaging;
    using Avalonia.Platform.Storage;
    using Avalonia.Threading;
    using CGImageFiltering.App.Buffers;
    using CGImageFiltering.App.Converters;
    using CGImageFiltering.App.Converters.Interfaces;
    using CGImageFiltering.App.Models;
    using CGImageFiltering.App.Models.Interfaces;
    using GCImageFiltering.Core.Buffers;
    using GCImageFiltering.Core.Filters.Convolution;
    using GCImageFiltering.Core.Filters.Function;
    using GCImageFiltering.Core.Filters.Interfaces;

    namespace CGImageFiltering.App.ViewModels;

    public class MainWindowViewModel : ViewModelBase
    {
        private DirectBitmap? _image;
        public DirectBitmap? Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsSaveEnabled));
            }
        }
        public string PreviewButtonText => IsOriginalDisplayed ? " Show Filtered" : "Show Original";
        private bool _isOriginalDisplayed = false;
        public bool IsOriginalDisplayed
        {
            get => _isOriginalDisplayed;
            private set
            {
                if (_isOriginalDisplayed == value) return;
                _isOriginalDisplayed = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PreviewButtonText));
            }
        }
        
        private bool _isBusy = false;
        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                OnPropertyChanged();
                RefreshCommands();
            }
        }
        public bool IsSaveEnabled => Image is not null;
        private DirectBitmap? OriginalImage { get; set; }
        private DirectBitmap? FilteredImage { get; set; }
        public Commands.RelayCommand OpenFileCommand { get; }
        public Commands.RelayCommand SaveFileCommand { get; }
        public Commands.RelayCommand ToggleImagePreviewCommand { get; }
        public Commands.RelayCommand ApplySelectedFilterCommand { get; }

        public ObservableCollection<IFilterOption> Filters { get; } =
        [
            new FilterOption("Brightness", () => new BrightnessFilter()),
            new FilterOption("Inversion", () => new InversionFilter()),
            new FilterOption("Contrast Enhancement", () => new ContrastEnhancementFilter()),
            new FilterOption("Gamma Correction", () => new GammaCorrectionFilter()),
            new FilterOption("Blur", () => new BoxBlurConvolutionFilter()),
            new FilterOption("Gaussian Smoothing", () => new GaussianSmoothingConvolutionFilter()),
            new FilterOption("Sharpen", () => new SharpenConvolutionFilter()),
            new FilterOption("Horizontal Edge Detection", () => new HorizontalEdgeDetectionConvolutionFilter())
        ];
            
        public IFilterOption SelectedFilter { get; set; }
        public MainWindowViewModel()
        {
            OpenFileCommand = new Commands.RelayCommand(OpenFileDialog, _ => true);
            SaveFileCommand = new Commands.RelayCommand(SaveFile, _ => Image is not null);
            ToggleImagePreviewCommand =  new Commands.RelayCommand(ToggleImagePreview, _ => Image is not null);
            ApplySelectedFilterCommand = new Commands.RelayCommand(_ => ApplyFilter(SelectedFilter!.FilterFactory()), CanApplyFilter);
        }

        private async void OpenFileDialog(object? parameter)
        {
            if (parameter is not IStorageFile file) return;
            await using var stream = await file.OpenReadAsync();
            Bitmap bitmap = new Bitmap(stream);
            OriginalImage = DirectBitmap.ToRgba(bitmap);
            FilteredImage = DirectBitmap.ToRgba(bitmap);
            Image = FilteredImage;
            IsOriginalDisplayed = false;
            RefreshCommands();
        }

        private async void SaveFile(object? parameter)
        {
            // TODO: add dialog notifying the user that there is no image loaded
            
            if (parameter is not IStorageFile file) return;
            await using var stream = await file.OpenWriteAsync();
            FilteredImage?.Bitmap.Save(stream);
        }
        
        private void ToggleImagePreview(object? parameter)
        {
            if(IsOriginalDisplayed)
                Image = FilteredImage;
            else 
                Image = OriginalImage;
            IsOriginalDisplayed = !IsOriginalDisplayed;
            RefreshCommands();
        }

        private async void ApplyFilter(IFilter filter)
        {
            if (Image is null) return;
            
            var image = Image; 
            var width = image.Width;
            var height = image.Height;
            var sourceBytes = image.Pixels.ToArray();
            
            IByteConverter byteConverter = new RgbaByteConverter(); 
            IsBusy = true;
            var result = await Task.Run(() =>
            {
                var pixelBuffer = new PixelBuffer(
                    width,
                    height,
                    byteConverter.ConvertToPixel(sourceBytes, width, height));

                var filteredBuffer = filter.Apply(pixelBuffer);

                return byteConverter.ConvertToByte(
                    filteredBuffer.Pixels,
                    filteredBuffer.Width,
                    filteredBuffer.Height);
            });
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                // In case race condition happened and the user changed the image while the filter was running
                if (!ReferenceEquals(image, Image))
                    return;
                
                image.Pixels = result;
                image.UpdateBitmap();
                InvalidateImage();
                IsBusy = false;
            });
        }
        

        private bool CanApplyFilter(object? parameter)
        {
            return Image is not null && !IsOriginalDisplayed && !IsBusy;
        }

        private void InvalidateImage()
        {
            // Force to make sure UI redraws updated writeable bitmap. TODO: fix. think about different solution
            DirectBitmap? current = Image;
            Image = null;
            Image = current;
            RefreshCommands();
        }

        private void RefreshCommands()
        {
            SaveFileCommand.RaiseCanExecuteChanged();
            ToggleImagePreviewCommand.RaiseCanExecuteChanged();
            ApplySelectedFilterCommand.RaiseCanExecuteChanged();
        }
    }
