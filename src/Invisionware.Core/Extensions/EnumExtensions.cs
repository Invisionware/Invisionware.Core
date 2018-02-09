// ***********************************************************************
// Assembly         : Invisionware.Core
// Author           : Shawn Anderson
// Created          : 01-27-2018
//
// Last Modified By : Shawn Anderson
// Last Modified On : 01-23-2018
// ***********************************************************************
// <copyright file="EnumExtensions.cs" company="Invisionware">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Runtime.Serialization;

namespace Invisionware.Extensions
{
	/// <summary>
	/// Class EnumExtensions.
	/// </summary>
	public static class EnumExtensions
	{
		/// <summary>
		/// Gets the type of the attribute of.
		/// </summary>
		/// <typeparam name="TAttribute"></typeparam>
		/// <param name="enumVal">The enum value.</param>
		/// <returns>T.</returns>
		public static TAttribute GetAttributeOfType<TAttribute>(this Enum enumVal) where TAttribute : Attribute
		{
			var type = enumVal.GetType();
			var memInfo = type.GetMember(enumVal.ToString());
			var attributes = memInfo[0].GetCustomAttributes(typeof(TAttribute), false);
			return (attributes.Length > 0) ? (TAttribute)attributes[0] : null;
		}

		/// <summary>
		/// Gets the enum value.
		/// </summary>
		/// <param name="enumVal">The enum value.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>System.String.</returns>
		public static string GetEnumValue(this Enum enumVal, string defaultValue)
		{
			return enumVal.GetAttributeOfType<EnumMemberAttribute>()?.Value ?? defaultValue;
		}

		/// <summary>
		/// Tries the convert.
		/// </summary>
		/// <typeparam name="TDestination">The type of the t destination.</typeparam>
		/// <param name="sourceEnum">The source enum.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>TDestination.</returns>
		public static TDestination TryConvert<TDestination>(this Enum sourceEnum, TDestination defaultValue = default(TDestination))
			where TDestination : struct
		{
			if (Enum.TryParse(sourceEnum.ToString(), true, out TDestination result))
			{
				return result;
			}

			return defaultValue;
		}
	}
}
