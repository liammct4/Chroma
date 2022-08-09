using Chroma.Model;
using Chroma.View.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Chroma.ViewModel
{
	/// <summary>
	/// View model for a <see cref="MainWindow"/>.
	/// </summary>
	public class MainWindowViewModel : ViewModelBase
	{
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
		/// Command triggered when the move up button has been pressed.
		/// </summary>
		public ICommand ColoursMoveUpButtonCommand { get; set; }
		/// <summary>
		/// Command triggered when the move down button has been pressed.
		/// </summary>
		public ICommand ColoursMoveDownButtonCommand { get; set; }

		public MainWindowViewModel()
		{
			GradientScaleConverter.ViewModel = this;
			Func<object, bool> colourRequiredCondition = x => CurrentColour is not null;

			// Link commands.
			AddColourCommand = new RelayCommand(AddColour, x => true);
			RemoveColourCommand = new RelayCommand(RemoveColour, colourRequiredCondition);
			RenameColourCommand = new RelayCommand(RenameColour, colourRequiredCondition);
			ColourEditBoxEnterCommand = new RelayCommand(ColourEditBoxEnter, x => IsEdit);
			CopyColourButtonCommand = new RelayCommand(CopyColour, colourRequiredCondition);
			DuplicateButtonCommand = new RelayCommand(DuplicateColour, colourRequiredCondition);
			ColoursMoveUpButtonCommand = new RelayCommand(MoveUp, colourRequiredCondition);
			ColoursMoveDownButtonCommand = new RelayCommand(MoveDown, colourRequiredCondition);

			// Test data.
			SavedColours = new ObservableCollection<ColourItem>()
			{
				new ColourItem() { Colour = Color.FromArgb(255, 100, 30, 180) },
				new ColourItem() { Colour = Color.FromArgb(255, 130, 60, 90) },
				new ColourItem() { Colour = Color.FromArgb(255, 59, 10, 130) },
				new ColourItem() { Colour = Color.FromArgb(255, 10, 90, 110) },
				new ColourItem() { Colour = Color.FromArgb(255, 255, 110, 100) },
			};
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
		public void MoveUp(object? parameter)
		{
			int index = SavedColours.IndexOf(CurrentColour);

			if (index == 0)
			{
				return;
			}

			SavedColours.Move(index, index - 1);
		}

		/// <summary>
		/// Triggered when the move down button is pressed.
		/// 
		/// If no colour is selected, the command isn't ran.
		/// </summary>
		public void MoveDown(object? parameter)
		{
			int index = SavedColours.IndexOf(CurrentColour);

			if (index + 1 > SavedColours.Count - 1)
			{
				return;
			}

			SavedColours.Move(index, index + 1);
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
		#endregion
		
	}
}
