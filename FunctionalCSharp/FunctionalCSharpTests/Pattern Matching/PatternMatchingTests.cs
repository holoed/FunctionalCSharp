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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FunctionalCSharp;
using NUnit.Framework;

namespace FunctionalCSharpTests
{
    [TestFixture]
    public class PatternMatchingTests
    {
        [Test]
        public void MatchLambdaRetValue()
        {
            Expression<Func<int>> f = () => 5;

            Func<Expression, int> getValue = null;
            getValue = exp =>
                exp.Match()
                   .With<LambdaExpression>(l => getValue(l.Body))
                   .With<ConstantExpression>(c => c.Value)
                   .Return<int>();

            Assert.AreEqual(5, getValue(f));
        }

        [Test]
        public void MatchUnaryOp()
        {
            bool x = true;
            Expression<Func<bool>> f = () => !x;

            Func<Expression, string> toString = null;
            toString = exp =>
                exp.Match()
                   .With<LambdaExpression>(l => toString(l.Body))
                   .With<MemberExpression>(m => m.Member.Name)
                   .With<UnaryExpression>(u => String.Format("{0} {1}", u.NodeType, toString(u.Operand)))
                   .Return<string>();

            Assert.AreEqual("Not x", toString(f));
        }

        [Test]
        public void MatchLambdaRetMemberAccess()
        {
            var hello = 0;
            Expression<Func<int>> f = () => hello;

            Func<Expression, string> getMember = null;
            getMember = exp =>
                exp.Match()
                   .With<LambdaExpression>(l => getMember(l.Body))
                   .With<MemberExpression>(m => m.Member.Name)
                   .Return<string>();

            Assert.AreEqual("hello", getMember(f));
        }


        [Test]
        public void MatchAdd()
        {
            var Operator = new Dictionary<ExpressionType, string> { { ExpressionType.Add, "+" } };

            Expression<Func<int,int,int>> add = (x,y) => x + y;

            Func<Expression, string> toString = null;
            toString = exp =>
                exp.Match()
                   .With<LambdaExpression>(l => toString(l.Body))
                   .With<ParameterExpression>(p => p.Name)
                   .With<BinaryExpression>(b => String.Format("{0} {1} {2}", toString(b.Left), Operator[b.NodeType], toString(b.Right)))
                   .Return<string>();

            Assert.AreEqual("x + y", toString(add));
        }

        [Test]
        public void MatchIncrement()
        {
            var Operator = new Dictionary<ExpressionType, string> { { ExpressionType.Add, "+" } };

            Expression<Func<int, int>> inc = n => n + 1;

            Func<Expression, string> toString = null;
            toString = exp =>
                exp.Match()
                   .With<LambdaExpression>(l => toString(l.Body))
                   .With<ParameterExpression>(p => p.Name)
                   .With<ConstantExpression>(c => c.Value.ToString())
                   .With<BinaryExpression>(b => String.Format("{0} {1} {2}", toString(b.Left), Operator[b.NodeType], toString(b.Right)))
                   .Return<string>();

            Assert.AreEqual("n + 1", toString(inc));
        }

        [Test]
        public void MatchAddAndMultiply()
        {
            var Operator = new Dictionary<ExpressionType, string> { { ExpressionType.Add, "+" }, { ExpressionType.Multiply, "*" } };

            Expression<Func<int, int, int, int>> add = (x, y, z) => (x + y) * z;

            Func<Expression, string> toString = null;
            toString = exp =>
                exp.Match()
                   .With<LambdaExpression>(l => toString(l.Body))
                   .With<ParameterExpression>(p => p.Name)
                   .With<BinaryExpression>(b => String.Format("({0} {1} {2})", toString(b.Left), Operator[b.NodeType], toString(b.Right)))
                   .Return<string>();

            Assert.AreEqual("((x + y) * z)", toString(add));
        }

        [Test]
        public void MatchNewExpression()
        {
            Expression<Func<object>> newExp = () => new {FirstName = "Edmondo", LastName = "Pentangelo"};

            Func<Expression, string> toString = null;
            toString = exp =>
                       exp.Match()
                           .With<LambdaExpression>(l => toString(l.Body))
                           .With<ConstantExpression>(c => c.Value.ToString())
                           .With<NewExpression>(p => p.Arguments
                                                      .Select(x => toString(x))
                                                      .Aggregate((x, y) => x + " " + y))
                           .Return<string>();

            Assert.AreEqual("Edmondo Pentangelo", toString(newExp));
        }

        [Test]
        public void MatchConditional()
        {
            var Operator = new Dictionary<ExpressionType, string> { { ExpressionType.GreaterThan, ">" } };
            Expression<Func<int, string>> f = n => n > 0 ? "A" : "B";

            Func<Expression, string> toString = null;
            toString = exp =>
                       exp.Match()
                           .With<LambdaExpression>(l => toString(l.Body))
                           .With<ParameterExpression>(p => p.Name)
                           .With<ConstantExpression>(c => c.Value.ToString())
                           .With<BinaryExpression>(b => String.Format("{0} {1} {2}", toString(b.Left), Operator[b.NodeType], toString(b.Right)))
                           .With<ConditionalExpression>(c => String.Format("{0} ? {1} : {2}", toString(c.Test), toString(c.IfTrue), toString(c.IfFalse)))
                           .Return<string>();

            Assert.AreEqual("n > 0 ? A : B", toString(f));
        }
    }   
}