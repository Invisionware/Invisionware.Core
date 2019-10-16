// ***********************************************************************
// Assembly         : Invisionware.Core
// Author           : Shawn Anderson
// Created          : 01-27-2018
//
// Last Modified By : Shawn Anderson
// Last Modified On : 01-23-2018
// ***********************************************************************
// <copyright file="PropertyInfoExtensions.cs" company="Invisionware">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Invisionware
{
	public static class PropertyInfoExtensions
	{
		/// <summary>
		/// Attempts to retrieve the attribute from the current type based on the value selector function.
		/// </summary>
		/// <typeparam name="TAttribute">The type of the t attribute.</typeparam>
		/// <typeparam name="TValue">The type of the t value.</typeparam>
		/// <param name="property">The type.</param>
		/// <param name="valueSelector">The value selector.</param>
		/// <example>typeof(SettingsAttribute).GetAttributeValue&lt;string&gt;((SettingsAttribute s) => s.Name); </example>
		/// <returns>TValue.</returns>
		public static TValue GetAttributeValue<TAttribute, TValue>(this PropertyInfo property, Func<TAttribute, TValue> valueSelector, bool includeInherited = true)
			where TAttribute : Attribute
		{
			var att = property.GetAttributeOfType<TAttribute>(includeInherited).FirstOrDefault();

			return att != null ? valueSelector(att) : default(TValue);
		}

		/// <summary>
		/// Gets the speciifc attribute on the property
		/// </summary>
		/// <typeparam name="TAttribute">The type of the attribute.</typeparam>
		/// <param name="property">The property.</param>
		/// <param name="includeInherited">if set to <c>true</c> [include inherited].</param>
		/// <returns>The attribute.</returns>
		public static IList<TAttribute> GetAttributeOfType<TAttribute>(this PropertyInfo property, bool includeInherited = true) where TAttribute : System.Attribute
		{			
			var results = property.GetCustomAttributes(typeof(TAttribute), true)?.Cast<TAttribute>()?.ToList();

			if (!includeInherited) return results;

			var nestedAtt = property.DeclaringType.GetInterfaces().SelectMany(x => x.GetProperties().Where(y => y.Name == property.Name).SelectMany(z => z.GetAttributeOfType<TAttribute>(false)))?.ToList();

			if (results == null)
			{
				results = nestedAtt;
			}
			else if (nestedAtt != null && nestedAtt.Any())
			{
				results.AddRange(nestedAtt);
			}

			return results;
		}

		/// <summary>
		/// Determines if the specified attribute exists on the specified type
		/// </summary>
		/// <typeparam name="TAttribute">The type of the t attribute.</typeparam>
		/// <param name="property">The property.</param>
		/// <param name="includeInherited">if set to <c>true</c> [include inherited].</param>
		/// <returns><c>true</c> if the attribute exists on the type, <c>false</c> otherwise.</returns>
		public static bool AttributeExists<TAttribute>(this PropertyInfo property, bool includeInherited = true) where TAttribute : System.Attribute
		{
			return property.GetAttributeOfType<TAttribute>(includeInherited) != null;
		}
	}
}
