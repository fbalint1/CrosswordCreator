using System;

namespace CrosswordCreator.Models
{
  [Serializable]
  public class CrosswordLine
  {
    public string LineWord { get; set; }
    public int SolutionCharacterNumberInLineWord { get; set; }
    public int PlaceInCrossword { get; set; }
    public string Clue { get; set; }
    public bool IsLastInCrossword { get; set; }
  }
}
