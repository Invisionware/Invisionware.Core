// ***********************************************************************
// Assembly         : Invisionware.Core
// Author           : Shawn Anderson
// Created          : 01-27-2018
//
// Last Modified By : Shawn Anderson
// Last Modified On : 01-23-2018
// ***********************************************************************
// <copyright file="DateTimeExtensions.cs" company="Invisionware">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;

namespace Invisionware
{
	/// <summary>
	/// Class DateTimeExtensions.
	/// </summary>
	public static class DateTimeExtensions
	{
		/// <summary>
		/// The unix time
		/// </summary>
		public static DateTime UnixTime = new DateTime(1970, 1, 1);

		/// <summary>
		/// Sinces the unix time.
		/// </summary>
		/// <param name="time">The time.</param>
		/// <returns>TimeSpan.</returns>
		public static TimeSpan SinceUnixTime(this DateTime time)
		{
			return time - UnixTime;
		}

		/// <summary>
		/// Sinces the unix time.
		/// </summary>
		/// <param name="time">The time.</param>
		/// <returns>TimeSpan.</returns>
		public static TimeSpan SinceUnixTime(this DateTimeOffset time)
		{
			return time - UnixTime;
		}

		/// <summary>
		/// Sinces the unix time.
		/// </summary>
		/// <param name="time">The time.</param>
		/// <returns>System.Nullable&lt;TimeSpan&gt;.</returns>
		public static TimeSpan? SinceUnixTime(this DateTime? time)
		{
			return time - UnixTime;
		}

		/// <summary>
		/// Sinces the unix time.
		/// </summary>
		/// <param name="time">The time.</param>
		/// <returns>System.Nullable&lt;TimeSpan&gt;.</returns>
		public static TimeSpan? SinceUnixTime(this DateTimeOffset? time)
		{
			return time - UnixTime;
		}

		/// <summary>
		/// Fulls the milliseconds.
		/// </summary>
		/// <param name="timeSpan">The time span.</param>
		/// <returns>System.Nullable&lt;System.Int64&gt;.</returns>
		public static long? FullMilliseconds(this TimeSpan? timeSpan)
		{
			return timeSpan == null ? default(long?) : (long)timeSpan.Value.TotalMilliseconds;
		}

		/// <summary>
		/// To the universal time.
		/// </summary>
		/// <param name="dateTime">The date time.</param>
		/// <returns>System.Nullable&lt;DateTime&gt;.</returns>
		public static DateTime? ToUniversalTime(this DateTime? dateTime)
		{
			return dateTime != null ? dateTime.Value.ToUniversalTime() : (DateTime?)null;
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <param name="dateTime">The date time.</param>
		/// <param name="format">The format.</param>
		/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
		public static string ToString(this DateTime? dateTime, string format)
		{
			return dateTime?.ToString(format);
		}

		/// <summary>
		/// Add days to the datatime (supports Nullable datatime objects).
		/// </summary>
		/// <param name="dateTime">The date time.</param>
		/// <param name="value">The value.</param>
		/// <returns>System.Nullable&lt;DateTime&gt;.</returns>
		public static DateTime? AddDays(this DateTime? dateTime, double value)
		{
			return dateTime == null ? (DateTime?)null : dateTime.Value.AddDays(value);
		}
	}
}
