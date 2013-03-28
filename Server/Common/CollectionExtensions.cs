using System;
using System.Collections.Generic;

namespace Vre.Server
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
    }
}