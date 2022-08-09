using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows;
using Chroma.Model;
using Chroma.Model.Utilities;

namespace Chroma.View.Converters
{
	/// <summary>
	/// Brightens a given colour to its brightest possible value.
	/// </summary>
	public class BrightenedColourConverter : IValueConverter
	{
		/// <summary>
		/// Brightens a given colour to its brightest possible value.
		/// </summary>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not Color)
			{
				return DependencyProperty.UnsetValue;
			}

			Color colour = (Color)value;

			return ColourUtilities.BrightenColour(colour);
		}

		/// <summary>
		/// Not implemented.
		/// </summary>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}
}
