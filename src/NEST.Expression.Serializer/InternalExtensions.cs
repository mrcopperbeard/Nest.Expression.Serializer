using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NEST.Expression.Serializer
{
	internal static class InternalExtensions
	{
		public static void AppendLine(this StringBuilder sb, string input, int depth)
		{
			sb.Append(new string(' ', depth * 2));
			sb.AppendLine(input);
		}

		public static JsonBuilder WrapAction(
			this StringBuilder sb,
			Action fillInner)
		{
			return new JsonBuilder(sb).WrapAction(fillInner);
		}

		public static string GetPath(this MemberExpression e)
		{
			// TODO: CamelCase.
			var nodes = e.ToString().Split('.');
			return nodes.Length == 1
				? nodes[0]
				: string.Join(".", nodes.Skip(1));
		}
	}
}