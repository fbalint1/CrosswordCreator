using CrosswordCreator.Models;
using CrosswordCreator.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace CrosswordCreator.ViewModels
{
  public class CrosswordLineEditorViewModel : INotifyPropertyChanged
  {
    private readonly char _requiredCharacter;
    private readonly string _startingWord;
    private readonly string _startingClue;
    private readonly int _startingCharacterPosition;
    private string _lineWord;
    private string _clue;
    private int _characterPlaceInWord;
    private bool _charChoiceVisibe;
    private bool _changed = false;
    private bool _wasSaved = false;

    [Obsolete("Only for design time default")]
    public CrosswordLineEditorViewModel()
      : this("Egyenlőtlenség".ToUpper(), "Ez egy teszt clue", 10)
    {
    }

    public CrosswordLineEditorViewModel(string word_, string clue_, int characterPlaceInWord_)
    {
      _requiredCharacter = word_[characterPlaceInWord_ - 1];
      _startingWord = word_;
      _startingClue = clue_;
      _startingCharacterPosition = characterPlaceInWord_;
      _lineWord = word_;
      _clue = clue_;
      _characterPlaceInWord = characterPlaceInWord_;
      _charChoiceVisibe = word_.Count(c => c == _requiredCharacter) > 1;

      ResetCommand = new RelayCommand(_ =>
      {
        LineWord = _startingWord;
        Clue = _startingClue;
        _characterPlaceInWord = _startingCharacterPosition;
        _changed = false;
      });
      SaveCommand = new RelayCommand(w =>
      {
        if (w is Window window && _changed)
        {
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
      SelectCharacterCommand = new RelayCommand(number =>
      {
        if (bool.TryParse(number.ToString(), out var ascending))
        {
          if (ascending)
          {
            for (int i = _characterPlaceInWord; i < _lineWord.Length; i++)
            {
              if (_lineWord[i] == _requiredCharacter)
              {
                _characterPlaceInWord = i + 1;
                _changed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveButtonTooltip)));
                break;
              }
            }
          }
          else
          {
            for (int i = _characterPlaceInWord - 2; i >= 0; i--)
            {
              if (_lineWord[i] == _requiredCharacter)
              {
                _characterPlaceInWord = i + 1;
                _changed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveButtonTooltip)));
                break;
              }
            }
          }
        }
      });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand ResetCommand { get; private set; }
    public ICommand SaveCommand { get; private set; }
    public ICommand CancelCommand { get; private set; }
    public ICommand SelectCharacterCommand { get; private set; }

    public char Character => _requiredCharacter;

    public bool ShowCharacterChoice => _lineWord.Count(c => c == _requiredCharacter) > 1;

    public bool CharacterChoiceFirstVisibility => _charChoiceVisibe;
    
    //public FlowDocument Documentt
    //{
    //  get
    //  {
    //    var doc = new FlowDocument();
    //    var paragraph = new Paragraph();
        
    //    var indexOf = _lineWord.IndexOf(_requiredCharacter);

    //    paragraph.Inlines.Add(new Run(_lineWord.Substring(0, indexOf)));
    //    paragraph.Inlines.Add(new Run(_lineWord[indexOf].ToString()) { Foreground = Brushes.Red });
    //    paragraph.Inlines.Add(new Run(_lineWord.Substring(indexOf + 1, _lineWord.Length)));

    //    doc.Blocks.Add(paragraph);
    //    return doc;
    //  }
    //}

    public string LineWord
    {
      get { return _lineWord; }
      set
      {
        _lineWord = value.ToUpper();
        _changed = true;

        if (!_charChoiceVisibe)
        {
          _charChoiceVisibe = true;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CharacterChoiceFirstVisibility)));
        }

        if (_lineWord.Length < _characterPlaceInWord
          || _lineWord[_characterPlaceInWord] != _requiredCharacter)
        {
          _characterPlaceInWord = _lineWord.IndexOf(_requiredCharacter);
        }

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LineWord)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveButtonTooltip)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowCharacterChoice)));
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
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveButtonTooltip)));
      }
    }

    public int CharacterPlaceInWord => _characterPlaceInWord;

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
