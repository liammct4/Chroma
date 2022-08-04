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
	/// Converts an ARGB colour object to a string and a string to an ARGB colour object.
	/// </summary>
	public class ARGBColourConverter : IValueConverter
	{
		/// <summary>
		/// Converts a <see cref="Color"/> object to a string representation in the format: "{A}, {R}, {G}, {B}"
		/// </summary>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not Color)
			{
				return DependencyProperty.UnsetValue;
			}

			Color colour = (Color)value;
			return $"{colour.A}, {colour.R}, {colour.G}, {colour.B}";
		}

		/// <summary>
		/// Converts a <see cref="string"/> representing an ARGB colour to a <see cref="Color"/>.
		/// </summary>s
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not string)
			{
				return DependencyProperty.UnsetValue;
			}

			string colour = (string)value;

			// Isolates each number in the string to hopefully provide four values representing the colour channels.
			MatchCollection channelMatches = RGBColourConverter.RGBStringFormat.Matches(colour);
			byte[] channels = channelMatches.Select(x => (byte)int.Parse(x.Value)).ToArray();

			// Do not throw an exception, only return no value.
			if (channelMatches.Count == 1)
			{
				return Color.FromArgb(channels[0], channels[0], channels[0], channels[0]);
			}
			else if (channelMatches.Count == 4)
			{
				return Color.FromArgb(channels[0], channels[1], channels[2], channels[3]);
			}
			else
			{
				return DependencyProperty.UnsetValue;
			}
		}
	}
}
