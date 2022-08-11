using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chroma.Model;

namespace Chroma.ViewModel
{
	public class JsonConfig
	{
		public ColourItem[] Colours { get; set; }
		public bool SaveColoursOnClose { get; set; }
	}
}
