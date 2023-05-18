using CrosswordCreator.Utilities;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace CrosswordCreator.ViewModels
{
  public delegate void DataChangedInViewModelHandler();

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
    private bool _characterChoiceFirstVisibility;
    private bool _changed = false;
    private bool _wasSaved = false;

    [Obsolete("Only for design time default")]
    public CrosswordLineEditorViewModel()
      : this("Egyenlőtlenség".ToUpper(), "Ez egy teszt clue", 9)
    {
    }

    public CrosswordLineEditorViewModel(string word_, string clue_, int characterPlaceInWord_)
    {
      _requiredCharacter = word_[characterPlaceInWord_];
      _startingWord = word_;
      _startingClue = clue_;
      _startingCharacterPosition = characterPlaceInWord_;
      _lineWord = word_;
      _clue = clue_;
      _characterPlaceInWord = characterPlaceInWord_;
      _charChoiceVisibe = word_.Count(c => c == _requiredCharacter) > 1;
      _characterChoiceFirstVisibility = _charChoiceVisibe;

      ResetCommand = new RelayCommand(_ =>
      {
        _changed = false;
        _characterPlaceInWord = _startingCharacterPosition;
        LineWord = _startingWord;
        Clue = _startingClue;
        ResetRequested?.Invoke();
      });
      SaveCommand = new RelayCommand(w =>
      {
        if (w is Window window && _changed)
        {
          _wasSaved = true;
          window.Close();
        }
      }, _ => IsChanged && IsValid);
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
            for (int i = _characterPlaceInWord + 1; i < _lineWord.Length; i++)
            {
              if (_lineWord[i] == _requiredCharacter)
              {
                _characterPlaceInWord = i;
                _changed = true;
                break;
              }
            }
          }
          else
          {
            for (int i = _characterPlaceInWord - 1; i >= 0; i--)
            {
              if (_lineWord[i] == _requiredCharacter)
              {
                _characterPlaceInWord = i;
                _changed = true;
                break;
              }
            }
          }

          if (_changed)
          {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChanged)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveButtonTooltip)));
            DataChangedInViewModel?.Invoke();
          }
        }
      });
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public event DataChangedInViewModelHandler? DataChangedInViewModel;
    public event DataChangedInViewModelHandler? ResetRequested;

    public ICommand ResetCommand { get; private set; }
    public ICommand SaveCommand { get; private set; }
    public ICommand CancelCommand { get; private set; }
    public ICommand SelectCharacterCommand { get; private set; }

    public char Character => _requiredCharacter;

    public bool CharacterChoiceFirstVisibility => _characterChoiceFirstVisibility;

    public bool CharacterChoiceVisible
    {
      get { return _charChoiceVisibe; }
      set
      {
        if (_charChoiceVisibe != value)
        {
          _charChoiceVisibe = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CharacterChoiceVisible)));
        }
      }
    }

    public string LineWord
    {
      get { return _lineWord; }
      set
      {
        var upperValue = value.ToUpper();
        if (upperValue.Equals(_lineWord))
        {
          return;
        }

        _lineWord = upperValue;
        _changed = _changed || !_lineWord.Equals(_startingWord);

        CharacterChoiceVisible = _lineWord.Count(c => _requiredCharacter == c) > 1;
        if (!_characterChoiceFirstVisibility && _charChoiceVisibe)
        {
          _characterChoiceFirstVisibility = true;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CharacterChoiceFirstVisibility)));
        }

        if (_characterPlaceInWord >= 0
          && _lineWord.Length > _characterPlaceInWord)
        {
          if (_lineWord[_characterPlaceInWord] != _requiredCharacter)
          {
            if (_characterPlaceInWord != 0
              && _lineWord.Length > _characterPlaceInWord
              && _lineWord[_characterPlaceInWord - 1] == _requiredCharacter)
            {
              _characterPlaceInWord--;
            }
            else if (_characterPlaceInWord + 1 < _lineWord.Length
              && _lineWord[_characterPlaceInWord + 1] == _requiredCharacter)
            {
              _characterPlaceInWord++;
            }
            else
            {
              _characterPlaceInWord = _lineWord.IndexOf(_requiredCharacter);
            }
          }
        }
        else
        {
          _characterPlaceInWord = _lineWord.IndexOf(_requiredCharacter);
        }

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LineWord)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChanged)));
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
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChanged)));
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

    private bool IsChanged => !_startingWord.Equals(_lineWord) || _changed;
    private bool IsValid => _lineWord.Contains(Character);
  }
}
