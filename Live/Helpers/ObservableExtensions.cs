using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace Live
{
	public static class ObservableExtensions
	{
		public static void AddRange<T> (this ObservableCollection<T> collection, IEnumerable<T> items)
		{
			items.ToList ().ForEach (collection.Add);
		}
	}
}

