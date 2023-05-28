using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace CrosswordCreator.Utilities
{
  public static class ClipboardUtil
  {
    private const int MAX_RETRY = 5;
    private const int DELAY_BEFORE_RETRY = 25;

    public static bool WriteToClipboard(object data_)
    {
      var counter = 0;
      var success = false;

      while (!success && counter++ < MAX_RETRY)
      {
        try
        {
          Clipboard.SetDataObject(data_);
          success = true;
        }
        catch (COMException)
        {
          Thread.Sleep(DELAY_BEFORE_RETRY);
        }
        catch (ExternalException)
        {
          Thread.Sleep(DELAY_BEFORE_RETRY);
        }
      }

      return success;
    }
  }
}
