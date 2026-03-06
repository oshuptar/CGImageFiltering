    using Avalonia.Media.Imaging;
    using Avalonia.Platform.Storage;
    using CGImageFiltering.App.Buffers;
    using CGImageFiltering.App.Converters;
    using CGImageFiltering.App.Converters.Interfaces;
    using GCImageFiltering.Core.Buffers;
    using GCImageFiltering.Core.FunctionFilters;
    using GCImageFiltering.Core.FunctionFilters.Interfaces;

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
        private DirectBitmap? OriginalImage { get; set; }
        private DirectBitmap? FilteredImage { get; set; }
        public Commands.RelayCommand OpenFileCommand { get; }
        public Commands.RelayCommand SaveFileCommand { get; }
        public Commands.RelayCommand ToggleImagePreviewCommand { get; }
        public Commands.RelayCommand ApplyBrigthnessFilterCommand { get; } 
        public Commands.RelayCommand ApplyInversionFilterCommand { get; }
        public Commands.RelayCommand ApplyContrastEnhancementFilterCommand { get; }
        public Commands.RelayCommand ApplyGammaCorrectionFilterCommand { get; }
        public MainWindowViewModel()
        {
            OpenFileCommand = new Commands.RelayCommand(OpenFileDialog, _ => true);
            SaveFileCommand = new Commands.RelayCommand(SaveFile, _ => Image is not null);
            ToggleImagePreviewCommand =  new Commands.RelayCommand(ToggleImagePreview, _ => Image is not null);
            ApplyBrigthnessFilterCommand = new Commands.RelayCommand(_ => ApplyFilter(new BrightnessFilter()), CanApplyFilter);
            ApplyInversionFilterCommand = new Commands.RelayCommand(_ => ApplyFilter(new InversionFilter()), CanApplyFilter);
            ApplyContrastEnhancementFilterCommand = new Commands.RelayCommand(_ => ApplyFilter(new ContrastEnhancementFilter()), CanApplyFilter);
            ApplyGammaCorrectionFilterCommand = new Commands.RelayCommand(_ => ApplyFilter(new GammaCorrectionFilter()), CanApplyFilter);
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

        private void ApplyFilter(IFilter filter)
        {
            if (Image is null) return;
            IByteConverter byteConverter = new RgbaByteConverter();
            PixelBuffer filteredBuffer =
                filter.Apply(new PixelBuffer(Image.Width, Image.Height, byteConverter.ConvertToPixel(Image.Pixels, Image.Width, Image.Height)));
            Image.Pixels = byteConverter.ConvertToByte(filteredBuffer.Pixels, filteredBuffer.Width, filteredBuffer.Height);
            Image.UpdateBitmap();
            InvalidateImage();
        }

        private bool CanApplyFilter(object? parameter)
        {
            return Image is not null && !IsOriginalDisplayed;
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
            ApplyBrigthnessFilterCommand.RaiseCanExecuteChanged();
            ApplyInversionFilterCommand.RaiseCanExecuteChanged();
            ApplyContrastEnhancementFilterCommand.RaiseCanExecuteChanged();
            ApplyGammaCorrectionFilterCommand.RaiseCanExecuteChanged();
        }
    }
