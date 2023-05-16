using System;

namespace CrosswordCreator.Models
{
  [Serializable]
  public class Crossword
  {
    public string Goal { get; set; }

    public CrosswordLine[] Lines { get; set; }

    public Crossword()
    {
    }

    public Crossword(string goal_)
    {
      Goal = goal_;
      Lines = new CrosswordLine[0];
    }
  }
}
