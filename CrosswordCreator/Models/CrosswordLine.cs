using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrosswordCreator.Models
{
  internal class CrosswordLine
  {
    public string LineWord { get; set; }
    public char CharacterOfSolution { get; set; }
    public int SolutionCharacterNumberInLineWord { get; set; }
    public int PlaceInCrossword { get; set; }
    public string Clue { get; set; }
  }
}
