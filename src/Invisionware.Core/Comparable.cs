// ***********************************************************************
// Assembly         : Invisionware.Core
// Author           : Shawn Anderson
// Created          : 01-27-2018
//
// Last Modified By : Shawn Anderson
// Last Modified On : 01-23-2018
// ***********************************************************************
// <copyright file="Comparable.cs" company="Invisionware">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;

namespace Invisionware
{
	public static class ComparableExtensions
	{
		public enum RangeBoundaryType
		{
			//[Description("Exclusive")]
			Exclusive,

			//[Description("Inclusive on both boundaries")]
			Inclusive,

			//[Description("Inclusive on only the lower boundary")]
			InclusiveLowerBoundaryOnly,

			//[Description("Inclusive on only the upper boundary")]
			InclusiveUpperBoundaryOnly
		}

		///-------------------------------------------------------------------------------------------------
		/// <summary> An IComparable extension method that checks to see if the value is between two other values. </summary>
		///
		/// <remarks> Sanderson, 11/9/2015. </remarks>
		///
		/// <param name="value">         The value to act on. </param>
		/// <param name="comparator0">   The comparator 0. </param>
		/// <param name="comparator1">   The first comparator. </param>
		/// <param name="rangeBoundary"> The range boundary. </param>
		///
		/// <returns> true if it succeeds, false if it fails. </returns>
		///-------------------------------------------------------------------------------------------------

		public static bool Between(this IComparable value, IComparable comparator0, IComparable comparator1,
			RangeBoundaryType rangeBoundary = RangeBoundaryType.Inclusive)
		{
			if (value == null) return false;

			switch (rangeBoundary)
			{
				case RangeBoundaryType.Exclusive:
					return (value.CompareTo(comparator0) > 0 && value.CompareTo(comparator1) < 0);

				case RangeBoundaryType.Inclusive:
					return (value.CompareTo(comparator0) >= 0 && value.CompareTo(comparator1) <= 0);

				case RangeBoundaryType.InclusiveLowerBoundaryOnly:
					return (value.CompareTo(comparator0) >= 0 && value.CompareTo(comparator1) < 0);

				case RangeBoundaryType.InclusiveUpperBoundaryOnly:
					return (value.CompareTo(comparator0) > 0 && value.CompareTo(comparator1) <= 0);

				default:
					return false;
			}
		}
	}
}