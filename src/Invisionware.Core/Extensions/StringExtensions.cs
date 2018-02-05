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

namespace Invisionware.Core.Extensions
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
	}
}
