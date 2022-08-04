using Chroma.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

		public MainWindowViewModel()
		{
			// Link commands.
			AddColourCommand = new RelayCommand(AddColour, x => true);
			RemoveColourCommand = new RelayCommand(RemoveColour, x => CurrentColour is not null);
			RenameColourCommand = new RelayCommand(RenameColour, x => CurrentColour is not null);

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
		/// If no colour is selected, nothing happens.
		/// </summary>
		public void RemoveColour(object? parameter)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Triggered when the rename button is pressed underneath the colour group.
		/// 
		/// If no colour is selected, nothing happens.
		/// </summary>
		public void RenameColour(object? parameter)
		{
			throw new NotImplementedException();
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
			get => CurrentColour is null ? 100 : CurrentColour.Colour.R;
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
