using CrosswordCreator.Utilities;
using System.Windows;
using System.Windows.Input;

namespace CrosswordCreator.Views
{
  /// <summary>
  /// Interaction logic for UserInputWindow.xaml
  /// </summary>
  public partial class UserInputWindow : Window
  {
    private bool _wasCancelled = false;
    private bool _wasLeftClicked = false;

    public UserInputWindow()
    {
      InitializeComponent();
    }

    public UserInputWindow(string title_, string content_, string leftOption_, string rightOption_)
      : this()
    {
      WindowTitle = title_;
      WindowContent = content_;
      LeftOption = leftOption_;
      RightOption = rightOption_;

      LeftCommand = new RelayCommand(_ =>
      {
        _wasLeftClicked = true;
        Close();
      });
      RightCommand = new RelayCommand(_ =>
      {
        Close();
      });
      CloseCommand = new RelayCommand(_ =>
      {
        _wasCancelled = true;
        Close();
      });

      DataContext = this;
    }

    public ICommand LeftCommand { get; private set; }
    public ICommand RightCommand { get; private set; }
    public ICommand CloseCommand { get; private set; }

    public string WindowTitle { get; private set; }
    public string WindowContent { get; private set; }
    public string LeftOption { get; private set; }
    public string RightOption { get; private set; }

    public bool WasCancelled => _wasCancelled;
    public bool WasLeftClicked => _wasLeftClicked;
  }
}
