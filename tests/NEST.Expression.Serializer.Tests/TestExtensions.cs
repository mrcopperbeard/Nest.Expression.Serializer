using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace NEST.Expression.Serializer.Tests
{
	public static class TestExtensions
	{
		private static readonly Regex EntryRegex = new Regex(@"[^\""\""{}:\[\] ,\r\n]+");

		public static AndConstraint<StringAssertions> ContainInOrder(
			this StringAssertions assertions,
			params string[] expectedEntries)
		{
			var existingEntries = new List<string>();
			foreach (Match match in EntryRegex.Matches(assertions.Subject))
			{
				existingEntries.Add(match.Groups[0].Value);
			}

			existingEntries.Should().ContainInOrder(expectedEntries);

			return new AndConstraint<StringAssertions>(assertions);
		}

		public static AndConstraint<StringAssertions> BeValidJson(this StringAssertions assertions)
		{
			try
			{
				JObject.Parse(assertions.Subject);
			}
			catch (JsonReaderException e)
			{
				throw new AssertionException($"Seems we have an invalid JSON. {e.Message} Subject: {assertions.Subject}", e);
			}

			return new AndConstraint<StringAssertions>(assertions);
		}
	}
}