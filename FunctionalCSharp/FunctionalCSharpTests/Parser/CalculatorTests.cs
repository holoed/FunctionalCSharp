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
using FunctionalCSharp.Parser;
using NUnit.Framework;
using FsCheck;

namespace FunctionalCSharpTests.Parser
{
    [TestFixture]
    public class CalculatorTests
    {
        [Test]
        public void IdentityElement()
        {
            QuickCheck(valueOf => valueOf("0 + {1}") == valueOf("{1} + 0"));
            QuickCheck(valueOf => valueOf("1 * {1}") == valueOf("{1} * 1"));     
        }

        [Test]
        public void Commutativity()
        {
            QuickCheck(valueOf => valueOf("{0} + {1}") == valueOf("{1} + {0}"));
            QuickCheck(valueOf => valueOf("{0} * {1}") == valueOf("{1} * {0}"));          
        }

        [Test]
        public void Associativity()
        {
            QuickCheck(valueOf => valueOf("{0} + ( {1} + {2} )") == valueOf("( {0} + {1} ) + {2}"));
            QuickCheck(valueOf => valueOf("{0} * ( {1} * {2} )") == valueOf("( {0} * {1} ) * {2}"));
        }

        [Test]
        public void Distributivity()
        {
            QuickCheck(valueOf => valueOf("{0} * ( {1} + {2} )") == valueOf("{0} * {1} + {0} * {2}"));
            QuickCheck(valueOf => valueOf("{0} * ( {1} + {2} )") == valueOf("{0} * {1} + {0} * {2}"));
        }

        [Test]
        public void SquareIsNeverNegative()
        {
            QuickCheck(valueOf => valueOf("{0} * {1}") >= 0);         
        }

        private static void QuickCheck(Func<Func<string, int>, bool> property)
        {
            Spec.ForAny(Calculate(property)).Check(FsNUnit.Config);
        }

        private static Func<int, bool> Calculate(Func<Func<string, int>, bool> check)
        {
            var rnd = new System.Random();
            return x =>
            {         
                var xs = Enumerable.Range(0, 3).Select(_ => rnd.Next(100)).ToArray();
                return check(exp => Calculate(String.Format(exp, xs.Cast<object>().ToArray())));
            };
        }

        private static int Calculate(IEnumerable<char> exp)
        {
            int result = CalculatorCombinators.Calculator().Execute(exp).First();
            return result;
        }
    }
}
