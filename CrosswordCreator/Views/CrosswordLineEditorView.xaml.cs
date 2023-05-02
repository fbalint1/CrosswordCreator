using CrosswordCreator.ViewModels;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace CrosswordCreator.Views
{
  /// <summary>
  /// Interaction logic for CrosswordLineEditorView.xaml
  /// </summary>
  public partial class CrosswordLineEditorView : Window
  {
    private readonly CrosswordLineEditorViewModel _viewModel;

    public CrosswordLineEditorView()
    {
      InitializeComponent();
    }

    public CrosswordLineEditorView(CrosswordLineEditorViewModel crosswordLineEditorViewModel_)
      : this()
    {
      DataContext = crosswordLineEditorViewModel_;
      _viewModel = crosswordLineEditorViewModel_;

      WordTextBox.Document.Blocks.Clear();

      WordTextBox.TextChanged += WordTextBox_TextChanged;

      WordTextBox.Document.Blocks.Add(new Paragraph(new Run(_viewModel.LineWord)));

      HighlightCharacter(WordTextBox);
    }

    private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

    private void WordTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      var richTextBox = sender as RichTextBox;
      var newText = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd)
        .Text
        .Replace("\r\n", string.Empty)
        .Trim();

      if (!newText.Equals(_viewModel.LineWord))
      {
        _viewModel.LineWord = newText;
        HighlightCharacter(richTextBox);
      }
    }

    private void HighlightCharacter(RichTextBox richTextBox_)
    {
      var newTextRange = new TextRange(richTextBox_.Document.ContentStart, richTextBox_.Document.ContentEnd);
      
      newTextRange.ApplyPropertyValue(ForegroundProperty, Brushes.Black);

      var currentPosition = richTextBox_.Document.ContentStart;

      while (currentPosition != null)
      {
        if (currentPosition.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
        {
          var text = currentPosition.GetTextInRun(LogicalDirection.Forward);

          var startIndex = text.IndexOf(_viewModel.LineWord);

          if (startIndex >= 0)
          {
            var startPosition = currentPosition.GetPositionAtOffset(startIndex + _viewModel.CharacterPlaceInWord);
            var endPosition = startPosition.GetPositionAtOffset(1);

            var range = new TextRange(startPosition, endPosition);
            range.ApplyPropertyValue(ForegroundProperty, Brushes.Red);

            richTextBox_.InvalidateVisual();
            break;
          }
        }

        currentPosition = currentPosition.GetNextContextPosition(LogicalDirection.Forward);
      }
    }
  }
}
