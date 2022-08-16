using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Chroma.View;
using Chroma.ViewModel;

namespace Chroma.View.Converters
{
	public class RGBChannelConverter : IValueConverter
	{
		public static MainWindowViewModel ViewModel;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not Color || parameter is not ColourChannel)
			{
				return DependencyProperty.UnsetValue;
			}

			Color colour = (Color)value;
			ColourChannel channelToRetrieve = (ColourChannel)parameter;

			return channelToRetrieve switch
			{
				ColourChannel.Alpha => colour.A,
				ColourChannel.Red => colour.R,
				ColourChannel.Green => colour.G,
				ColourChannel.Blue => colour.B,
				_ => DependencyProperty.UnsetValue,
			};
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not double || parameter is not ColourChannel)
			{
				return DependencyProperty.UnsetValue;
			}

			byte newChannel = (byte)(double)value;
			ColourChannel channel = (ColourChannel)parameter;
			Color currentColour = ViewModel.Colour;

			switch (channel)
			{
				case ColourChannel.Alpha:
					currentColour.A = newChannel;
					break;
				case ColourChannel.Red:
					currentColour.R = newChannel;
					break;
				case ColourChannel.Green:
					currentColour.G = newChannel;
					break;
				case ColourChannel.Blue:
					currentColour.B = newChannel;
					break;
			}

			return currentColour;
		}
	}
}
