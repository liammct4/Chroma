using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chroma.Model.Utilities
{
	public static class GenericUtilities
	{
		/// <summary>
		/// Returns the maximum value with a provided set of values.
		/// </summary>
		public static T Max<T>(params T[] values) => values.Max();
	}
}
