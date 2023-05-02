using CrosswordCreator.Models;
using CrosswordCreator.Utilities;
using CrosswordCreator.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;

namespace CrosswordCreator.ViewModels
{
  internal class MainViewModel : INotifyPropertyChanged, IDisposable
  {
    private const string TEMP_FOLDER = "CrosswordCreator";
    private const string PREF_FILE_NAME = "preferences.xml";

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool _isCurrentCrosswordModified = false;
    private bool _showNewCrosswordInput = false;
    private string _selectedPath;
    private volatile bool _isSaving = false;
    private volatile bool _isLoading = false;

    public MainViewModel()
    {
      LoadPreviousWorkingDirectory();

      Rows = new ObservableCollection<CrosswordLineViewModel>
      {
        new CrosswordLineViewModel(new CrosswordLine() { LineWord = "VALAMI", SolutionCharacterNumberInLineWord = 2, Clue = "Ez egy clue", PlaceInCrossword = 1 }),
        new CrosswordLineViewModel(new CrosswordLine() { LineWord = "EGYENLŐTLENSÉG", SolutionCharacterNumberInLineWord = 9, Clue = "Ez egy clue", PlaceInCrossword = 2 }),
        new CrosswordLineViewModel(new CrosswordLine() { LineWord = "KANÁL", SolutionCharacterNumberInLineWord = 2, Clue = "Ez egy clue", PlaceInCrossword = 3, IsLastInCrossword = true })
      };

      RecalculateGridMetrics();

      NewCrosswordCommand = new RelayCommand(_ =>
      {
        _showNewCrosswordInput = true;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowNewCrosswordInput)));
      });

      SaveCrosswordCommand = new RelayCommand(_ =>
      {
        if (string.IsNullOrEmpty(_selectedPath))
        {
          // TODO: warning or just pop the thing up?
          return;
        }


      });

      SelectWorkingFolderCommand = new RelayCommand(_ =>
      {
        if (_isSaving || _isLoading)
        {
          return;
        }

        //var dialog = new FolderBrowserDialog();
        //dialog.ShowDialog();

        //if (dialog.Result == DialogResult.Ok)
        //{
        //  _selectedPath = dialog.SelectedPath;
        //}
      }, _ => !_isLoading && !_isSaving);

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
    public ICommand CancelNewCrosswordCommand { get; private set; }
    public ICommand StartNewCrosswordCommand { get; private set; }
    public ICommand EditCommand { get; private set; }

    public bool ShowNewCrosswordInput => _showNewCrosswordInput;

    private void RecalculateGridMetrics()
    {
      var widthLeft = 1;
      var widthRight = 1;

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

    private void SavePreviousWorkingDirectory()
    {
      var appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), TEMP_FOLDER);

      if (!Directory.Exists(appDataFolder))
      {
        Directory.CreateDirectory(appDataFolder);
      }

      File.WriteAllText($"{Path.Combine(appDataFolder, PREF_FILE_NAME)}", _selectedPath);
    }

    private void LoadPreviousWorkingDirectory()
    {
      var preferencesFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), TEMP_FOLDER, PREF_FILE_NAME);

      if (!File.Exists(preferencesFile))
      {
        _selectedPath = string.Empty;
        return;
      }

      //_selectedPath = File.ReadAllLines(preferencesFile)[0];
    }

    public void Dispose()
    {
      SavePreviousWorkingDirectory();

      if (_isCurrentCrosswordModified)
      {

      }
    }
  }
}
