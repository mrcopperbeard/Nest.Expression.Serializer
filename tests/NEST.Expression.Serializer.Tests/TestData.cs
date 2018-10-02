using System;

namespace NEST.Expression.Serializer.Tests
{
	public class TestData
	{
		public string StringProperty { get; set; }
		public DateTime DatetimeProperty { get; set; }
		public long LongProperty { get; set; }
		public bool BoolProperty { get; set; }
		public TestData Inner { get; set; }
	}
}