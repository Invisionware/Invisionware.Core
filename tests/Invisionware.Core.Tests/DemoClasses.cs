using System;
using System.Collections.Generic;
using System.Text;

namespace Invisionware.Core.Tests
{
	[Attribute1(Name = "Interface Attribute")]
	public interface IInterface1
	{
		[Attribute1(Name = "Int Value Property on Interface")]
		int IntValue { get; set; }
		string StringValue { get; set; }
		bool BoolValue { get; set; }
	}

	public class CClass1 : IInterface1
	{
		public int IntValue { get; set; }
		[Attribute1(Name = "String Value Property on Class")]
		public string StringValue { get; set; }
		public bool BoolValue { get; set; }
	}

	public class Attribute1 : System.Attribute
	{
		public string Name { get; set; }
	}
}
