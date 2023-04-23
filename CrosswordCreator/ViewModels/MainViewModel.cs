using CrosswordCreator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    public ObservableCollection<CrosswordLineViewModel> Rows { get; set; }
  }
}
