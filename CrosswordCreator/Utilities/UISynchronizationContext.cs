using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrosswordCreator.Utilities
{
  public class UISynchronizationContext
  {
    private static UISynchronizationContext _instance;
    public static UISynchronizationContext Instance => _instance ?? CreateInstance();

    private readonly SynchronizationContext _synchronizationContext;
    private readonly TaskScheduler _taskScheduler;
    private readonly int _id;

    // Must be called on UI thread
    private UISynchronizationContext()
    {
      _synchronizationContext = SynchronizationContext.Current;
      _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
      _id = Thread.CurrentThread.ManagedThreadId;
    }

    private bool IsOnUIContext => _id == Thread.CurrentThread.ManagedThreadId;

    public void Run(Action action_)
    {
      if (IsOnUIContext)
      {
        action_();
      }
      else
      {
        _synchronizationContext.Send(_ => action_(), null);
      }
    }

    private static UISynchronizationContext CreateInstance()
    {
      _instance = new UISynchronizationContext();
      return _instance;
    }
  }
}
