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
    private CrosswordLineViewModel _viewModel;

    public CrosswordLineView()
    {
      InitializeComponent();

      this.DataContextChanged += CrosswordLineView_DataContextChanged;
    }

    private void CrosswordLineView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      _viewModel = DataContext as CrosswordLineViewModel;
      if (_viewModel is not null)
      {
        _viewModel.PropertyChanged += HandlePropertyChangedFromViewModel;

        HandlePropertyChangedFromViewModel(sender, new PropertyChangedEventArgs(nameof(_viewModel.ControlWidth)));
      }
    }

    private void HandlePropertyChangedFromViewModel(object sender_, PropertyChangedEventArgs e_)
    {
      if (e_.PropertyName == nameof(_viewModel.ControlWidth)
        || e_.PropertyName == nameof(_viewModel.LineItem.LineWord))
      {
        var chars = new char[_viewModel.ControlWidth * 2 + 1];

        int positionToMark = 0;

        for (int i = 0; i < _viewModel.LineItem.LineWord.Length; i++)
        {
          chars[_viewModel.ControlWidth - _viewModel.LineItem.SolutionCharacterNumberInLineWord + i] =
            _viewModel.LineItem.LineWord[i];

          if (i + 1 == _viewModel.LineItem.SolutionCharacterNumberInLineWord)
          {
            positionToMark = _viewModel.ControlWidth - _viewModel.LineItem.SolutionCharacterNumberInLineWord + i;
          }
        }

        _container.Children.Clear();

        for (int i = 0; i < chars.Length; i++)
        {
          var labelToAdd = new Label()
          {
            Content = chars[i],
            Visibility = chars[i] != 0 ? Visibility.Visible : Visibility.Hidden,
          };

          if (i == positionToMark)
          {
            labelToAdd.BorderThickness = new Thickness(2, 
              _viewModel.LineItem.PlaceInCrossword == 1 ? 2 : 1,
              2,
              _viewModel.LineItem.IsLastInCrossword ? 2 : 1);
          }
          _container.Children.Add(labelToAdd);
        }
      }
    }
  }
}
