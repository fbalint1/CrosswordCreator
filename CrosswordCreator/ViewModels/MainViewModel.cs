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
        new CrosswordLineViewModel(new CrosswordLine() { LineWord = "VALAMI", SolutionCharacterNumberInLineWord = 3, Clue = "Ez egy clue", PlaceInCrossword = 1 }),
        new CrosswordLineViewModel(new CrosswordLine() { LineWord = "EGYENLŐTLENSÉG", SolutionCharacterNumberInLineWord = 10, Clue = "Ez egy clue", PlaceInCrossword = 2 }),
        new CrosswordLineViewModel(new CrosswordLine() { LineWord = "KANÁL", SolutionCharacterNumberInLineWord = 3, Clue = "Ez egy clue", PlaceInCrossword = 3, IsLastInCrossword = true })
      };

      RecalculateGridMetrics();

      EditCommand = new RelayCommand(row =>
      {
        if (row is CrosswordLineViewModel crosswordLineViewModel)
        {
          var editorViewModel = new CrosswordLineEditorViewModel(crosswordLineViewModel.Word, crosswordLineViewModel.Clue, crosswordLineViewModel.SolutionCharacterNumber);
          var editorView = new CrosswordLineEditorView(editorViewModel);
          editorView.ShowDialog();

          if (editorViewModel.ShouldUpateMainWindow)
          {
            crosswordLineViewModel.Word = editorViewModel.LineWord;
            crosswordLineViewModel.Clue = editorViewModel.Clue;
            crosswordLineViewModel.SolutionCharacterNumber = editorViewModel.CharacterPlaceInWord;

            RecalculateGridMetrics();
          }
        }
      });
    }

    public ObservableCollection<CrosswordLineViewModel> Rows { get; set; }

    public int ControlWidthLeft { get; set; }
    public int ControlWidthRight { get; set; }  

    public ICommand EditCommand { get; private set; }

    private void RecalculateGridMetrics()
    {
      var widthLeft = 1;
      var widthRight = 1;

      foreach (var row in Rows)
      {
        var lengthLeft = row.SolutionCharacterNumber - 1;
        widthLeft = lengthLeft > widthLeft ? lengthLeft : widthLeft;

        var lengthRight = row.Word.Length - row.SolutionCharacterNumber;
        widthRight = lengthRight > widthRight ? lengthRight : widthRight; 
      }

      ControlWidthLeft = widthLeft;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ControlWidthLeft)));
      ControlWidthRight = widthRight;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ControlWidthRight))); 
    }
  }
}
