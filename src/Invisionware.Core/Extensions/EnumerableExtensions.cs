// ***********************************************************************
// Assembly         : Invisionware.Core
// Author           : Shawn Anderson
// Created          : 01-27-2018
//
// Last Modified By : Shawn Anderson
// Last Modified On : 01-27-2018
// ***********************************************************************
// <copyright file="EnumerableExtensions.cs" company="Invisionware">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace Invisionware.Extensions
{
	/// <summary>
	/// Class EnumerableExtensions.
	/// </summary>
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Breaks the array up into Chunks of a specific size.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">The source.</param>
		/// <param name="chunkSize">Size of the chunk.</param>
		/// <returns>A collection of collections that is split into slices</returns>
		public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
		{
			return source
				.Select((x, i) => new { Index = i, Value = x })
				.GroupBy(x => x.Index / chunkSize)
				.Select(x => x.Select(v => v.Value));
		}

		/// <summary>
		/// A null reference safe version of Any.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection">The collection.</param>
		/// <param name="predicate">The predicate.</param>
		/// <returns><c>true</c> if the collection is not null and contains any items, <c>false</c> otherwise.</returns>
		public static bool AnySafe<T>(this IEnumerable<T> collection, Func<T, bool> predicate = null)
		{
			return collection != null && (predicate != null ? collection.Any(predicate) : collection.Any());
		}

		/// <summary>
		/// A null reference safe version of Count.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection">The collection.</param>
		/// <param name="predicate">The predicate.</param>
		/// <returns>the number of items in the collection or 0 if its null.</returns>
		public static int CountSafe<T>(this IEnumerable<T> collection, Func<T, bool> predicate = null)
		{
			return collection != null ? (predicate != null ? collection.Count(predicate) : collection.Count()) : 0;
		}

		/// <summary>
		/// Takes the last n number of items in the array.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">The source.</param>
		/// <param name="count">The count.</param>
		/// <returns>a new collection that contains only the last count number of items</returns>
		public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int count)
		{
			return source?.Skip(Math.Max(0, source.Count() - count));
		}
	}
}
