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
using FunctionalCSharp;
using FunctionalCSharp.DiscriminatedUnions;
using NUnit.Framework;

namespace FunctionalCSharpTests.DiscriminatedUnions
{
    [TestFixture]
    public class DiscriminatedUnionsTests
    {
        public interface Exp
        {
            Exp Var(string x);
            Exp Lam(string x, Exp e);
            Exp Let(string x, Exp e1, Exp e2);
            Exp App(Exp e1, Exp e2);
        }

        [Test]
        public void CreateDataTypeAndVisitUsingPatternMatching()
        {
            var exp = DataType.New<Exp>();

            exp = exp.Lam("x", exp.Var("x"));

            Func<Exp, string> toString = null;
            toString = expr => expr.Match()
                .With(o => o.Var, (string name) => name)
                .With(o => o.Lam, (string name, Exp e) => string.Format("fun {0} -> {1}", name, toString(e)))
                .Return<string>();

            Assert.AreEqual("fun x -> x", toString(exp));
        }

        [Test]
        public void CreateDataTypeAndVisitUsingPatternMatching2()
        {
            var exp = DataType.New<Exp>();

            exp = exp.Let("f", exp.Lam("x", exp.Var("x")), exp.Var("f"));                        

            Func<Exp, string> toString = null;
            toString = expr => expr.Match()
                .With(o => o.Var, (string x) => x)
                .With(o => o.Let, (string x, Exp e1, Exp e2) => string.Format("let {0} = {1} in {2}", x, toString(e1), toString(e2)))
                .With(o => o.Lam, (string x, Exp e) => string.Format("fun {0} -> {1}", x, toString(e)))
                .Return<string>();

            Assert.AreEqual("let f = fun x -> x in f", toString(exp));
        }

        [Test]
        public void CreateDataTypeAndVisitUsingPatternMatching3()
        {
            var exp = DataType.New<Exp>();

            exp = exp.Let("compose",
                                     exp.Lam("f",
                                             exp.Lam("g",
                                                     exp.Lam("x",
                                                             exp.App(exp.Var("g"), exp.App(exp.Var("f"), exp.Var("x")))))),
                                     exp.Var("compose"));

            Func<Exp, string> toString = null;
            toString = expr => expr.Match()
                .With(o => o.Var, (string x) => x)
                .With(o => o.Let, (string x, Exp e1, Exp e2) => string.Format("let {0} = {1} in {2}", x, toString(e1), toString(e2)))
                .With(o => o.Lam, (string x, Exp e) => string.Format("fun {0} -> {1}", x, toString(e)))
                .With(o => o.App, (Exp e1, Exp e2) => string.Format("({0} {1})", toString(e1), toString(e2)))
                .Return<string>();

            Assert.AreEqual("let compose = fun f -> fun g -> fun x -> (g (f x)) in compose", toString(exp));
        }

        [Test]
        public void Equality()
        {
            var exp = DataType.New<Exp>();
            Assert.AreEqual(exp.Var("x"), exp.Var("x"));
            Assert.AreNotEqual(exp.Var("x"), exp.Var("y"));
            Assert.AreNotEqual(exp.Var("x"), exp.Lam("y", exp.Var("x")));

            Assert.AreEqual(exp.Lam("x", exp.Var("x")), exp.Lam("x", exp.Var("x")));
            Assert.AreNotEqual(exp.Lam("x", exp.Var("x")), exp.Lam("x", exp.Var("y")));
            Assert.AreNotEqual(exp.Lam("x", exp.Var("x")), exp.Lam("y", exp.Var("x")));

            Assert.AreEqual(exp.App(exp.Var("x"), exp.Var("x")), exp.App(exp.Var("x"), exp.Var("x")));
            Assert.AreNotEqual(exp.App(exp.Var("x"), exp.Var("x")), exp.App(exp.Var("y"), exp.Var("x")));
        }
    }
}
