using CrosswordCreator.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

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
      _viewModel.DataChangedInViewModel += ViewModel_DataChangedInViewModel;

      WordTextBox.Document.Blocks.Clear();

      WordTextBox.TextChanged += WordTextBox_TextChanged;

      WordTextBox.Document.Blocks.Add(new Paragraph(new Run(_viewModel.LineWord)));

      HighlightCharacter(WordTextBox);
    }

    private void ViewModel_DataChangedInViewModel()
    {
      HighlightCharacter(WordTextBox);
    }

    private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

    private void WordTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      var richTextBox = sender as RichTextBox;

      var boxRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);

      var newText = boxRange
        .Text
        .Replace("\r\n", string.Empty)
        .Trim();

      var upperNewText = newText.ToUpper();

      if (!newText.Equals(upperNewText))
      {
        var caretOffset = richTextBox.Document.ContentStart.GetOffsetToPosition(richTextBox.CaretPosition);
        boxRange.Text = upperNewText;
        richTextBox.CaretPosition = richTextBox.Document.ContentStart.GetPositionAtOffset(caretOffset);
      }

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

      if (_viewModel.CharacterPlaceInWord == -1)
      {
        // No point in continuing if the required character is not there
        return;
      }

      var currentPosition = richTextBox_.Document.ContentStart;

      var actualCharactersPassed = 0;

      while (currentPosition != null)
      {
        if (currentPosition.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
        {
          var text = currentPosition.GetTextInRun(LogicalDirection.Forward);

          if (text.Length + actualCharactersPassed > _viewModel.CharacterPlaceInWord)
          {
            var startPosition = currentPosition.GetPositionAtOffset(_viewModel.CharacterPlaceInWord - actualCharactersPassed);
            var endPosition = startPosition.GetPositionAtOffset(1);

            var range = new TextRange(startPosition, endPosition);
            range.ApplyPropertyValue(ForegroundProperty, Brushes.Red);

            richTextBox_.InvalidateVisual();
            break;
          }

          actualCharactersPassed += text.Length;
        }

        currentPosition = currentPosition.GetNextContextPosition(LogicalDirection.Forward);
      }
    }
  }
}
