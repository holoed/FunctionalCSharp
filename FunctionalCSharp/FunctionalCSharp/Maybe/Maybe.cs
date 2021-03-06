﻿#region License

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

namespace FunctionalCSharp
{
    public static class Maybe
    {
        public static Maybe<T> Some<T>(this T value)
        {
            return new Some<T>(value);
        }

        public static Maybe<T> None<T>()
        {
            return new None<T>();
        }
    }

    public abstract class Maybe<T>
    {
    }
}
