#region License

/* ****************************************************************************
 * Copyright (c) Edmondo Pentangelo. 
 *
 * This source code is subject to terms and conditions of the Microsoft Public License. 
 * A copy of the license can be found in the License.html file at the root of this distribution. 
 * By using this source code in any fashion, you are agreeing to be bound by the terms of the 
 * Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 * ***************************************************************************/

#endregion

using System;

namespace FunctionalCSharp.Tuples
{
    public interface Tuple<T1, T2>
    {
        T1 First { get; }
        T2 Second { get; }
    }

    public class Tuple
    {
        public static Tuple<T1, T2> New<T1,T2>(T1 t1, T2 t2)
        {
            return 
                ObjectExpression
                .New<Tuple<T1, T2>>()
                .With(o => () => o.First, () => t1)
                .With(o => () => o.Second, () => t2)
                .With(o => o.Equals, (object obj) =>
                                         {
                                             var other = obj as Tuple<T1, T2>;
                                             if (Equals(other, null)) return false;
                                             return Equals(other.First, t1) && Equals(other.Second, t2);
                                         })
                .With(o => o.ToString, () => String.Format("{0}, {1}", ToString(t1), ToString(t2)))
                .Return();
        }

        public static Tuple<T1, Tuple<T2, T3>> New<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            return New(t1, New(t2, t3));
                
        }

        private static string ToString<T>(T value)
        {
            return value
                .Match()
                .With<string>(x => String.Format("\"{0}\"", x))
                .Default(Convert.ToString)
                .Return<string>();
        }
    }
}
