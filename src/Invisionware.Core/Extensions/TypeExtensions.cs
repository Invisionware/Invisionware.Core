// ***********************************************************************
// Assembly         : Invisionware.Core
// Author           : Shawn Anderson
// Created          : 01-27-2018
//
// Last Modified By : Shawn Anderson
// Last Modified On : 01-23-2018
// ***********************************************************************
// <copyright file="TypeExtensions.cs" company="Invisionware">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace Invisionware.Core.Extensions
{
	/// <summary>
	/// Class TypeExtensions.
	/// </summary>
	public static class TypeExtensions
	{
		/// <summary>
		/// Attempts to retrieve .
		/// </summary>
		/// <typeparam name="TAttribute">The type of the t attribute.</typeparam>
		/// <typeparam name="TValue">The type of the t value.</typeparam>
		/// <param name="type">The type.</param>
		/// <param name="valueSelector">The value selector.</param>
		/// <example>typeof(SettingsAttribute).GetAttributeValue&lt;string&gt;((SettingsAttribute s) => s.Name); </example>
		/// <returns>TValue.</returns>
		public static TValue GetAttributeValue<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> valueSelector)
			where TAttribute : Attribute
		{
			var att = type.GetAttributeOfType<TAttribute>().FirstOrDefault();

			return att != null ? valueSelector(att) : default(TValue);
		}

		/// <summary>
		/// Gets the speciifc attribute on the type
		/// </summary>
		/// <typeparam name="TAttribute"></typeparam>
		/// <returns>The attribute.</returns>
		public static IList<TAttribute> GetAttributeOfType<TAttribute>(this Type type, bool includeInherited = true) where TAttribute : System.Attribute
		{
			var results = type.GetCustomAttributes(typeof(TAttribute), true)?.Cast<TAttribute>()?.ToList();

			if (!includeInherited) return results;

			var nestedAtt = type.GetInterfaces().SelectMany(x => x.GetAttributeOfType<TAttribute>(true))?.ToList();

			if (results == null)
			{
				results = nestedAtt;
			}
			else if (nestedAtt.AnySafe())
			{
				results.AddRange(nestedAtt);
			}

			return results;
		}

		/// <summary>
		/// Determines if the specified attribute exists on the specified type
		/// </summary>
		/// <typeparam name="TAttribute">The type of the t attribute.</typeparam>
		/// <param name="type">The type.</param>
		/// <returns><c>true</c> if the attribute exists on the type, <c>false</c> otherwise.</returns>
		public static bool AttributeExists<TAttribute>(this Type type) where TAttribute : System.Attribute
		{
			return type.GetAttributeOfType<TAttribute>() != null;
		}
	}
}
