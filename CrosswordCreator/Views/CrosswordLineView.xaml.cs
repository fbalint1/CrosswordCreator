using CrosswordCreator.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CrosswordCreator.Views
{
  /// <summary>
  /// Interaction logic for CrosswordLineView.xaml
  /// </summary>
  public partial class CrosswordLineView : UserControl
  {
    private readonly CrosswordLineViewModel _viewModel;
    private char _chart = 'A';
    private int amount = 3;

    public CrosswordLineView()
    {
      InitializeComponent();

      //_viewModel = DataContext as CrosswordLineViewModel;
      //_viewModel.PropertyChanged += HandlePropertyChangedFromViewModel;

      _container.Children.Clear();

      for (int i = 0; i < amount; i++)
      {
        var labelToAdd = new Label()
        {
          Content = _chart
        };

        _container.Children.Add(labelToAdd);
      }
    }

    //public void Change()
    //{
    //  _chart++;
    //  amount++;
    //  _container.Children.Clear();

    //  for (int i = 0; i < amount; i++)
    //  {
    //    var labelToAdd = new Label()
    //    {
    //      Content = _chart,
    //      Visibility = i % 2 == 0 ? Visibility.Visible : Visibility.Hidden
    //    };

    //    _container.Children.Add(labelToAdd);
    //  }
    //}

    private void HandlePropertyChangedFromViewModel(object sender_, PropertyChangedEventArgs e_)
    {
      if (e_.PropertyName == nameof(_viewModel.ControlWidth))
      {
        // width: 3 
        // valami
        var word = _viewModel.LineItem.LineWord;

        _container.Children.Clear();

        for (int i = 0; i < _viewModel.ControlWidth; i++)
        {
          var labelToAdd = new Label()
          {
            Content = "A"
          };

          _container.Children.Add(labelToAdd);
        }
      }
    }
  }
}
