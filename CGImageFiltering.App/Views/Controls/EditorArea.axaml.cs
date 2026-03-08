using Avalonia.Controls;
using Avalonia.Input;
using CGImageFiltering.App.ViewModels;
using CGImageFiltering.App.Views.Converters;


namespace CGImageFiltering.App.Views.Controls;

public partial class EditorArea : UserControl
{
    public EditorArea()
    {
        InitializeComponent();
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if(DataContext is not EditorViewModel viewModel || sender is not Control canvas)
            return;
        
        var position = e.GetPosition(canvas);
        var logicalPosition = CanvasPositionConverter.ToLogical(position);
        
    }
}
