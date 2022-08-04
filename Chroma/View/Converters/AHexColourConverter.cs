using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Chroma.View.Converters
{
	/// <summary>
	/// Converts a <see cref="Color"/> to a hex string in the format of: "#RRGGBB".
	/// </summary>
	public class AHexColourConverter : IValueConverter
	{
		/// <summary>
		/// Converts a <see cref="Color"/> object to a hex <see cref="string"/>.
		/// </summary>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not Color)
			{
				return DependencyProperty.UnsetValue;
			}

			Color colour = (Color)value;

			return $"#{colour.A:x2}{colour.R:x2}{colour.G:x2}{colour.B:x2}";
		}

		/// <summary>
		/// Converts an RGB hex <see cref="string"/> to <see cref="Color"/>.
		/// </summary>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not string)
			{
				return DependencyProperty.UnsetValue;
			}

			string rawHex = (string)value;

			// Remove unwanted characters leaving only data remaining.
			string filtered = rawHex.Trim().Replace("#", "");

			// A valid hex should only have 2 or 8 characters.
			if (filtered.Length == 2)
			{
				if (!int.TryParse(filtered[0..2], NumberStyles.HexNumber, culture, out int hexNumber))
				{
					return DependencyProperty.UnsetValue;
				}

				return Color.FromArgb((byte)hexNumber, (byte)hexNumber, (byte)hexNumber, (byte)hexNumber);
			}
			else if (filtered.Length == 8)
			{
				List<byte> channels = new();

				for (int i = 0; i < filtered.Length; i += 2)
				{
					string hex = filtered[i..(i + 2)];

					if (!int.TryParse(hex, NumberStyles.HexNumber, culture, out int hexNumber))
					{
						return DependencyProperty.UnsetValue;
					}

					hexNumber = hexNumber > 255 ? 255 : hexNumber;
					hexNumber = hexNumber < 0 ? 0 : hexNumber;

					channels.Add((byte)hexNumber);
				}

				return Color.FromArgb(channels[0], channels[1], channels[2], channels[3]);
			}
			else
			{
				return DependencyProperty.UnsetValue;
			}
		}
	}
}
