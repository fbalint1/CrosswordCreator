using CrosswordCreator.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

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
      _viewModel = (CrosswordLineViewModel)DataContext;
      if (_viewModel is not null)
      {
        _viewModel.PropertyChanged += HandlePropertyChangedFromViewModel;

        HandlePropertyChangedFromViewModel(sender, new PropertyChangedEventArgs(nameof(_viewModel.ControlWidthLeft)));
      }
    }

    private void HandlePropertyChangedFromViewModel(object sender_, PropertyChangedEventArgs e_)
    {
      if (e_.PropertyName == nameof(_viewModel.ControlWidthLeft)
        || e_.PropertyName == nameof(_viewModel.LineItem.LineWord))
      {
        var chars = new char[_viewModel.ControlWidthLeft + _viewModel.ControlWidthRight + 1];

        int positionToMark = 0;

        for (int i = 0; i < _viewModel.LineItem.LineWord.Length; i++)
        {
          int index = _viewModel.ControlWidthLeft - _viewModel.LineItem.SolutionCharacterNumberInLineWord + 1 + i;
          chars[index] =
            _viewModel.LineItem.LineWord[i];

          if (i + 1 == _viewModel.LineItem.SolutionCharacterNumberInLineWord)
          {
            positionToMark = _viewModel.ControlWidthLeft - _viewModel.LineItem.SolutionCharacterNumberInLineWord + 1 + i;
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
            labelToAdd.BorderThickness = new Thickness(3,
              _viewModel.LineItem.PlaceInCrossword == 1 ? 3 : 1,
              3,
              _viewModel.LineItem.IsLastInCrossword ? 3 : 1);
          }
          _container.Children.Add(labelToAdd);
        }
      }
    }
  }
}
