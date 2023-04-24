using CrosswordCreator.Models;
using CrosswordCreator.Utilities;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace CrosswordCreator.ViewModels
{
  public class CrosswordLineEditorViewModel : INotifyPropertyChanged
  {
    private readonly CrosswordLine _crosswordLine;

    private string _lineWord;
    private string _clue;
    private bool _changed = false;
    private bool _wasSaved = false;

    // Only for design time default
    [Obsolete]
    public CrosswordLineEditorViewModel()
      : this(new CrosswordLine()
      {
        LineWord = "Egyenlőtlenség".ToUpper(),
        Clue = "Ez egy teszt clue",
        SolutionCharacterNumberInLineWord = 10,
        PlaceInCrossword = 1
      })
    {
    }

    public CrosswordLineEditorViewModel(CrosswordLine crosswordLine_)
    {
      _crosswordLine = crosswordLine_;

      _lineWord = crosswordLine_.LineWord;
      _clue = crosswordLine_.Clue;

      ResetCommand = new RelayCommand(_ =>
      {
        LineWord = _crosswordLine.LineWord;
        Clue = _crosswordLine.Clue;
        _changed = false;
      });
      SaveCommand = new RelayCommand(w =>
      {
        if (w is Window window && _changed)
        {
          _crosswordLine.SolutionCharacterNumberInLineWord = _lineWord.IndexOf(Character) + 1;
          _crosswordLine.LineWord = _lineWord;
          _crosswordLine.Clue = Clue;
          _wasSaved = true;
          window.Close();
        }
      }, _ => _changed && IsValid);
      CancelCommand = new RelayCommand(w =>
      {
        if (w is Window window)
        {
          window.Close();
        }
      });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand ResetCommand { get; private set; }
    public ICommand SaveCommand { get; private set; }
    public ICommand CancelCommand { get; private set; }

    public char Character => _crosswordLine.LineWord[_crosswordLine.SolutionCharacterNumberInLineWord - 1];

    public string LineWord
    {
      get { return _lineWord; }
      set
      {
        _lineWord = value.ToUpper();
        _changed = true;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LineWord)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveButtonTooltip)));
      }
    }
    public string Clue
    {
      get { return _clue; }
      set
      {
        _clue = value;
        _changed = true;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Clue)));
      }
    }

    public string SaveButtonTooltip
    {
      get
      {
        if (!_changed)
        {
          return "Nem történt változás!";
        }
        else if (!IsValid)
        {
          return "Szó nem tartalmazza a megoldás betűjét!";
        }

        return null;
      }
    }
    
    public bool ShouldUpateMainWindow => _wasSaved;

    private bool IsValid => _lineWord.Contains(Character);
  }
}
