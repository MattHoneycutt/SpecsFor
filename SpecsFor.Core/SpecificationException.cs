using System;
using System.Text;

namespace SpecsFor.Core
{
	public class SpecificationException : ApplicationException
	{
		protected SpecificationException(string stage, Exception[] exceptions)
			: base("An error occurred during the spec '" + stage + "' phase. \r\n" + BuildMessage(exceptions))
		{
			Exceptions = exceptions;
		}

		private static string BuildMessage(Exception[] exceptions)
		{
			var result = new StringBuilder();

			foreach (var exception in exceptions)
			{
				result.AppendLine("--------------------------------------------------------------------");
				result.Append(exception).AppendLine();
			}

			return result.ToString();
		}

		public Exception[] Exceptions { get; private set; }

		public override string ToString()
		{
			var result = new StringBuilder();

			result.AppendLine(base.ToString());
			
			foreach (var exception in Exceptions)
			{
				result.AppendLine("--------------------------------------------------------------------");
				result.Append(exception).AppendLine();
			}

			return result.ToString();
		}
	}
}