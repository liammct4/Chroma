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
		private static Random colourRandom = new();

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
			Colour = GenerateRandomColor();
			Name = $"New Colour {ColourCount++}";
		}

		public ColourItem(Color colour, string name = "New Colour")
		{
			Colour = colour;
			Name = name;
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));

		public static Color GenerateRandomColor()
		{
			byte r = (byte)colourRandom.Next(0, 255);
			byte g = (byte)colourRandom.Next(0, 255);
			byte b = (byte)colourRandom.Next(0, 255);

			return Color.FromRgb(r, g, b);
		}
	}
}
