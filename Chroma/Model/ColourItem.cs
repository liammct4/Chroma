using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Chroma.Model
{
	public class ColourItem : INotifyPropertyChanged
	{
		internal static int ColourCount = 1;
		public Color Colour
		{
			get => _colour;
			set
			{
				_colour = value;
				OnPropertyChanged(nameof(Colour));
			}
		}
		private Color _colour;
		public string Name
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}
		private string _name;

		public ColourItem()
		{
			Colour = Color.FromRgb(255, 255, 255);
			Name = $"New Colour {ColourCount++}";
		}

		public ColourItem(Color colour, string name = "New Colour")
		{
			Colour = colour;
			Name = name;
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));
	}
}
