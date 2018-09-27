// ***********************************************************************
// Assembly         : Invisionware.Core
// Author           : Shawn Anderson
// Created          : 01-27-2018
//
// Last Modified By : Shawn Anderson
// Last Modified On : 01-23-2018
// ***********************************************************************
// <copyright file="StringExtensions.cs" company="Invisionware">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;

namespace Invisionware
{
	/// <summary>
	/// Class StringExtensions.
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Determines whether [is null or empty] [the specified string].
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns><c>true</c> if [is null or empty] [the specified string]; otherwise, <c>false</c>.</returns>
		public static bool IsNullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}

		/// <summary>
		/// Enhanced String Compare function that handles null values on either side
		/// Note: If both sides are null a -999 is returned
		/// </summary>
		/// <param name="x">The first string to compare.</param>
		/// <param name="y">The secondstring to compare.</param>
		/// <param name="comparison">The comparison type to use.</param>
		/// <returns>System.Int32.</returns>
		public static int CompareEx(this string x, string y, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
		{
			if (x.IsNullOrEmpty() && y.IsNullOrEmpty()) return -999;
			if (x == null && y != null) return 1;
			if (x != null && y == null) return -1;

			return string.Compare(x, y, comparison);
		}

		/// <summary>
		/// To the base64.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns>System.String.</returns>
		public static string ToBase64(this string source)
		{
			if (source.IsNullOrEmpty()) return null;

			var ret = System.Text.Encoding.UTF8.GetBytes(source);
			var s = Convert.ToBase64String(ret);

			return s;
		}
	}
}
