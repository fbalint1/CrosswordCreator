using CrosswordCreator.Models;
using System.ComponentModel;

namespace CrosswordCreator.ViewModels
{
  internal class CrosswordLineViewModel : INotifyPropertyChanged
  {
    private readonly CrosswordLine _lineItem;

    public event PropertyChangedEventHandler? PropertyChanged;

    public CrosswordLineViewModel(CrosswordLine lineItem_)
    {
      _lineItem = lineItem_;
    }

    public string Word
    {
      get { return _lineItem.LineWord; }
      set
      {
        _lineItem.LineWord = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Word)));
      }
    }

    public string Clue
    {
      get { return _lineItem.Clue; }
      set
      {
        _lineItem.Clue = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Clue)));
      }
    }

    public int SolutionCharacterNumber {
      get { return _lineItem.SolutionCharacterNumberInLineWord; }
      set
      {
        _lineItem.SolutionCharacterNumberInLineWord = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SolutionCharacterNumber)));
      }
    }

    public int PlaceInCrossword => _lineItem.PlaceInCrossword;

    public bool IsFirstInCrossword => _lineItem.PlaceInCrossword == 1;

    public bool IsLastInCrossword => _lineItem.IsLastInCrossword;
  }
}
