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
    public MainWindow()
    {
      InitializeComponent();
    }

    // TODO: drop xml on window to open
    // TODO: editor window hovers 

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      // TODO: call VM with event args

      if (!e.Handled)
      {
        ((IDisposable)DataContext)?.Dispose();
        Close();
      }
    }
    private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
  }
}
