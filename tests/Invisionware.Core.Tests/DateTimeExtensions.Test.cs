using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Invisionware.Core.Tests
{
	public class DateTimeExtensionsTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void UnixTimeTest()
		{
			Assert.Pass();
		}

		[Test]
		[TestCaseSource(nameof(DateTimeTestItems))]
		public void SinceUnixTimeDateTimeTest(DateTime item)
		{
			Assert.Pass();
		}

		[Test]
		[TestCaseSource(nameof(DateTimeOffsetTestItems))]
		public void SinceUnixTimeDateTimeOffSetTest(DateTimeOffset item)
		{
			Assert.Pass();
		}


		[Test]
		[TestCaseSource(nameof(TimeSpanTestItems))]
		public void FullMillisecondsTest(TimeSpan item)
		{
			Assert.Pass();
		}

		[Test]
		[TestCaseSource(nameof(DateTimeTestItems))]
		public void ToUniversalTime(DateTime item)
		{
			Assert.Pass();
		}

		[Test]
		[TestCaseSource(nameof(DateTimeTestItems))]
		public void ToStringTest(DateTime item)
		{
			Assert.Pass();
		}

		[Test]
		[TestCaseSource(nameof(DateTimeTestItems))]
		public void AddDaysTest(DateTime item)
		{
			Assert.Pass();
		}

		public static IEnumerable<TestCaseData> DateTimeTestItems()
		{
			yield return new TestCaseData(DateTime.Now);
			yield return new TestCaseData(null);
		}

		public static IEnumerable<TestCaseData> TimeSpanTestItems()
		{
			yield return new TestCaseData(new TimeSpan(DateTime.Now.Ticks));
			yield return new TestCaseData(null);
		}

		public static IEnumerable<TestCaseData> DateTimeOffsetTestItems()
		{
			yield return new TestCaseData(new DateTimeOffset(DateTime.Now.AddYears(-1)));
			yield return new TestCaseData(null);
		}
	}
}