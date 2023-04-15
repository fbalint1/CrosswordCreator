using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrosswordCreator.Models
{
  internal class Crossword
  {
    public string Goal { get; set; }

    public CrosswordLine[] Lines { get; set; }
  }
}
