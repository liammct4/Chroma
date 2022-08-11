using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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

		/// <summary>
		/// Generates a random colour with each colour channel (excluding) ranging from 0 to 255.
		/// </summary>
		/// <returns>A new random colour.</returns>
		public static Color GenerateRandomColor()
		{
			byte r = (byte)colourRandom.Next(0, 255);
			byte g = (byte)colourRandom.Next(0, 255);
			byte b = (byte)colourRandom.Next(0, 255);

			return Color.FromRgb(r, g, b);
		}

		/// <summary>
		/// Extracts colours from a json file.
		/// </summary>
		/// <param name="file">The path to the file.</param>
		/// <returns>A list containing each colour in the file.</returns>
		public static IList<ColourItem> ExtractFromJSON(string file)
		{
			using FileStream fs = File.OpenRead(file);
			using StreamReader sr = new(fs);
			using JsonDocument colourJson = JsonDocument.Parse(sr.ReadToEnd());
			
			ColourFile colours = (ColourFile)colourJson.Deserialize(typeof(ColourFile));

			return colours.Colours;
		}

		/// <summary>
		/// Creates a json file from a list of colours.
		/// </summary>
		/// <param name="file">The file which the json data will be written to.</param>
		/// <param name="items">The colours which will be written to the file.</param>
		public static void CreateJsonFrom(string file, IEnumerable<ColourItem> items)
		{
			using FileStream fs = File.OpenWrite(file);
			ColourFile newFile = new()
			{
				Colours = items.ToArray()
			};

			// Make sure to clear the previous data.
			fs.SetLength(0);
			fs.Flush();

			string json = JsonSerializer.Serialize(newFile);
			byte[] streamData = Encoding.UTF8.GetBytes(json);

			fs.Write(streamData);
		}
	}

	public class ColourFile
	{
		public ColourItem[] Colours { get; set; }
	}
}
