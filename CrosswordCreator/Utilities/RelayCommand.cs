using System;
using System.Windows.Input;

namespace CrosswordCreator.Utilities
{
  public class RelayCommand : ICommand
  {
    private event EventHandler CanExecuteChangedInternal;

    private Action<object> _actionToExecute;
    private Predicate<object> _canExecute;

    public event EventHandler CanExecuteChanged
    {
      add
      {
        CommandManager.RequerySuggested += value;
        CanExecuteChangedInternal += value;
      }
      remove
      {
        CommandManager.RequerySuggested -= value;
        CanExecuteChangedInternal -= value;
      }
    }

    public RelayCommand(Action<object> actionToExecute_, Predicate<object> canExecuteFunction_)
    {
      _actionToExecute = actionToExecute_ ??
          throw new ArgumentException("Action to execute not defined!");

      _canExecute = canExecuteFunction_ ??
          throw new ArgumentException("Can execute function not defined!");
    }

    public RelayCommand(Action<object> actionToExecute_)
      : this(actionToExecute_, t => true)
    {
    }

    public bool CanExecute(object parameter)
    {
      return _canExecute != null
          && _canExecute(parameter);
    }

    public void Execute(object parameter)
    {
      _actionToExecute?.Invoke(parameter);
    }

    public void OnCanExecuteChanged()
    {
      EventHandler handler = CanExecuteChangedInternal;

      handler?.Invoke(this, EventArgs.Empty);
    }

    public void Destroy()
    {
      _canExecute = t => false;
      _actionToExecute = t => { return; };
    }
  }
}
