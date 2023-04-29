using CrosswordCreator.Models;
using CrosswordCreator.Utilities;
using CrosswordCreator.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace CrosswordCreator.ViewModels
{
  internal class MainViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    public MainViewModel()
    {
      Rows = new ObservableCollection<CrosswordLineViewModel>
      {
        new CrosswordLineViewModel(new CrosswordLine() { LineWord = "VALAMI", SolutionCharacterNumberInLineWord = 3, Clue = "Ez egy clue", PlaceInCrossword = 1 }) { ControlWidthLeft = 9, ControlWidthRight = 4 },
        new CrosswordLineViewModel(new CrosswordLine() { LineWord = "EGYENLŐTLENSÉG", SolutionCharacterNumberInLineWord = 10, Clue = "Ez egy clue", PlaceInCrossword = 2 }) { ControlWidthLeft = 9, ControlWidthRight = 4 },
        new CrosswordLineViewModel(new CrosswordLine() { LineWord = "KANÁL", SolutionCharacterNumberInLineWord = 3, Clue = "Ez egy clue", PlaceInCrossword = 3, IsLastInCrossword = true }) { ControlWidthLeft = 9, ControlWidthRight = 4 }
      };

      EditCommand = new RelayCommand(row =>
      {
        if (row is CrosswordLineViewModel crosswordLineViewModel)
        {
          var editorViewModel = new CrosswordLineEditorViewModel(crosswordLineViewModel.LineItem);
          var editorView = new CrosswordLineEditorView(editorViewModel);
          editorView.ShowDialog();

          if (editorViewModel.ShouldUpateMainWindow)
          {
            RecalculateGridMetrics();
          }
        }
      });
    }

    public ObservableCollection<CrosswordLineViewModel> Rows { get; set; }

    public ICommand EditCommand { get; private set; }

    private void RecalculateGridMetrics()
    {
      var widthLeft = 1;
      var widthRight = 1;

      foreach (var row in Rows)
      {
        var lengthLeft = row.LineItem.SolutionCharacterNumberInLineWord - 1;
        widthLeft = lengthLeft > widthLeft ? lengthLeft : widthLeft;

        var lengthRight = row.LineItem.LineWord.Length - row.LineItem.SolutionCharacterNumberInLineWord;
        widthRight = lengthRight > widthRight ? lengthRight : widthRight; 
      }

      foreach (var row in Rows)
      {
        row.ControlWidthLeft = widthLeft;
        row.ControlWidthRight = widthRight;
        row.NotifyUpdates();
      }
    }
  }
}
