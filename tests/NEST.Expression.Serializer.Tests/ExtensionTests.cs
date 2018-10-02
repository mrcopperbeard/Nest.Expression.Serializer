using FluentAssertions;
using NUnit.Framework;

namespace NEST.Expression.Serializer.Tests
{
	[TestFixture]
	public class ExtensionTests
	{
		[TestCase("a, b, c, d, e", new[] { "b", "c", "d" })]
		[TestCase("a, b, c, d, e", new[] { "b", "d" })]
		public void ContainInOrder_Positive(string value, string[] expected)
		{
			value
				.Should()
				.ContainInOrder(expected);
		}

		[TestCase("a, b, c, d, e", new[] { "b", "d", "f" })]
		[TestCase("a, b, c, d, e", new[] { "b", "e", "d" })]
		public void ContainInOrder_Negative(string value, string[] expected)
		{
			value
				.Invoking(_ => _.Should().ContainInOrder(expected))
				.Should()
				.Throw<AssertionException>();
		}

		[TestCase("{\"a\": true}")]
		public void BeValidJson_Positive(string value)
		{
			value.Should().BeValidJson();
		}

		[TestCase("{\"a\": true}}")]
		[TestCase("{\"a\": true, \"a\": 42}")] // TODO: Fix
		public void BeValidJson_Negative(string value)
		{
			value
				.Invoking(_ => _.Should().BeValidJson())
				.Should()
				.Throw<AssertionException>();
		}
	}
}