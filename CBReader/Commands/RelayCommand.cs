using System.Windows.Input;

namespace CBReader.Commands;
public class RelayCommand : ICommand
{
    //@see https://www.youtube.com/watch?v=4v8PobcZpqM, about 10 mintues time stamp. Self reference, learning MVVM.

    public event EventHandler? CanExecuteChanged;
    private Action<object> _Execute { get; set; }     // action takes object as an argument, returns void
    private Predicate<object> _CanExecute { get; set; }   // predicate returns a bool, also takes object as an arg
    
    // whenever we create an instance of the class, we execute CanExecute and Execute. We pass in TWO METHODS as an argument.
    public RelayCommand(Action<object> ExecuteMethod, Predicate<object> CanExecuteMethod)
    {
        // we store the ExecuteMethod and CanExecuteMethods in variables: _exe... _can...
        _Execute = ExecuteMethod;
        _CanExecute = CanExecuteMethod;
    }

    // these two methods are execute when we call the command from the VIEW.
    public bool CanExecute(object? parameter)
    {
        return _CanExecute(parameter);
    }

    public void Execute(object? parameter)
    {
        _Execute(parameter);
    }
}

