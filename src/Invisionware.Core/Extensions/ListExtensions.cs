// ***********************************************************************
// Assembly         : Invisionware.Core
// Author           : Shawn Anderson
// Created          : 01-28-2018
//
// Last Modified By : Shawn Anderson
// Last Modified On : 01-28-2018
// ***********************************************************************
// <copyright file="ListExtensions.cs" company="Invisionware">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System.Linq;

namespace Invisionware.Extensions
{
	/// <summary>
	/// Class ListExtensions.
	/// </summary>
	public static class ListExtensions
	{
		/// <summary>
		/// Removes the range.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">The list.</param>
		/// <param name="index">The index.</param>
		/// <param name="count">The count.</param>
		/// <returns>IList&lt;T&gt;.</returns>
		public static IList<T> RemoveRange<T>(this IList<T> list, int index, int count)
		{
			if (list == null || !list.Any()) return list;
			if (index < 0 || index > list.Count) return list;
			if (count < 0 || count > list.Count || index + count > list.Count) return list;

			var result = list.ToList();
			result.RemoveRange(index, count);

			return result;
		}
	}
}
