﻿using CrosswordCreator.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CrosswordCreator.Views
{
  /// <summary>
  /// Interaction logic for CrosswordLineView.xaml
  /// </summary>
  public partial class CrosswordLineView : UserControl
  {
    private int _currentLeft = 0;
    private int _currentRight = 0;
    private string _currentWord = "";
    private int _currentSolutionCharacter = 0;

    private CrosswordLineViewModel _viewModel;

    public CrosswordLineView()
    {
      InitializeComponent();
    }

    public int CellsLeftFromMiddle
    {
      get { return (int)GetValue(CellsLeftFromMiddleProperty); }
      set { SetValue(CellsLeftFromMiddleProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CellsLeftFromMiddle.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CellsLeftFromMiddleProperty =
        DependencyProperty.Register("CellsLeftFromMiddle", typeof(int), typeof(CrosswordLineView), new PropertyMetadata(DependencyPropertyChanged));

    public int CellsRightFromMiddle
    {
      get { return (int)GetValue(CellsRightFromMiddleProperty); }
      set { SetValue(CellsRightFromMiddleProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CellsRightFromMiddle.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CellsRightFromMiddleProperty =
        DependencyProperty.Register("CellsRightFromMiddle", typeof(int), typeof(CrosswordLineView), new PropertyMetadata(DependencyPropertyChanged));

    public string Word
    {
      get { return (string)GetValue(WordProperty); }
      set { SetValue(WordProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Word.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty WordProperty =
        DependencyProperty.Register("Word", typeof(string), typeof(CrosswordLineView), new PropertyMetadata(DependencyPropertyChanged));

    public int SolutionCharacterNumber
    {
      get { return (int)GetValue(SolutionCharacterNumberProperty); }
      set { SetValue(SolutionCharacterNumberProperty, value); }
    }

    // Using a DependencyProperty as the backing store for SolutionCharacterNumber.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SolutionCharacterNumberProperty =
        DependencyProperty.Register("SolutionCharacterNumber", typeof(int), typeof(CrosswordLineView), new PropertyMetadata(DependencyPropertyChanged));

    private static void DependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is CrosswordLineView lineView)
      {
        lineView.HandlePropertyChanged();
      }
    }

    private void HandlePropertyChanged()
    {
      if (_viewModel == null)
      {
        _viewModel = (CrosswordLineViewModel)DataContext;

        if (_viewModel == null)
        {
          return;
        }
      }

      if (CellsLeftFromMiddle != _currentLeft
        || CellsRightFromMiddle != _currentRight
        || Word != _currentWord
        || SolutionCharacterNumber != _currentSolutionCharacter)
      {
        if (_viewModel.SolutionCharacterNumber > CellsLeftFromMiddle
          || _viewModel.Word.Length - _viewModel.SolutionCharacterNumber - 1 > CellsRightFromMiddle)
        {
          return;
        }

        var chars = new char[CellsLeftFromMiddle + CellsRightFromMiddle + 1];

        int positionToMark = 0;

        for (int i = 0; i < _viewModel.Word.Length; i++)
        {
          int index = CellsLeftFromMiddle - _viewModel.SolutionCharacterNumber + i;
          chars[index] = _viewModel.Word[i];

          if (i == _viewModel.SolutionCharacterNumber)
          {
            positionToMark = CellsLeftFromMiddle - _viewModel.SolutionCharacterNumber + i;
          }
        }

        _container.Children.Clear();

        var foregroundBind = new Binding("Foreground")
        {
          Source = this,
          Mode = BindingMode.OneWay,
        };

        for (int i = 0; i < chars.Length; i++)
        {
          var labelToAdd = new Label()
          {
            Content = chars[i],
            Visibility = chars[i] != 0 ? Visibility.Visible : Visibility.Hidden,
          };
          labelToAdd.SetBinding(ForegroundProperty, foregroundBind);


          if (i == positionToMark)
          {
            labelToAdd.BorderThickness = new Thickness(3,
              _viewModel.IsFirstInCrossword ? 3 : 1,
              3,
              _viewModel.IsLastInCrossword ? 3 : 1);
          }
          _container.Children.Add(labelToAdd);
        }

        _currentLeft = CellsLeftFromMiddle;
        _currentRight = CellsRightFromMiddle;
        _currentWord = Word;
      }
    }
  }
}
