using System;
using System.Linq.Expressions;
using FluentAssertions;
using NUnit.Framework;

namespace NEST.Expression.Serializer.Tests
{
	[TestFixture]
	public class SerializationTests
	{
		[Test]
		public void SerializationTest()
		{
			const string Test = nameof(Test);
			Expression<Func<TestData, bool>> exp = data => data.StringProperty == "Test" && data.LongProperty == 42;

			var result = NestSerializer.Serialize(exp);

			//Console.Out.WriteLine(NestSerializer.Probe(exp));
			//Console.Out.WriteLine("==Result==");
			Console.Out.WriteLine(result);

			result
				.Should()
				.NotBeNullOrEmpty();
		}

		[Test]
		public void Serialize_GetBoolProperty_ShouldHaveCorrectPath()
		{
			NestSerializer
				.Serialize<TestData>(data => data.BoolProperty)
				.Should()
				.BeValidJson()
				.And
				.ContainInOrder("query", "bool", "must", "term", "BoolProperty");
		}

		[Test]
		public void Serialize_GetInnerProperty_ShouldHaveCorrectPath()
		{
			NestSerializer
				.Serialize<TestData>(data => data.Inner.Inner.BoolProperty)
				.Should()
				.BeValidJson()
				.And
				.ContainInOrder("term", "Inner.Inner.BoolProperty");
		}

		[Test]
		public void Serialize_Equal_ShouldHaveCorrectPath()
		{
			NestSerializer
				.Serialize<TestData>(data => data.StringProperty == "Test")
				.Should()
				.BeValidJson()
				.And
				.ContainInOrder("term", "StringProperty", "Test");
		}

		// TODO: Contains to "must" property in on "bool" object.
		[Test]
		public void Serialize_EqualAndEqual_ShouldHaveCorrectPath()
		{
			NestSerializer
				.Serialize<TestData>(data => data.StringProperty == "Test" && data.LongProperty == 42)
				.Should()
				.BeValidJson()
				.And
				.ContainInOrder("query", "bool", "must", "bool", "must", "term", "StringProperty", "Test", "term", "LongProperty", "42");
		}

		[Test]
		public void Serialize_EqualAndNotEqual_ShouldHaveCorrectPath()
		{
			NestSerializer
				.Serialize<TestData>(data => data.StringProperty == "Test" && data.LongProperty != 42)
				.Should()
				.BeValidJson()
				.And
				.ContainInOrder("query", "bool", "must", "bool", "must", "term", "StringProperty", "Test", "must_not", "term", "LongProperty", "42");
		}
	}
}