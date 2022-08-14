using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Chroma.Model;
using Chroma.View.Converters;
using Chroma.Model.Functionality;
using Chroma.Model.Utilities;
using Microsoft.Win32;
using System.Text.Json;

namespace Chroma.ViewModel
{
	/// <summary>
	/// View model for a <see cref="MainWindow"/>.
	/// </summary>
	public class MainWindowViewModel : ViewModelBase
	{
		/// <summary>
		/// Gets the path for the saved colours.
		/// </summary>
		public static string SavedColourFilePath => Path.Combine(Directory.GetCurrentDirectory(), "config.json");
		/// <summary>
		/// Command triggered when the "Add" button is pressed underneath the colour group.
		/// </summary>
		public ICommand AddColourCommand { get; set; }
		/// <summary>
		/// Command triggered when the "Remove" button is pressed underneath the colour group.
		/// </summary>
		public ICommand RemoveColourCommand { get; set; }
		/// <summary>
		/// Command triggered when the "Rename" button is pressed underneath the colour group.
		/// </summary>
		public ICommand RenameColourCommand { get; set; }
		/// <summary>
		/// Command triggered when renaming a colour and the user presses enter signalling to stop renaming.
		/// </summary>
		public ICommand ColourEditBoxEnterCommand { get; set; }
		/// <summary>
		/// Command triggered when a copy colour button is pressed alongside a colour text box (rgb, argb, hex etc).
		/// 
		/// Each button provides its own converter to create the string to place in the clipboard.
		/// </summary>
		public ICommand CopyColourButtonCommand { get; set; }
		/// <summary>
		/// Command triggered when the duplicate button has been pressed.
		/// </summary>
		public ICommand DuplicateButtonCommand { get; set; }
		/// <summary>
		/// Command triggered when the move up/down button has been pressed or the move up/down key has been pressed.
		/// </summary>
		public ICommand ColoursMoveCommand { get; set; }
		/// <summary>
		/// Command triggered when the randomize button has been pressed.
		/// </summary>
		public ICommand RandomizeColourButtonCommand { get; set; }
		/// <summary>
		/// Command triggered when the colour picker button has been pressed.
		/// </summary>
		public ICommand ColourPickerButtonCommand { get; set; }
		/// <summary>
		/// Command triggered while app is in colour picker mode and the escape key has been pressed indicating to cancel.
		/// </summary>
		public ICommand ColourPickerCancelCommand { get; set; }
		/// <summary>
		/// Command triggered when the clear colours button has been pressed.
		/// </summary>
		public ICommand ClearColoursCommand { get; set; }
		/// <summary>
		/// Command triggered when the export colours button has been pressed.
		/// </summary>
		public ICommand ExportColoursCommand { get; set; }
		/// <summary>
		/// Command triggered when the import colours button has been pressed.
		/// </summary>
		public ICommand ImportColoursCommand { get; set; }

		public MainWindowViewModel()
		{
			GradientScaleConverter.ViewModel = this;
			Func<object, bool> colourRequiredCondition = x => CurrentColour is not null;
			Func<object, bool> colourExistCondition = x => SavedColours.Count >= 1;

			// Link commands.
			AddColourCommand = new RelayCommand(AddColour, x => true);
			RemoveColourCommand = new RelayCommand(RemoveColour, colourRequiredCondition);
			RenameColourCommand = new RelayCommand(RenameColour, colourRequiredCondition);
			ColourEditBoxEnterCommand = new RelayCommand(ColourEditBoxEnter, x => IsEdit);
			CopyColourButtonCommand = new RelayCommand(CopyColour, colourRequiredCondition);
			DuplicateButtonCommand = new RelayCommand(DuplicateColour, colourRequiredCondition);
			ColoursMoveCommand = new RelayCommand(MoveColour, colourRequiredCondition);
			RandomizeColourButtonCommand = new RelayCommand(RandomizeColour, colourRequiredCondition);
			ColourPickerButtonCommand = new RelayCommand(StartColourPicker, colourRequiredCondition);
			ClearColoursCommand = new RelayCommand(ClearColours, colourExistCondition);
			ExportColoursCommand = new RelayCommand(ExportColours, colourExistCondition);
			ImportColoursCommand = new RelayCommand(ImportColours, x => true);
			ColourPickerCancelCommand = new RelayCommand(CancelColourPicker, x => IsColourPicker);

			LoadSettings();

			MouseHook.MouseAction += ColourPickerClickEvent;
		}

		#region Commands
		/// <summary>
		/// Triggered when the add button is pressed underneath the colour group.
		/// </summary>
		public void AddColour(object? parameter) => SavedColours.Add(new ColourItem());

		/// <summary>
		/// Triggered when the remove button is pressed underneath the colour group.
		/// 
		/// If no colour is selected, this method isn't ran.
		/// </summary>
		public void RemoveColour(object? parameter) => SavedColours.Remove(CurrentColour);

		/// <summary>
		/// Triggered when the rename button is pressed underneath the colour group.
		/// 
		/// If no colour is selected, this method isn't ran.
		/// </summary>
		public void RenameColour(object? parameter)
		{
			IsEdit = true;
		}

		/// <summary>
		/// Triggered when the user presses the enter key when renaming a colour.
		/// 
		/// Only applied if in edit mode.
		/// </summary>
		public void ColourEditBoxEnter(object? parameter)
		{
			IsEdit = false;
		}

		/// <summary>
		/// Triggered when a "copy to clipboard" button is pressed.
		/// </summary>
		/// <param name="parameter">The converter which will generate the string to set in the clipboard.</param>
		public void CopyColour(object? parameter)
		{
			IValueConverter converter = (IValueConverter)parameter;
			string colourString = (string)converter.Convert(CurrentColour.Colour, typeof(Color), null, CultureInfo.CurrentCulture);

			Clipboard.SetText(colourString);
		}

		/// <summary>
		/// Triggered when the duplicate colour button is pressed.
		/// 
		/// If no colour is selected, the command isn't ran.
		/// </summary>
		public void DuplicateColour(object? parameter)
		{
			// Since Color is a struct and structs are pass by value, no manual copying needed.
			Color duplicated = CurrentColour.Colour;

			ColourItem newItem = new() { Colour = duplicated };
			SavedColours.Add(newItem);
		}

		/// <summary>
		/// Triggered when the move up button is pressed.
		/// 
		/// If no colour is selected, the command isn't ran.
		/// </summary>
		public void MoveColour(object? parameter)
		{
			if (parameter is not ItemDirection)
			{
				return;
			}

			// The parameter indicates which direction to go (provided by the command binding in view).
			ItemDirection direction = (ItemDirection)parameter;

			int index = SavedColours.IndexOf(CurrentColour);
			int newIndex = index;

			if (direction == ItemDirection.Up)
			{
				newIndex--;
			}
			else if (direction == ItemDirection.Down)
			{
				newIndex++;
			}

			// Check if the new index is out of bounds.
			if (newIndex < 0 || newIndex > SavedColours.Count - 1)
			{
				return;
			}

			SavedColours.Move(index, newIndex);
		}

		/// <summary>
		/// Triggered when the randomize button is pressed.
		/// 
		/// If no colour is selected, the command isn't ran.
		/// </summary>
		public void RandomizeColour(object? parameter)
		{
			Color randomColour = ColourItem.GenerateRandomColor();
			Colour = randomColour;
		}

		/// <summary>
		/// Triggered when the colour picker button is pressed.
		/// 
		/// If no colour is selected, the command isn't ran.
		/// </summary>
		public void StartColourPicker(object? parameter)
		{
			IsColourPicker = true;
			IsOnTop = true;
		}

		/// <summary>
		/// Triggered when the escape key has been pressed in colour picker mode.
		/// </summary>
		public async void CancelColourPicker(object? parameter)
		{
			IsColourPicker = false;

			await Task.Delay(100);
			IsOnTop = false;
		}

		/// <summary>
		/// Triggered when the clear colours button has been pressed.
		/// </summary>
		public void ClearColours(object? parameter)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure you want to clear all colours?", "Confirm Clear", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (result == MessageBoxResult.Yes)
			{
				SavedColours.Clear();
			}
		}

		/// <summary>
		/// Triggered when the export colours button has been pressed.
		/// </summary>
		public void ExportColours(object? parameter)
		{
			SaveFileDialog dialog = new()
			{
				Title = "Export File",
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
				Filter = "JSON Files (*.json)|*.json|All files (*.*)|*.*"
			};

			bool? userHasSaved = dialog.ShowDialog();

			if (userHasSaved is true)
			{
				ColourItem.CreateJsonFrom(dialog.FileName, SavedColours);
			}
		}

		/// <summary>
		/// Triggered when the import colours button has been pressed.
		/// </summary>
		public void ImportColours(object? parameter)
		{
			MessageBoxResult result = MessageBox.Show("Do you want to clear all current colours first before importing?",
				"Import Colours",
				MessageBoxButton.YesNoCancel,
				MessageBoxImage.Question);

			if (result == MessageBoxResult.Cancel)
			{
				return;
			}

			OpenFileDialog dialog = new()
			{
				Title = "Import Colours",
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
				Filter = "JSON Files (*.json)|*.json|All files (*.*)|*.*"
			};

			bool? userHasSelected = dialog.ShowDialog();

			if (userHasSelected is true)
			{
				IList<ColourItem> importedColours = ColourItem.ExtractFromJSON(dialog.FileName);
				if (result == MessageBoxResult.Yes)
				{
					SavedColours = new ObservableCollection<ColourItem>(importedColours);
				}
				else
				{
					// Do the concatenation in a separate collection as each item added would notfiy property changed.
					IEnumerable<ColourItem> mixedItems = SavedColours.Concat(importedColours);
					SavedColours = new ObservableCollection<ColourItem>(mixedItems);
				}
			}
		}

		/// <summary>
		/// Triggered when the <see cref="MouseHook.MouseAction"/> event is raised and the application is in ColourPicker Mode.
		/// </summary>
		public async void ColourPickerClickEvent(object sender, EventArgs e)
		{
			Colour = ColourUtilities.GetColourAtMousePoint();
			IsColourPicker = false;

			await Task.Delay(100);
			IsOnTop = false;
		}

		/// <summary>
		/// Occurs when the application closes. The current colours will be saved.
		/// </summary>
		public void ApplicationClosing(object? sender, EventArgs e)
		{
			SaveSettings();
		}
		#endregion
		#region Binding Properties
		public ObservableCollection<ColourItem> SavedColours
		{
			get => _savedColours;
			set
			{
				_savedColours = value;
				OnPropertyChanged(nameof(SavedColours));
			}
		}
		private ObservableCollection<ColourItem> _savedColours;
		public ColourItem CurrentColour
		{
			get => _currentColour;
			set
			{
				_currentColour = value;
				IsEdit = false;
				OnPropertyChanged(nameof(CurrentColour));
				OnPropertyChanged(nameof(Colour));
				OnPropertyChanged(nameof(A));
				OnPropertyChanged(nameof(R));
				OnPropertyChanged(nameof(G));
				OnPropertyChanged(nameof(B));
				RelayCommand.RaiseCanExecuteChanged();
			}
		}
		private ColourItem _currentColour;
		public Color Colour
		{
			get => CurrentColour is null ? Color.FromRgb(255, 255, 255) : CurrentColour.Colour;
			set
			{
				CurrentColour.Colour = value;
				OnPropertyChanged(nameof(Colour));
				OnPropertyChanged(nameof(CurrentColour));
				OnPropertyChanged(nameof(A));
				OnPropertyChanged(nameof(R));
				OnPropertyChanged(nameof(G));
				OnPropertyChanged(nameof(B));
			}
		}
		public bool IsEdit
		{
			get => _isEdit;
			set
			{
				_isEdit = value;
				OnPropertyChanged(nameof(IsEdit));
			}
		}
		private bool _isEdit;
		public int A
		{
			get => CurrentColour is null ? 255 : CurrentColour.Colour.A;
			set
			{
				CurrentColour.Colour = Color.FromArgb((byte)value, CurrentColour.Colour.R, CurrentColour.Colour.G, CurrentColour.Colour.B);
				OnPropertyChanged(nameof(A));
				OnPropertyChanged(nameof(Colour));
			}
		}
		public int R
		{
			get => CurrentColour is null ? 255 : CurrentColour.Colour.R;
			set
			{
				CurrentColour.Colour = Color.FromArgb(CurrentColour.Colour.A, (byte)value, CurrentColour.Colour.G, CurrentColour.Colour.B);
				OnPropertyChanged(nameof(R));
				OnPropertyChanged(nameof(Colour));
			}
		}
		public int G
		{
			get => CurrentColour is null ? 255 : CurrentColour.Colour.G;
			set
			{
				CurrentColour.Colour = Color.FromArgb(CurrentColour.Colour.A, CurrentColour.Colour.R, (byte)value, CurrentColour.Colour.B);
				OnPropertyChanged(nameof(G));
				OnPropertyChanged(nameof(Colour));
			}
		}
		public int B
		{
			get => CurrentColour is null ? 255 : CurrentColour.Colour.B;
			set
			{
				CurrentColour.Colour = Color.FromArgb(CurrentColour.Colour.A, CurrentColour.Colour.R, CurrentColour.Colour.G, (byte)value);
				OnPropertyChanged(nameof(B));
				OnPropertyChanged(nameof(Colour));
			}
		}
		public bool IsColourPicker
		{
			get => _isColourPicker;
			set
			{
				_isColourPicker = value;
				OnPropertyChanged(nameof(IsColourPicker));

				if (value)
				{
					MouseHook.Start();
				}
				else
				{
					MouseHook.Stop();
				}
			}
		}
		private bool _isColourPicker;
		public bool IsOnTop
		{
			get => _isOnTop;
			set
			{
				_isOnTop = value;
				OnPropertyChanged(nameof(IsOnTop));
			}
		}
		private bool _isOnTop;
		public bool SaveColoursOnClose
		{
			get => _saveColoursOnClose;
			set
			{
				_saveColoursOnClose = value;
				OnPropertyChanged(nameof(SaveColoursOnClose));
			}
		}
		private bool _saveColoursOnClose;
		#endregion
		/// <summary>
		/// Loads the settings from the previous application session.
		/// </summary>
		public void LoadSettings()
		{
			using FileStream fs = File.OpenRead(SavedColourFilePath);
			using StreamReader sr = new(fs);
			using JsonDocument colourJson = JsonDocument.Parse(sr.ReadToEnd());

			JsonConfig colours = (JsonConfig)colourJson.Deserialize(typeof(JsonConfig));
			SavedColours = new ObservableCollection<ColourItem>(colours.Colours);
			SaveColoursOnClose = colours.SaveColoursOnClose;
		}

		/// <summary>
		/// Saves the settings from the previous application session.
		/// </summary>
		public void SaveSettings()
		{
			ColourItem[] items;

			if (SaveColoursOnClose)
			{
				items = SavedColours.ToArray();
			}
			else
			{
				items = ColourItem.ExtractFromJSON(SavedColourFilePath).ToArray();
			}

			using FileStream fs = File.OpenWrite(SavedColourFilePath);

			JsonConfig newFile = new()
			{
				Colours = items,
				SaveColoursOnClose = SaveColoursOnClose
			};

			// Make sure to clear the previous data.
			fs.SetLength(0);
			fs.Flush();

			JsonSerializerOptions outputOptions = new()
			{
				WriteIndented = true
			};
			string json = JsonSerializer.Serialize(newFile, outputOptions);
			byte[] streamData = Encoding.UTF8.GetBytes(json);

			fs.Write(streamData);
		}
	}
}
