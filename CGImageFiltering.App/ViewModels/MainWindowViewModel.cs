    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Avalonia.Media.Imaging;
    using Avalonia.Platform;
    using Avalonia.Platform.Storage;
    using Avalonia.Threading;
    using CGImageFiltering.App.Buffers;
    using CGImageFiltering.App.ViewModels.Abstractions;
    using GCImageFiltering.Core.Buffers;
    using GCImageFiltering.Core.Buffers.Enums;
    using GCImageFiltering.Core.Converters;
    using GCImageFiltering.Core.Converters.Abstractions;
    using GCImageFiltering.Core.Filters.Interfaces;

    namespace CGImageFiltering.App.ViewModels;

    public class MainWindowViewModel : ViewModelBase
    {
        private DirectBitmap? OriginalImage
        {
            get => field;
            set
            {
                field = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsSaveEnabled));
                OnPropertyChanged(nameof(Image));
            }
        }

        private DirectBitmap? FilteredImage
        {
            get => field;
            set
            {
                field = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsSaveEnabled));
                OnPropertyChanged(nameof(Image));
            }
        }
        public DirectBitmap? Image => _isOriginalDisplayed ? OriginalImage : FilteredImage;
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
                OnPropertyChanged(nameof(Image));
            }
        }

        public bool IsBusy
        {
            get => field;
            private set
            {
                if (field == value) return;
                field = value;
                OnPropertyChanged();
                RefreshCommands();
            }
        }

        public bool IsSaveEnabled => Image is not null;
        public Commands.RelayCommand OpenFileCommand { get; }
        public Commands.RelayCommand SaveFileCommand { get; }
        public Commands.RelayCommand ResetImageCommand { get; }
        public Commands.RelayCommand ToggleImagePreviewCommand { get; }
        public Commands.RelayCommand ApplySelectedFilterCommand { get; }
        public Commands.RelayCommand ConvertToGrayscaleCommand { get; }
        public EditorViewModel EditorViewModel { get; } = new();

        public MainWindowViewModel()
        {
            OpenFileCommand = new Commands.RelayCommand(OpenFile, _ => !IsBusy);
            SaveFileCommand = new Commands.RelayCommand(SaveFile, _ => Image is not null);
            ResetImageCommand = new Commands.RelayCommand(ResetImage, _ => Image is not null);
            ToggleImagePreviewCommand = new Commands.RelayCommand(ToggleImagePreview, _ => Image is not null);
            ApplySelectedFilterCommand =
                new Commands.RelayCommand(_ => ApplyFilter(EditorViewModel.SelectedFilter!.FilterFactory()),
                    CanApplyFilter);
            ConvertToGrayscaleCommand = new Commands.RelayCommand(ConvertToGrayscale, CanModifyImage);
            EditorViewModel.PropertyChanged += OnEditorViewModelPropertyChanged;
        }

        private void OnEditorViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e) => RefreshCommands();

        private void ConvertToGrayscale(object? parameter)
        {
            if(Image is null) return;
            var pixels = Image.Pixels.ToArray();
            IConverter grayscaleConverter = new GrayscaleConverter();
            var resultBuffer = grayscaleConverter.Convert(new PixelBuffer(Image.Width, Image.Height, pixels, Image.Stride,
                Image.PixelFormat == PixelFormats.Gray8 ? ColorFormat.Grayscale : ColorFormat.Rgba));
            FilteredImage = new DirectBitmap(resultBuffer.Width, resultBuffer.Height, Image.Dpi, PixelFormats.Gray8, resultBuffer.Pixels);
        }
        
        private async void OpenFile(object? parameter)
        {
            if (parameter is not IStorageFile file) return;
            await using var stream = await file.OpenReadAsync();
            Bitmap bitmap = new Bitmap(stream);
            OriginalImage = DirectBitmap.ToRgba(bitmap);
            FilteredImage = DirectBitmap.ToRgba(bitmap);
            //Image = FilteredImage;
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
            // if (IsOriginalDisplayed)
            //     Image = FilteredImage;
            // else
            //     Image = OriginalImage;
            IsOriginalDisplayed = !IsOriginalDisplayed;
            RefreshCommands();
        }
        
        private void ResetImage(object? parameter)
        {
            if (OriginalImage is null)
                return;
            FilteredImage = new DirectBitmap(
                OriginalImage.Width,
                OriginalImage.Height,
                OriginalImage.Dpi,
                OriginalImage.PixelFormat,
                OriginalImage.Pixels
            );
            //Image = FilteredImage;
            IsOriginalDisplayed = false;
            RefreshCommands();
        }

        private async void ApplyFilter(IFilter filter)
        {
            if (Image is null) return;

            var image = Image;
            var width = image.Width;
            var height = image.Height;
            var sourceBytes = image.Pixels.ToArray();

            IsBusy = true;
            var result = await Task.Run(() =>
            {
                var pixelBuffer = new PixelBuffer(
                    width,
                    height, 
                    sourceBytes,
                    image.Stride,
                    image.PixelFormat == PixelFormats.Gray8 ? ColorFormat.Grayscale : ColorFormat.Rgba);

                var filteredBuffer = filter.Apply(pixelBuffer);

                return filteredBuffer.Pixels;
            });
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                // In case race condition happened and the user changed the image while the filter was running
                try
                {
                    if (!ReferenceEquals(image, Image))
                        return;

                    image.Pixels = result;
                    image.UpdateBitmap();
                    InvalidateImage();
                }
                finally
                {
                    IsBusy = false;
                }
            });
        }
        private bool CanModifyImage(object? parameter) => Image is not null && !IsOriginalDisplayed && !IsBusy;
        private bool CanApplyFilter(object? parameter) => CanModifyImage(parameter) && EditorViewModel.SelectedFilter is not null;

        private void InvalidateImage()
        {
            // Force to make sure UI redratruews updated writeable bitmap. TODO: fix. think about different solution
            DirectBitmap? current = FilteredImage;
            FilteredImage = null;
            FilteredImage = current;
            RefreshCommands();
        }

        private void RefreshCommands()
        {
            SaveFileCommand.RaiseCanExecuteChanged();
            ToggleImagePreviewCommand.RaiseCanExecuteChanged();
            ResetImageCommand.RaiseCanExecuteChanged();
            ApplySelectedFilterCommand.RaiseCanExecuteChanged();
            ConvertToGrayscaleCommand.RaiseCanExecuteChanged();
        }
    }
