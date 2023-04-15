using CrosswordCreator.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrosswordCreator.ViewModels
{
  internal class CrosswordLineViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    public CrosswordLineViewModel(CrosswordLine lineItem)
    {
      LineItem = lineItem;
    }

    public CrosswordLine LineItem { get; }

    public int ControlWidth { get; set; }

  }
}
