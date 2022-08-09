using Chroma.Model.Utilities;
using Chroma.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Chroma.View.Converters
{
	/// <summary>
	/// Retrieves the biggest channel in a colour (excluding alpha).
	/// </summary>
	public class GradientScaleConverter : IValueConverter
	{
		// Since parameters cannot be passed through as a binding, the view model will have to be referenced directly.
		internal static MainWindowViewModel ViewModel;

		/// <summary>
		/// Retrieves the biggest channel in a colour (excluding alpha).
		/// </summary>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Color colour = (Color)value;
			int biggestChannel = GenericUtilities.Max(colour.R, colour.G, colour.B);
			
			return biggestChannel;
		}

		/// <summary>
		/// Brightens or darkens a colour based on the channel value provided. The biggest channel will be the <paramref name="value"/> given.
		/// </summary>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			const byte LOWER_MINIMUM = 1;
			double channelValue = (double)value;
			Color currentColour = ViewModel.CurrentColour.Colour;

			int biggestChannel = GenericUtilities.Max(currentColour.R, currentColour.G, currentColour.B, 1);
			double multiplierChange = channelValue / biggestChannel;

			return Color.FromRgb(
				r: (byte)Math.Ceiling(Math.Max(currentColour.R, LOWER_MINIMUM) * multiplierChange),
				g: (byte)Math.Ceiling(Math.Max(currentColour.G, LOWER_MINIMUM) * multiplierChange),
				b: (byte)Math.Ceiling(Math.Max(currentColour.B, LOWER_MINIMUM) * multiplierChange)
			);
		}
	}
}
