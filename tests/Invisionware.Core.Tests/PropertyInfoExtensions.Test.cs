using FluentAssertions;
using NUnit.Framework;

namespace Invisionware.Core.Tests
{
	public class PropertyInfoExtensionsTests
	{
		[SetUp]
		public void Setup()
		{
		}

		//[Test]
		//public void GetAttributeValueTest()
		//{
		//	var c = new CClass1();
		//	var property = c.GetType().GetProperty("StringValue");

		//	property.GetAttributeValue<Attribute1, string>().Should().BeEqual("String Value Property on Class");
		//}

		[Test]
		public void GetAttributeOfTypeTest()
		{
			var c = new CClass1();
			var t = c.GetType();
			var property = t.GetProperty("StringValue");

			var results = property.GetAttributeOfType<Attribute1>(true);

			results.Should().NotBeNullOrEmpty();
			results.Should().HaveCountGreaterOrEqualTo(1);
		}

		[Test]
		public void AttributeExistsTest()
		{
			var c = new CClass1();
			var t = c.GetType();
			var property = t.GetProperty("StringValue");

			property.AttributeExists<Attribute1>().Should().BeTrue();
		}
	}
}