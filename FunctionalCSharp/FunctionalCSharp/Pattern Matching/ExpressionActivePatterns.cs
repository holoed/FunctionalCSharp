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
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;

namespace FunctionalCSharp
{
    public static class ExpressionActivePatterns
    {
        public static PatternMatching<T> Lambda<T>(this PatternMatching<T> x, Func<IEnumerable<ParameterExpression>, Expression, object> f)
        {
            return x.With<LambdaExpression>(p => f(p.Parameters, p.Body));
        }

        public static PatternMatching<T> Unary<T>(this PatternMatching<T> x, Func<Expression, object> f)
        {
            return x.With<UnaryExpression>(p => f(p.Operand));
        }

        public static PatternMatching<T> MethodCall<T>(this PatternMatching<T> x, Func<MethodInfo, IList<Expression>, object> f)
        {
            return x.With<MethodCallExpression>(p => f(p.Method, p.Arguments));
        }

        public static PatternMatching<T> Param<T>(this PatternMatching<T> x, Func<string, object> f)
        {
            return x.With<ParameterExpression>(p => f(p.Name));
        }

        public static PatternMatching<T> Const<T>(this PatternMatching<T> x, Func<object, object> f)
        {
            return x.With<ConstantExpression>(c => f(c.Value));
        }

        public static PatternMatching<T> Add<T>(this PatternMatching<T> x, Func<Expression, Expression, object> f)
        {
            return x.With<BinaryExpression>(b => b.NodeType == ExpressionType.Add, b => f(b.Left, b.Right));
        }

        public static PatternMatching<T> Mult<T>(this PatternMatching<T> x, Func<Expression, Expression, object> f)
        {
            return x.With<BinaryExpression>(b => b.NodeType == ExpressionType.Multiply, b => f(b.Left, b.Right));
        }
    }
}