using System.ComponentModel;

namespace Chroma.ViewModel
{
	/// <summary>
	/// Base class for view model classes, cannot be instantiated.
	/// </summary>
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		/// <summary>
		/// Used to notify UI of binded property change. Call when binded property has been changed.
		/// </summary>
		/// <param name="propertyName">Name of property</param>
		public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));
	}
}
