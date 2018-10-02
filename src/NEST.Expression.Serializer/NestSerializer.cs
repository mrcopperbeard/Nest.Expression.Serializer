using System;
using System.Linq.Expressions;
using System.Text;

namespace NEST.Expression.Serializer
{
	public class NestSerializer
	{
		public static string Serialize<T>(Expression<Func<T, bool>> e)
		{
			var sb = new StringBuilder();
			sb.AppendLine("{");
			VisitExpression(e, sb, 1);
			sb.AppendLine("}");

			return sb.ToString();
		}

		public static string Probe<T>(Expression<Func<T, bool>> e)
		{
			var sb = new StringBuilder();
			ProbeExpression(e, sb);

			return sb.ToString();
		}

		private static void ProbeExpression(System.Linq.Expressions.Expression e, StringBuilder sb)
		{
			sb.AppendLine($"{Enum.GetName(typeof(ExpressionType), e.NodeType)}: {e.GetType().Name}");
			switch (e.NodeType)
			{
				case ExpressionType.Lambda:
					ProbeExpression((e as LambdaExpression)?.Body, sb);
					break;
				case ExpressionType.AndAlso:
				case ExpressionType.Equal:
					ProbeExpression((e as BinaryExpression)?.Left, sb);
					ProbeExpression((e as BinaryExpression)?.Right, sb);
					break;
				case ExpressionType.MemberAccess:
					var propertyName = (e as MemberExpression)?.Member.Name;
					sb.AppendLine($"Member: {propertyName}");
					break;
				case ExpressionType.Constant:
					var value = (e as ConstantExpression)?.Value;
					sb.AppendLine($"Value: {value}");
					break;
			}
		}

		private static void VisitExpression(
			System.Linq.Expressions.Expression e,
			StringBuilder sb,
			int depth)
		{
			switch (e.NodeType)
			{
				case ExpressionType.Lambda:
					sb
						.WrapAction(() => VisitExpression((e as LambdaExpression)?.Body, sb, depth))
						.OnDepth(depth)
						.InElement("query")
						.InElement("bool")
						.InArray("must")
						.InNewScope()
						.Build();

					break;
				case ExpressionType.AndAlso:
					sb
						.WrapAction(() =>
						{
							VisitExpression((e as BinaryExpression)?.Left, sb, depth + 1);
							sb.Append(",");
							VisitExpression((e as BinaryExpression)?.Right, sb, depth + 1);
						})
						.OnDepth(depth)
						.InElement("bool")
						.Build();
					break;
				case ExpressionType.OrElse:
					sb
						.WrapAction(() =>
						{
							VisitExpression((e as BinaryExpression)?.Left, sb, depth + 1);
							sb.Append(",");
							VisitExpression((e as BinaryExpression)?.Right, sb, depth + 1);
						})
						.OnDepth(depth)
						.InElement("bool")
						.Build();
					break;
				case ExpressionType.Equal:
					sb
						.WrapAction(() => AppendTerm(e as BinaryExpression, sb, depth + 1))
						.OnDepth(depth)
						.InArray("must")
						.InNewScope()
						.Build();
					break;
				case ExpressionType.NotEqual:
					sb
						.WrapAction(() => AppendTerm(e as BinaryExpression, sb, depth + 1))
						.OnDepth(depth)
						.InArray("must_not")
						.InNewScope()
						.Build();
					break;
				case ExpressionType.MemberAccess:
					AppendTerm(e as MemberExpression, sb, depth);
					break;
			}
		}

		private static void AppendTerm(
			MemberExpression e,
			StringBuilder sb,
			int depth)
		{
			var propertyPath = e.GetPath();
			AppendTerm(propertyPath, true, sb, depth);
		}

		private static void AppendTerm(
			BinaryExpression e,
			StringBuilder sb,
			int depth)
		{
			var member =
				e.Left as MemberExpression
				?? e.Right as MemberExpression
				?? throw new InvalidOperationException("Can't find member exception.");

			var constant =
				e.Right as ConstantExpression
				?? e.Left as ConstantExpression
				?? throw new InvalidOperationException("Can't find constant exception.");

			var value = constant.Value;
			var propertyPath = member.GetPath();

			AppendTerm(propertyPath, value, sb, depth);
		}

		private static void AppendTerm(string path, object value, StringBuilder sb, int depth)
		{
			string formatted;
			if (value == null)
			{
				formatted = "null";
			}
			else if (value is bool)
			{
				formatted = value.ToString().ToLowerInvariant();
			}
			else if (value is ValueType)
			{
				formatted = value.ToString();
			}
			else
			{
				formatted = $"\"{value}\"";
			}

			sb
				.WrapAction(() => sb.AppendLine($"\"value\": {formatted}", depth))
				.OnDepth(depth)
				.InElement("term")
				.InElement(path)
				.Build();
		}
	}
}
