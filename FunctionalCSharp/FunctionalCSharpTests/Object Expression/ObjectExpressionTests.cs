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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FunctionalCSharp;
using FunctionalCSharp.Tuples;
using NUnit.Framework;

namespace FunctionalCSharpTests
{
    [TestFixture]
    public class ObjectExpressionTests
    {
        public interface IFoo
        {
            void A(string x);
            string C();
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
            foo.C();
        }

        [Test]
        public void MethodImplementationReturnsString()
        {
            var foo = ObjectExpression.New<IFoo>()
                                      .With(o => o.C, () => "Hello")
                                      .Return();
            Assert.AreEqual("Hello", foo.C());
        }

        [Test]
        public void MethodTakesString()
        {
            string result = null;
            var foo = ObjectExpression.New<IFoo>()
                                      .With<string>(o => o.A, x => result = x)
                                      .Return();
            foo.A("Hello");
            Assert.AreEqual("Hello", result);
        }

        [Test]
        public void MethodTakes2Args()
        {         
            Tuple<int, string> result = null;
            var foo = ObjectExpression.New<IDictionary<int, string>>()
                                      .With(o => o.Add, (int key, string value) => { result = Tuple.New(key, value); })
                                      .Return();
            foo.Add(42, "Hello");
            Assert.AreEqual(Tuple.New(42, "Hello"), result);
        }

        [Test]
        public void NewInstanceThatImplementsInterface()
        {
            var obj = ObjectExpression
                .New<IDisposable>()
                .Return();
            Assert.IsInstanceOf<IDisposable>(obj);
        }

        [Test]
        public void DelegatesToMethod()
        {
            bool called = false;
            var obj = ObjectExpression
                .New<IDisposable>()
                .With(o => o.Dispose, () => { called = true; })
                .Return();
            obj.Dispose();
            Assert.IsTrue(called);
        }

        [Test]
        public void DelegatesToMethodThatReturnsAnObject()
        {
            var expectedObject = new object();
            var obj = ObjectExpression
                .New<ICloneable>()
                .With(o => o.Clone, () => expectedObject)
                .Return();
            Assert.AreSame(expectedObject, obj.Clone());
        }

        [Test]
        public void DelegatesToMethodThatReturnsAValueType()
        {            
            var obj = ObjectExpression
                .New<IList>()
                .With(o => () => o.Count, () => 42)
                .Return();
            Assert.AreEqual(42, obj.Count);
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
                                      .With(o => o.B<int>, () => 42)
                                      .Return();
            Assert.AreEqual(42, foo.B<int>());
        }

        [Test]
        public void MatchOnGenericTypeImplementation()
        {
            var foo = ObjectExpression.New<IFoo>()
                                      .With(o => o.B<int>, () => 42)
                                      .With(o => o.B<string>, () => "Hello World")
                                      .Return();
            Assert.AreEqual(42, foo.B<int>());
            Assert.AreEqual("Hello World", foo.B<string>());
        }

        [Test]
        public void ExampleOfSimpleIoCContainer()
        {
            var container = ObjectExpression
                .New<IoC_Container>()
                .With(o => o.Get<IFoo>, () => ObjectExpression.New<IFoo>().Return())
                .With(o => o.Get<IBar>, () => ObjectExpression.New<IBar>().Return())
                .Return();

            Assert.IsInstanceOf(typeof(IFoo), container.Get<IFoo>());
            Assert.IsInstanceOf(typeof(IBar), container.Get<IBar>());
        }

        [Test]
        public void ComparerTest()
        {
            var list = new SortedList<int, string>(
                ObjectExpression
                .New<IComparer<int>>()
                .With(o => o.Compare, (int x, int y) => x - y)
                .Return());

            list.Add(3, "l");
            list.Add(2, "e");
            list.Add(4, "l");
            list.Add(1, "H");
            list.Add(5, "o");

            Assert.AreEqual("Hello", list.Values.Aggregate((x, y) => x + y));                
        }

        [Test]
        public void AnonymousTypeParameter()
        {
            var foo = CreateAnonymous(new {Name="JohnDoe"});
            Assert.IsNotNull(foo);
        }

        public static IFoo<T> CreateAnonymous<T>(T t)
        {
            return ObjectExpression.New<IFoo<T>>().Return();
        }

        public interface IFoo<T>
        {}

        public interface IoC_Container
        {
            T Get<T>();
        }

        public interface IBar { }
    }
}
