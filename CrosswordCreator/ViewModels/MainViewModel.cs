using CrosswordCreator.Models;
using CrosswordCreator.Models.Enums;
using CrosswordCreator.Utilities;
using CrosswordCreator.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Timer = System.Windows.Forms.Timer;

namespace CrosswordCreator.ViewModels
{
  internal class MainViewModel : INotifyPropertyChanged, IDisposable
  {
    private const string TEMP_FOLDER = "CrosswordCreator";
    private const string PREF_FILE_NAME = "preferences.xml";
    private const string LOG_FILE_NAME = "log.txt";

    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly UISynchronizationContext _uISynchronizationContext;
    private readonly Timer _statusClearTimer;

    private Crossword _currentCrossword;
    private bool _isCurrentCrosswordModified = false;
    private bool _showNewCrosswordInput = false;
    private bool _showSolutions = true;
    private string _newCrosswordText = string.Empty;
    private string _selectedPath;
    private volatile bool _isSaving = false;
    private volatile bool _isLoading = false;
    private volatile StatusEnum _statusEnum = StatusEnum.None;

    public MainViewModel()
    {
      _uISynchronizationContext = UISynchronizationContext.Instance;
      _statusClearTimer = new Timer()
      {
        Interval = 5000,
      };
      _statusClearTimer.Tick += ClearStatusProperty;

      Rows = new ObservableCollection<CrosswordLineViewModel>();

      LoadPreviousWorkingDirectory();

#if DEBUG
      //var testCrossword = new Crossword()
      //{
      //  Goal = "LEN",
      //  Lines = new CrosswordLine[]
      //  {
      //    new CrosswordLine() { LineWord = "VALAMI", SolutionCharacterNumberInLineWord = 2, Clue = "Ez egy clue", PlaceInCrossword = 1 },
      //    new CrosswordLine() { LineWord = "EGYENLŐTLENSÉG", SolutionCharacterNumberInLineWord = 9, Clue = "Ez egy clue", PlaceInCrossword = 2 },
      //    new CrosswordLine() { LineWord = "KANÁL", SolutionCharacterNumberInLineWord = 2, Clue = "Ez egy clue", PlaceInCrossword = 3, IsLastInCrossword = true }
      //  }
      //};

      //PopulateDataFromCrossword(testCrossword);
#endif

      NewCrosswordCommand = new RelayCommand(_ =>
      {
        _showNewCrosswordInput = true;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowNewCrosswordInput)));
      });

      SaveCrosswordCommand = new RelayCommand(_ =>
      {
        if (string.IsNullOrEmpty(_selectedPath))
        {
          ShowDirectorySelection();
        }

        Task.Run(SaveCurrentCrossword);
      });

      SelectWorkingFolderCommand = new RelayCommand(_ =>
      {
        if (_isSaving || _isLoading)
        {
          return;
        }

        ShowDirectorySelection();
      }, _ => !_isLoading && !_isSaving);

      OpenCrosswordCommmand = new RelayCommand(_ =>
      {
        var dialog = new OpenFileDialog();
        dialog.Filter = "CSV files (*.xml)|*.xml";
        if (!string.IsNullOrEmpty(_selectedPath))
        {
          dialog.InitialDirectory = _selectedPath;
        }
        var result = dialog.ShowDialog();
        if (dialog.FileName != null && result == DialogResult.OK)
        {
          try
          {
            SetCurrentCrossword(CrosswordSerializer.GetCrossword(dialog.FileName));
          }
          catch (Exception)
          {
            StatusEnum = StatusEnum.LoadFailed;
          }
        }
      });

      StartNewCrosswordCommand = new RelayCommand(_ =>
      {
        if (_isCurrentCrosswordModified)
        {
          var promptResult = PromptUserForSave();

          switch (promptResult)
          {
            case PromptResultEnum.Cancel:
              return;
            case PromptResultEnum.Continue:
              Task.Run(SaveCurrentCrossword)
                .ContinueWith(t =>
                {
                  StatusEnum = StatusEnum.Loading;

                  _uISynchronizationContext.Run(() =>
                  {
                    PopulateDataFromCrossword(new Crossword(_newCrosswordText));

                    ResetNewCrosswordMetrics();
                  });
                });
              break;
            case PromptResultEnum.Skip:
              StatusEnum = StatusEnum.Loading;

              PopulateDataFromCrossword(new Crossword(_newCrosswordText));

              ResetNewCrosswordMetrics();
              break;
            default:
              throw new InvalidOperationException("Unmapped enum state");
          }
        }
        else
        {
          PopulateDataFromCrossword(new Crossword(_newCrosswordText));

          ResetNewCrosswordMetrics();
        }
      });

      CancelNewCrosswordCommand = new RelayCommand(_ =>
      {
        _showNewCrosswordInput = false;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowNewCrosswordInput)));
      });

      EditCommand = new RelayCommand(row =>
      {
        if (row is CrosswordLineViewModel crosswordLineViewModel)
        {
          var editorViewModel = new CrosswordLineEditorViewModel(crosswordLineViewModel.Word, crosswordLineViewModel.Clue, crosswordLineViewModel.SolutionCharacterNumber);
          var editorView = new CrosswordLineEditorView(editorViewModel);
          editorView.ShowDialog();

          if (editorViewModel.ShouldUpateMainWindow)
          {
            _isCurrentCrosswordModified = true;

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

    public ICommand NewCrosswordCommand { get; private set; }
    public ICommand SaveCrosswordCommand { get; private set; }
    public ICommand SelectWorkingFolderCommand { get; private set; }
    public ICommand OpenCrosswordCommmand { get; private set; }
    public ICommand CancelNewCrosswordCommand { get; private set; }
    public ICommand StartNewCrosswordCommand { get; private set; }
    public ICommand EditCommand { get; private set; }

    public bool ShowNewCrosswordInput => _showNewCrosswordInput;

    public bool ShowSolutions
    {
      get { return _showSolutions; }
      set
      {
        if (_showSolutions != value)
        {
          _showSolutions = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowSolutions)));
        }
      }
    }

    public StatusEnum StatusEnum
    {
      get { return _statusEnum; }
      set
      {
        if (_statusEnum != value)
        {
          _statusEnum = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusEnum)));

          if (_statusEnum != StatusEnum.None)
          {
            _uISynchronizationContext.Run(() =>
            {
              _statusClearTimer.Stop();
              _statusClearTimer.Start();
            });
          }
        }
      }
    }

    public string NewCrosswordText
    {
      get { return _newCrosswordText; }
      set
      {
        _newCrosswordText = value.ToUpper();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewCrosswordText)));
      }
    }

    private void PopulateDataFromCrossword(Crossword crossword_)
    {
      Rows.Clear();

      _currentCrossword = crossword_;

      if (crossword_.Lines.Length == 0)
      {
        for (int i = 0; i < crossword_.Goal.Length; i++)
        {
          Rows.Add(new CrosswordLineViewModel(new CrosswordLine()
          {
            LineWord = crossword_.Goal[i].ToString(),
            SolutionCharacterNumberInLineWord = 0,
            Clue = string.Empty,
            PlaceInCrossword = i + 1,
            IsLastInCrossword = i == crossword_.Goal.Length - 1,
          }));
        }
      }
      else
      {
        foreach (var line in crossword_.Lines)
        {
          Rows.Add(new CrosswordLineViewModel(line));
        }
      }

      RecalculateGridMetrics();

      StatusEnum = StatusEnum.LoadComplete;
    }

    private void RecalculateGridMetrics()
    {
      var widthLeft = 4;
      var widthRight = 4;

      foreach (var row in Rows)
      {
        var lengthLeft = row.SolutionCharacterNumber;
        widthLeft = lengthLeft > widthLeft ? lengthLeft : widthLeft;

        var lengthRight = row.Word.Length - row.SolutionCharacterNumber - 1;
        widthRight = lengthRight > widthRight ? lengthRight : widthRight;
      }

      ControlWidthLeft = widthLeft;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ControlWidthLeft)));
      ControlWidthRight = widthRight;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ControlWidthRight)));
    }

    private void ResetNewCrosswordMetrics()
    {
      _showNewCrosswordInput = false;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowNewCrosswordInput)));

      NewCrosswordText = string.Empty;
    }

    private void ShowDirectorySelection()
    {
      var dialog = new FolderBrowserDialog();

      if (dialog.ShowDialog() == DialogResult.OK)
      {
        _selectedPath = dialog.SelectedPath;
      }
    }

    private void SaveWorkingDirectory()
    {
      var appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), TEMP_FOLDER);

      if (!Directory.Exists(appDataFolder))
      {
        Directory.CreateDirectory(appDataFolder);
      }

      File.WriteAllLines($"{Path.Combine(appDataFolder, PREF_FILE_NAME)}", new[] { _selectedPath, _currentCrossword?.Goal ?? string.Empty });
    }

    private void LoadPreviousWorkingDirectory()
    {
      var preferencesFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), TEMP_FOLDER, PREF_FILE_NAME);

      if (!File.Exists(preferencesFile))
      {
        _selectedPath = string.Empty;
        return;
      }

      var preferencesLines = File.ReadAllLines(preferencesFile);

      if (preferencesLines.Length > 0)
      {
        _selectedPath = preferencesLines[0];
      }

      if (preferencesLines.Length > 1 && preferencesLines[1] != string.Empty)
      {
        var fileName = Path.Combine(_selectedPath, preferencesLines[1] + ".xml");

        if (File.Exists(fileName))
        {
          PopulateDataFromCrossword(CrosswordSerializer.GetCrossword(fileName));
        }
      }
    }

    public void SaveCurrentCrossword()
    {
      _isSaving = true;
      StatusEnum = StatusEnum.Saving;

      try
      {
        CrosswordSerializer.PersistCrossword(_currentCrossword, _selectedPath);
        StatusEnum = StatusEnum.SaveComplete;
      }
      catch (Exception)
      {
        StatusEnum = StatusEnum.SaveFailed;
      }
      finally
      {
        _isSaving = false;
      }
    }

    public PromptResultEnum PromptUserForSave()
    {
      if (!_isCurrentCrosswordModified)
      {
        return PromptResultEnum.Skip;
      }

      var dialog = new UserInputWindow("Mentés?", "A jelenlegi keresztrejtvény nincs mentve. Elmentsük?", "Igen", "Nem");
      dialog.ShowDialog();

      if (dialog.WasCancelled)
      {
        // Clicked X, cancel
        return PromptResultEnum.Cancel;
      }
      else if (dialog.WasLeftClicked)
      {
        return PromptResultEnum.Continue;
      }

      // Clicked no, don't save
      return PromptResultEnum.Skip;
    }

    private void ClearStatusProperty(object? _, EventArgs __)
    {
      StatusEnum = StatusEnum.None;

      _statusClearTimer.Stop();
    }

    public void SetCurrentCrossword(Crossword crossword_)
    {
      var promptResult = PromptUserForSave();

      switch (promptResult)
      {
        case PromptResultEnum.Cancel:
          break;
        case PromptResultEnum.Continue:
          Task.Run(SaveCurrentCrossword)
            .ContinueWith(t =>
            {
              StatusEnum = StatusEnum.Loading;

              _uISynchronizationContext.Run(() => PopulateDataFromCrossword(crossword_));
            });
          break;
        case PromptResultEnum.Skip:
          StatusEnum = StatusEnum.Loading;

          PopulateDataFromCrossword(crossword_);
          break;
        default:
          throw new InvalidOperationException("Unmapped enum state");
      }
    }

    public void Dispose()
    {
      SaveWorkingDirectory();

      _statusClearTimer?.Dispose();
    }
  }
}
