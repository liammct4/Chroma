using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Chroma.View.Converters
{
	/// <summary>
	/// Converts an RGB colour object to a string and a string to an RGB colour object.
	/// </summary>
	public class RGBColourConverter : IValueConverter
	{
		/// <summary>
		/// Used for separating colour channels.
		/// </summary>
		internal static Regex RGBStringFormat = new Regex(@"[\d]+", RegexOptions.Compiled);

		/// <summary>
		/// Converts a <see cref="Color"/> object to a string representation in the format: "{R}, {G}, {B}"
		/// </summary>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not Color)
			{
				return DependencyProperty.UnsetValue;
			}

			Color colour = (Color)value;
			return $"{colour.R}, {colour.G}, {colour.B}";
		}

		/// <summary>
		/// Converts a <see cref="string"/> representing an RGB colour to a <see cref="Color"/>.
		/// </summary>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not string)
			{
				return DependencyProperty.UnsetValue;
			}

			string colour = (string)value;

			// Isolates each number in the string to hopefully provide three values representing the colour channels.
			MatchCollection channelMatches = RGBStringFormat.Matches(colour);
			byte[] channels = channelMatches.Select(x => (byte)int.Parse(x.Value)).ToArray();

			// Do not throw an exception, only return no value if the value was not valid.
			if (channelMatches.Count == 1)
			{
				return Color.FromRgb(channels[0], channels[0], channels[0]);
			}
			else if (channelMatches.Count == 3)
			{
				return Color.FromRgb(channels[0], channels[1], channels[2]);
			}
			else
			{
				return DependencyProperty.UnsetValue;
			}
		}
	}
}
