using NUnit.Framework;
using FluentAssertions;
using System.Collections.Generic;

namespace Invisionware.Core.Tests
{
	public class StringExtensionsTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		[TestCaseSource(nameof(StringTestItems))]
		public void IsNullOrEmptyTest(string str)
		{
			str.IsNullOrEmpty();

			Assert.Pass();
		}

		[Test]
		[TestCaseSource(nameof(StringCompareTestItems))]
		public void CompareExTest(string str1, string str2, int expected)
		{
			str1.CompareEx(str2).Should().Be(expected);
		}

		[Test]
		[TestCaseSource(nameof(StringBase64TestItems))]
		public void ToBase64Test(string str, string strExpected)
		{
			str.ToBase64().Should().Be(strExpected);
		}

		public static IEnumerable<TestCaseData> StringTestItems()
		{
			yield return new TestCaseData("some string");
			yield return new TestCaseData("");
			yield return new TestCaseData(null);
		}

		public static IEnumerable<TestCaseData> StringCompareTestItems()
		{
			yield return new TestCaseData("str1", "str1", 0);
			yield return new TestCaseData("str2", "str1", 1);
			yield return new TestCaseData("str1", "str2", -1);
			yield return new TestCaseData("", "", -999);
			yield return new TestCaseData(null, null, -999);
			yield return new TestCaseData("", null, -999);
		}

		public static IEnumerable<TestCaseData> StringBase64TestItems()
		{
			yield return new TestCaseData("some string", "c29tZSBzdHJpbmc=");
			yield return new TestCaseData("", null);
			yield return new TestCaseData(null, null);
		}
	}
}