using CrosswordCreator.Utilities;
using System.ComponentModel;

namespace CrosswordCreator.Models.Enums
{
  internal enum StatusEnum
  {
    None,

    [Description("Mentés sikeres")]
    [Icon("Images/green_tick.png")]
    SaveComplete,

    [Description("Mentés sikertelen")]
    [Icon("Images/red_x.png")]
    SaveFailed,

    [Description("Mentés...")]
    [Icon("Images/hourglass.png")]
    Saving,

    [Description("Betöltés sikeres")]
    [Icon("Images/green_tick.png")]
    LoadComplete,

    [Description("Betöltés sikertelen")]
    [Icon("Images/red_x.png")]
    LoadFailed,

    [Description("Betöltés...")]
    [Icon("Images/hourglass.png")]
    Loading,
  }
}
