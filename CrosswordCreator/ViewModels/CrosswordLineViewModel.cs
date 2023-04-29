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

    public int ControlWidthLeft { get; set; }
    public int ControlWidthRight { get; set; }

    public string Clue => LineItem.Clue;

    public void NotifyUpdates()
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LineItem.LineWord)));
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Clue)));
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ControlWidthLeft)));
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ControlWidthRight)));
    }
  }
}
