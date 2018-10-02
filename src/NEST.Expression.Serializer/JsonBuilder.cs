using System;
using System.Collections.Generic;
using System.Text;

namespace NEST.Expression.Serializer
{
	internal class JsonBuilder
	{
		private readonly StringBuilder _sb;

		private readonly List<ElementDescription> _suffixes;

		private int _depth;

		private Action _action;

		public JsonBuilder(StringBuilder sb)
		{
			_sb = sb;
			_depth = 0;
			_suffixes = new List<ElementDescription>();
		}

		public JsonBuilder WrapAction(Action action)
		{
			_action = action;

			return this;
		}

		public JsonBuilder OnDepth(int depth)
		{
			_depth = depth;

			return this;
		}

		public JsonBuilder InElement(string elementName)
		{
			_suffixes.Add(new ElementDescription($"\"{elementName}\": {{", "}"));

			return this;
		}

		public JsonBuilder InArray(string elementName)
		{
			_suffixes.Add(new ElementDescription($"\"{elementName}\": [", "]"));

			return this;
		}

		public JsonBuilder InNewScope()
		{
			_suffixes.Add(new ElementDescription("{", "}"));

			return this;
		}

		public void Build()
		{
			var postfixBuilder = new StringBuilder();
			var count = _suffixes.Count;
			for (var i = 0; i < count; i++)
			{
				_sb.AppendLine(_suffixes[i].Prefix, _depth + i + 1);
				postfixBuilder.AppendLine(_suffixes[count - i -1].Postfix, _depth + count - i);
			}

			_action();
			_sb.AppendLine(postfixBuilder.ToString());
		}

		private class ElementDescription
		{
			public ElementDescription(string prefix, string postfix)
			{
				Prefix = prefix;
				Postfix = postfix;
			}

			public string Prefix { get; }
			public string Postfix { get; }
		}
	}
}