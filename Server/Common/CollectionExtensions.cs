using System;
using System.Collections.Generic;
using System.Text;

namespace Vre
{
    public static class CollectionExtensions
    {
        public static void Intersect<X>(ICollection<X> left, ICollection<X> right,
            out ICollection<X> leftUnique, out ICollection<X> rightUnique)
        {
            leftUnique = new List<X>(left);
            rightUnique = new List<X>(right);
            foreach (X e in left)
                if (right.Contains(e)) { leftUnique.Remove(e); rightUnique.Remove(e); }
        }

        public static IEnumerable<Y> ConvertTo<X, Y>(this IEnumerable<X> coll, Converter<X, Y> converter)
        {
            if (null == coll) throw new ArgumentNullException("Collection");
            if (null == converter) throw new ArgumentNullException("Converter");
            foreach (X o in coll) yield return converter(o);
        }

		public static string ToString<T>(this IEnumerable<T> coll, char separator)
		{
			StringBuilder result = new StringBuilder();
			bool sep = false;
			foreach (var item in coll)
			{
				if (sep) result.Append(separator);
				result.Append(item.ToString());
				sep = true;
			}
			return result.ToString();
		}

		public static void Foreach<T>(this IEnumerable<T> coll, Action<T> op)
		{
			foreach (T e in coll) op(e);
		}

		public static bool Compare(this byte[] left, byte[] right)
		{
			if (null == left)
			{
				if (null == right) return true;
				else return false;
			}
			else if (null == right)
			{
				return false;
			}
			else
			{
				if (left.Length != right.Length) return false;
				for (int idx = left.Length - 1; idx >= 0; idx--)
					if (left[idx] != right[idx]) return false;
				return true;
			}
		}

		public static long IndexOfPattern(this byte[] buffer, byte[] pattern, long startPos)
		{
			return IndexOfPattern(buffer, pattern, startPos, buffer.Length);
		}

		public static long IndexOfPattern(this byte[] buffer, byte[] pattern, long startPos, long limit)
		{
			// TODO: Implement http://en.wikipedia.org/wiki/Boyer%E2%80%93Moore_string_search_algorithm
			int plen = pattern.Length;
			long blen = limit - plen;
			for (long result = startPos; result < blen; result++)
			{
				int idx = plen;
				for (idx--; idx >= 0; idx--)
					if (buffer[result + idx] != pattern[idx]) break;
				if (idx < 0) return result;
			}
			return -1;
		}
	}
}