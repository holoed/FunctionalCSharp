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

using System;
using System.Linq.Expressions;

namespace FunctionalCSharp
{
    public static class ObjectExpression
    {
        public static ObjectExpression<T> New<T>()
        {
            return new ObjectExpression<T>();
        }
    }

    public class ObjectExpression<T>
    {
        private readonly ObjectBuilder _decorator;

        public ObjectExpression()
        {
            _decorator = ObjectBuilder.New();
        }

        public ObjectExpression<T> With(Expression<Func<T, Action>> method, Action handler)
        {
            _decorator.AddMethod(method, handler);
            return this;
        }

        public ObjectExpression<T> With<T1>(Expression<Func<T, Action<T1>>> method, Action<T1> handler)
        {
            _decorator.AddMethod(method, handler);
            return this;
        }

        public ObjectExpression<T> With<T1>(Expression<Func<T, Func<T1>>> method, Func<T1> handler)
        {
            _decorator.AddMethod(method, handler);
            return this;
        }

        public T Return()
        {
            return _decorator.Return<T>();
        }
    }
}