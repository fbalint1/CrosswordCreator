using CrosswordCreator.Models;
using CrosswordCreator.Models.Enums;
using CrosswordCreator.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using CrosswordCreator.Utilities;
using System.Drawing;
using System.IO;
using Point = System.Windows.Point;

namespace CrosswordCreator
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
      InitializeComponent();

      _viewModel = DataContext as MainViewModel;
    }

    private void MinimizeButtonClicked(object sender, RoutedEventArgs e)
    {
      WindowState = WindowState.Minimized;
    }

    private void MaximizeButtonClicked(object sender, RoutedEventArgs e)
    {
      WindowState = WindowState == WindowState.Maximized 
        ? WindowState.Normal 
        : WindowState.Maximized;
    }

    private void CloseButtonClicked(object sender, RoutedEventArgs e)
    {
      var promptResult = _viewModel.PromptUserForSave();

      switch (promptResult)
      {
        case PromptResultEnum.Cancel:
          e.Handled = true;
          break;
        case PromptResultEnum.Continue:
          _viewModel.SaveCurrentCrossword();
          break;
        case PromptResultEnum.Skip:
          break;
        default:
          throw new InvalidOperationException("Unmapped enum state");
      }

      if (!e.Handled)
      {
        _viewModel?.Dispose();
        Close();
      }
    }
    private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

    private void Window_Drop(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(DataFormats.FileDrop))
      {
        var files = (string[])e.Data.GetData(DataFormats.FileDrop);

        if (files[0].EndsWith(".xml"))
        {
          try
          {
            _viewModel.SetCurrentCrossword(CrosswordSerializer.GetCrossword(files[0]));
          }
          catch (Exception)
          {
            _viewModel.StatusEnum = StatusEnum.LoadFailed;
          }
        }
      }
    }

    private void CopyScreenshotButtonClicked(object sender, RoutedEventArgs e)
    {
      try
      {
        double height, width;
        int renderHeight, renderWidth;

        height = ItemList.RenderSize.Height;
        renderHeight = (int)height;
        width = ItemList.RenderSize.Width;
        // Subtract 40 from the width to cut down the edit row button from the right 
        renderWidth = (int)width - 40;

        var renderTarget = new RenderTargetBitmap(renderWidth, renderHeight, 96, 96, PixelFormats.Pbgra32);

        var visualBrush = new VisualBrush(ItemList);

        var drawingVisual = new DrawingVisual();
        using (DrawingContext drawingContext = drawingVisual.RenderOpen())
        {
          // Still render the full distance otherwise the controls visual will be adjusted to fit everything
          // But this way the buttons end up being rendered out of bounds, and thus disappear
          drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(0, 0), new Point(width, height)));
        }

        renderTarget.Render(drawingVisual);

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(renderTarget));

        using (var stream = new MemoryStream())
        {
          encoder.Save(stream);
          var image = Image.FromStream(stream);
          ClipboardUtil.WriteToClipboard(image);
        }

        _viewModel.StatusEnum = StatusEnum.CopySuccessful;
      }
      catch (Exception)
      {
        _viewModel.StatusEnum = StatusEnum.CopyFailed;
      }
    }
  }
}
