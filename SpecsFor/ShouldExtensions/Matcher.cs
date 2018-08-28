﻿using System;
using System.Linq.Expressions;

namespace SpecsFor.ShouldExtensions
{
	public class Matcher
	{
		[ThreadStatic]
		public static Matcher LastMatcher;

		public static void Create<T>(Expression<Func<T, bool>> matcher, string message)
		{
			LastMatcher = new Matcher<T>(matcher, message);
		}
	}

	public class Matcher<T> : Matcher
	{
		private readonly Expression<Func<T, bool>> _matcher;
		private readonly string _message;

		public Matcher(Expression<Func<T, bool>> matcher, string message)
		{
			_matcher = matcher ?? (x => true);
			_message = message;
		}

		public override bool Equals(object obj)
		{
			if (!ObjectIsCompatibleWithType(obj))
			{
				return false;
			}

			var matcher = _matcher.Compile();

			return matcher((T)obj);
		}

	    protected bool Equals(Matcher<T> other)
	    {
	        return Equals(_matcher, other._matcher) && string.Equals(_message, other._message);
	    }

	    public override int GetHashCode()
	    {
	        unchecked
	        {
	            return ((_matcher != null ? _matcher.GetHashCode() : 0) * 397) ^ (_message != null ? _message.GetHashCode() : 0);
	        }
	    }

	    private static bool ObjectIsCompatibleWithType(object obj)
		{
			if (obj is T || obj == null) return true;

			//if (typeof (T).IsNullable() && (typeof (T).GetInnerTypeFromNullable()) == obj.GetType())
			//	return true;

			return false;
		}

		public override string ToString()
		{
			return _message;
		}
	}
}