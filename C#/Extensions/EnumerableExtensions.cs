using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KnowDiabetes.Core.Extensions
{
	/// <summary>
	/// Provides a set of static methods for querying objects that implement <see cref="IEnumerable{T}"/>.
	/// </summary>
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Determines whether a sequence is null or does not contain any elements.
		/// </summary>
		/// <param name="source">The <see cref="IEnumerable{T}"/> to check for null or emptiness.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <returns>false if the source is null or empty, otherwise true.</returns>
		public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null)
			{
				return true;
			}
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
					return false;
			}
			return true;
		}
		/// <summary>
		/// Returns empty collection if null. 
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <param name="items">The items.</param>
		/// <returns></returns>
		public static IEnumerable<TItem> EmptyIfNull<TItem>(this IEnumerable<TItem> items)
		{
			return items ?? Enumerable.Empty<TItem>();
		}
		/// <summary>
		/// Returns the enumerable as collection. Casts or creates a list if the cast fails.
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <param name="items">The items.</param>
		/// <returns></returns>
		public static IReadOnlyCollection<TItem> AsCollection<TItem>(this IEnumerable<TItem> items)
		{
			return (items as IReadOnlyCollection<TItem>) ?? items.ToList();
		}
		/// <summary>
		/// Skips all null items from the items
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <param name="items">The items.</param>
		/// <returns></returns>
		public static IEnumerable<TItem> SkipNulls<TItem>(this IEnumerable<TItem> items)
		{
			return items.Where(item => item != null);
		}
	}
}