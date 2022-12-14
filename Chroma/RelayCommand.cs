using System;
using System.Windows.Input;

namespace Chroma.ViewModel
{
	public class RelayCommand : ICommand
	{
		private Action<object> execute;
		private Func<object, bool> canExecute;
		public event EventHandler? CanExecuteChanged
		{
			add
			{
				_canExecuteChanged += value;
				CommandManager.RequerySuggested += value;
			}
			remove
			{
				_canExecuteChanged -= value;
				CommandManager.RequerySuggested -= value;
			}
		}
		private EventHandler? _canExecuteChanged;

		public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
		{
			this.execute = execute;
			this.canExecute = canExecute;
		}

		public bool CanExecute(object? parameter) => canExecute(parameter);

		public void Execute(object? parameter) => execute(parameter);

		public static void RaiseCanExecuteChanged()
		{
			CommandManager.InvalidateRequerySuggested();
		}
	}
}