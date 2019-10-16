using FluentAssertions;
using NUnit.Framework;

namespace Invisionware.Core.Tests
{
	public class TypeExtensionsTests
	{
		[SetUp]
		public void Setup()
		{
		}

		//[Test]
		//public void GetAttributeValueTest()
		//{
		//	var c = new CClass1();
		//	var t = c.GetType();

		//}

		[Test]
		public void GetAttributeOfTypeTest()
		{
			var c = new CClass1();
			var t = c.GetType();

			var results = t.GetAttributeOfType<Attribute1>(true);

			results.Should().NotBeNullOrEmpty();
			results.Should().HaveCountGreaterOrEqualTo(1);
		}

		[Test]
		public void GetPropertiesWithAttributeTest()
		{
			var c = new CClass1();
			var t = c.GetType();

			var results = t.GetPropertiesWithAttribute<Attribute1>(true);

			results.Should().NotBeNullOrEmpty();
			results.Should().HaveCountGreaterOrEqualTo(2);
		}

		[Test]
		public void AttributeExistsTest()
		{
			var c = new CClass1();
			var t = c.GetType();

			t.AttributeExists<Attribute1>().Should().BeTrue();
		}

		public void GetPropertiesTest()
		{
			var c = new CClass1();
			var t = c.GetType();

			t.GetProperties().Should().NotBeNull();
		}
	}
}