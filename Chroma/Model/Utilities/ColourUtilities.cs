using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Chroma.Model;
using Drawing = System.Drawing;
using WinForms = System.Windows.Forms;

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

		/// <summary>
		/// Gets the colour underneath the cursor.
		/// </summary>
		/// <returns></returns>
		public static Color GetColourAtMousePoint()
		{
			Drawing.Rectangle rect = WinForms.Screen.GetBounds(new Drawing.Point(0, 0));
			Drawing.Point mousePoint = WinForms.Cursor.Position;

			using Drawing.Bitmap bitmap = new(rect.Width, rect.Height);
			using (Drawing.Graphics g = Drawing.Graphics.FromImage(bitmap))
			{
				g.CopyFromScreen(new Drawing.Point(rect.Left, rect.Top), Drawing.Point.Empty, rect.Size);
			}

			Drawing.Color colour = bitmap.GetPixel(mousePoint.X, mousePoint.Y);

			bitmap.Save(@"C:\Users\Liam\Desktop\Winforms.png");

			return Color.FromArgb(colour.A, colour.R, colour.G, colour.B);
		}
	}
}
