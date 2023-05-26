using CrosswordCreator.Models;
using CrosswordCreator.Models.Enums;
using CrosswordCreator.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;

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

    private void Button_Click(object sender, RoutedEventArgs e)
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
  }
}
