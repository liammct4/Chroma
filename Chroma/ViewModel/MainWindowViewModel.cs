using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Chroma.ViewModel
{
	/// <summary>
	/// View model for a <see cref="MainWindow"/>.
	/// </summary>
	public class MainWindowViewModel : ViewModelBase
	{
		public Color CurrentColour
		{
			get => _currentColour;
			set
			{
				_currentColour = value;
				OnPropertyChanged(nameof(CurrentColour));
			}
		}
		private Color _currentColour;
		public int R
		{
			get => CurrentColour.R;
			set => CurrentColour = Color.FromRgb((byte)value, CurrentColour.G, CurrentColour.B);
		}
		public int G
		{
			get => CurrentColour.G;
			set => CurrentColour = Color.FromRgb(CurrentColour.R, (byte)value, CurrentColour.B);
		}
		public int B
		{
			get => CurrentColour.B;
			set => CurrentColour = Color.FromRgb(CurrentColour.R, CurrentColour.G, (byte)value);
		}
	}
}
