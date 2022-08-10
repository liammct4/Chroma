using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Chroma.View.Converters
{
	/// <summary>
	/// Inverts a boolean provided.
	/// </summary>
	public class BooleanInverterConverter : IValueConverter
	{
		/// <summary>
		/// Inverts a provided boolean.
		/// </summary>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not bool)
			{
				return DependencyProperty.UnsetValue;
			}

			bool convertValue = (bool)value;
			return !convertValue;
		}

		/// <summary>
		/// Not Implemented.
		/// </summary>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}
}
