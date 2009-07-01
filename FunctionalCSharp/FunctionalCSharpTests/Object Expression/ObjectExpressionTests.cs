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
using NUnit.Framework;

namespace FunctionalCSharpTests
{
    [TestFixture]
    public class ObjectExpressionTests
    {
        public interface IFoo
        {
            void A(string x);
            string A();
            T B<T>();
        }

        [Test]
        public void Empty()
        {
            Assert.IsInstanceOf(typeof(IFoo), 
                ObjectExpression.New<IFoo>()
                                .Return());
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void NotImplemented()
        {
            var foo = ObjectExpression.New<IFoo>()
                                      .Return();
            foo.A();
        }

        [Test]
        public void MethodImplementationReturnsString()
        {
            var foo = ObjectExpression.New<IFoo>()
                                      .With(o => o.A(), _ => "Hello")
                                      .Return();
            Assert.AreEqual("Hello", foo.A());
        }

        [Test]
        public void MethodTakesString()
        {
            string result = null;
            var foo = ObjectExpression.New<IFoo>()
                                      .With<string>(o => o.A(null), x => result = x)
                                      .Return();
            foo.A("Hello");
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void GeneicMethodNotImplemented()
        {
            var foo = ObjectExpression.New<IFoo>()                                
                                      .Return();
            foo.B<int>();
        }

        [Test]
        public void GenericMethodImplementation()
        {
            var foo = ObjectExpression.New<IFoo>()
                                      .With(o => o.B<int>(), _ => 42)
                                      .Return();
            Assert.AreEqual(42, foo.B<int>());
        }

        [Test]
        public void MatchOnGenericTypeImplementation()
        {
            var foo = ObjectExpression.New<IFoo>()
                                      .With(o => o.B<int>(), _ => 42)
                                      .With(o => o.B<string>(), _ => "Hello World")
                                      .Return();
            Assert.AreEqual(42, foo.B<int>());
            Assert.AreEqual("Hello World", foo.B<string>());
        }

        public interface IoC_Container
        {
            T Get<T>();
        }

        public interface IBar {}

        [Test]
        public void ExampleOfSimpleIoCContainer()
        {
            var container = ObjectExpression
                .New<IoC_Container>()
                .With(o => o.Get<IFoo>(), _ => ObjectExpression.New<IFoo>().Return())
                .With(o => o.Get<IBar>(), _ => ObjectExpression.New<IBar>().Return())
                .Return();

            Assert.IsInstanceOf(typeof(IFoo), container.Get<IFoo>());
            Assert.IsInstanceOf(typeof(IBar), container.Get<IBar>());
        }
    }
}
