using CrosswordCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrosswordCreator.ViewModels
{
  internal class CrosswordLineEditorViewModel
  {
    private readonly CrosswordLine _crosswordLine;

    private bool _changed = false;

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

      LineWord = crosswordLine_.LineWord;
      Clue = crosswordLine_.Clue;
    }

    public char Character => _crosswordLine.LineWord[_crosswordLine.SolutionCharacterNumberInLineWord - 1];

    public string LineWord { get; set; }
    public string Clue { get; set; }
  }
}
