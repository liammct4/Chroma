using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Chroma.Model;

namespace Chroma.Model.Utilities
{
	/// <summary>
	/// Provides utility methods for colours.
	/// </summary>
	public static class ColourUtilities
	{
		/// <summary>
		/// Brightens a colour until any colour channel (Excluding alpha) reaches <paramref name="maximumChannel"/>.
		/// </summary>
		/// <param name="baseColour">The colour to brighten.</param>
		/// <param name="maximumChannel">The maximum value for every channel. Default is 255.</param>
		/// <returns>A brightend colour of <paramref name="baseColour"/>.</returns>
		public static Color BrightenColour(Color baseColour, int maximumChannel = 255)
		{
			const byte LOWER_MINIMUM = 1;
			double multiplierChange;

			byte ApplyMultiplierChange(int channel) => (byte)(Math.Max(channel, LOWER_MINIMUM) * multiplierChange);

			// If the base colour is black, the method would return black instead of white so the "1" parameter is needed.
			int biggestChannel = GenericUtilities.Max(baseColour.R, baseColour.G, baseColour.B, 1);
			multiplierChange = maximumChannel / (double)biggestChannel;

			Color brightened = Color.FromRgb(
				r: ApplyMultiplierChange(baseColour.R),
				g: ApplyMultiplierChange(baseColour.G),
				b: ApplyMultiplierChange(baseColour.B)
			);

			return brightened;
		}
	}
}
