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
using FunctionalCSharp;
using NUnit.Framework;

namespace FunctionalCSharpTests
{
    [TestFixture]
    public class ActivePatternsTests
    {
         [Test]
        public void MatchExpressionWithActivePatterns()
        {
            Expression<Func<int, int, int, int>> f = (x, y, z) => (x + y) * z;

            Func<Expression, string> toString = null;
            toString = exp =>
                       exp.Match()
                           .Lambda((args, body) => toString(body))
                           .Param ((name)       => name)
                           .Add   ((l, r)       => String.Format("({0} + {1})", toString(l), toString(r)))
                           .Mult  ((l, r)       => String.Format("{0} * {1}", toString(l), toString(r)))
                           .Return<string>();

            Assert.AreEqual("(x + y) * z", toString(f));
        }
    }
}