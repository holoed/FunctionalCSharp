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
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace FunctionalCSharp
{
    public class ObjectBuilder
    {     
        private static AssemblyBuilder _assemblyBuilder;
        private static ModuleBuilder _module;
        private TypeBuilder _typeBuilder;
        private string _generatedTypeName;
        private static int _id;
        private MethodBuilder _methodBuilder;
        private FieldBuilder _methodsField;
        private readonly IDictionary<string, Delegate> _methodsDictionary = new Dictionary<string, Delegate>();
        private static readonly IDictionary<TypeKey, Type> TypeCache = new Dictionary<TypeKey, Type>();

        private class TypeKey
        {
            private readonly string _key;

            public static TypeKey Create<T>(IEnumerable<KeyValuePair<string, Delegate>> methods)
            {
                return new TypeKey(typeof(T), methods);
            }

            private TypeKey(Type type, IEnumerable<KeyValuePair<string, Delegate>> methods)
            {
                _key = type.FullName + methods.Aggregate(" ", (x, y) => x + " " + y.Key);
            }

            public override int GetHashCode()
            {
                return _key.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                var other = obj as TypeKey;
                if (other == null) return false;
                return _key == other._key;
            }

            public override string ToString()
            {
                return _key;
            }
        }

        static ObjectBuilder()
        {
            SetupBuilderAndModule("Decorators");
        }

        private ObjectBuilder()
        {}

        public static ObjectBuilder New()
        {
            return new ObjectBuilder();
        }

        public void AddMethod(MethodInfo info, Delegate value)
        {
            var name = info.Name;
            if (name.StartsWith("get_") &&
                value.Method.GetParameters().Select(p => p.ParameterType).Contains(info.ReturnType))
                name = name.Replace("get_", "set_");
            _methodsDictionary.Add(GetMethodKey(info, name), value);
            return;
        }

        private static string GetMethodKey(MethodInfo info, string name)
        {
            if (info.IsGenericMethod)
                name = String.Format("{0}{1}", name,
                                     info.GetGenericArguments().Select(t => t.FullName).Aggregate(
                                         (x, y) => String.Format("{0}{1}", x, y)));
            return name;
        }

        public T Return<T>()
        {
            return (T)Activator.CreateInstance(CreateType<T>(), _methodsDictionary);
        }

        private static void SetupBuilderAndModule(string moduleName)
        {
            var assemblyName = new AssemblyName(moduleName);
            _assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            _module = _assemblyBuilder.DefineDynamicModule(moduleName, moduleName + ".dll");
        }

        private Type CreateType<T>()
        {
            var typeKey = TypeKey.Create<T>(_methodsDictionary);
            if (!TypeCache.ContainsKey(typeKey))
            {
                EmitEmptyType<T>();
                EmitConstructor();
                EmitMethods<T>();
                TypeCache.Add(typeKey, _typeBuilder.CreateType());
            }

            return TypeCache[typeKey];
        }

        private void EmitConstructor()
        {
            var constructorBuilder = _typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new[] { _methodsDictionary.GetType() });
            constructorBuilder.DefineParameter(1, ParameterAttributes.In, "methods");

            var gen = constructorBuilder.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stfld, _methodsField);
            gen.Emit(OpCodes.Ret);
        }

        private void EmitMethods<T>()
        {
            var interfaceType = typeof(T);
            var methods = GetAllMethods(interfaceType);
            foreach (var method in methods)
                CreateMethod(method);
            foreach (var method in typeof(object).GetMethods().Where(HasLambdaToDelegateTo))
                CreateMethod(method);
        }

        private static IEnumerable<MethodInfo> GetAllMethods(Type interfaceType)
        {
            var interfaces = interfaceType.GetInterfaces();
            if (interfaces != null)
                foreach (var method in interfaces.SelectMany(i => i.GetMethods()))
                    yield return method;
            foreach (var method in interfaceType.GetMethods())
                yield return method;
        }

        private void CreateMethod(MethodInfo method)
        {
            var attributes = method.Attributes;
            attributes &= ~MethodAttributes.Abstract;
            _methodBuilder = _typeBuilder.DefineMethod(
                method.Name,
                attributes,
                CallingConventions.Standard,
                method.ReturnType,
                method
                    .GetParameters()
                    .Select(p => p.ParameterType).ToArray());

            if (method.IsVirtual && !method.IsAbstract)
                _typeBuilder.DefineMethodOverride(_methodBuilder, method);

            if (method.ContainsGenericParameters)
                _methodBuilder.DefineGenericParameters(method.GetGenericArguments().Select(type => type.Name).ToArray());

            CreateMethodBody(method);
        }

        private void CreateMethodBody(MethodInfo interfaceMethod)
        {
            var il = _methodBuilder.GetILGenerator();
            if (HasLambdaToDelegateTo(interfaceMethod))
                DelegateToLambda(il, interfaceMethod);
            else
                il.ThrowException(typeof (NotImplementedException));
        }

        private bool HasLambdaToDelegateTo(MethodInfo method)
        {
            if (method.IsGenericMethod)
                return true;
            return _methodsDictionary.ContainsKey(GetMethodKey(method, method.Name));
        }       

        private void DelegateToLambda(ILGenerator il, MethodInfo method)
        {
            var listLocal = il.DeclareLocal(typeof(List<object>));
            var argsLocal = il.DeclareLocal(typeof(object[]));
            il.Emit(OpCodes.Newobj, typeof(List<object>).GetConstructor(new Type[0]));
            il.Emit(OpCodes.Stloc, listLocal);
            var parameters = method.GetParameters();
            for (var i = 0; i < parameters.Length; i++)
            {
                il.Emit(OpCodes.Ldloc, listLocal);
                il.Emit(OpCodes.Ldarg, i + 1);
                if (parameters[i].ParameterType.IsValueType)
                    il.Emit(OpCodes.Box, parameters[i].ParameterType);
                il.Emit(OpCodes.Callvirt, typeof(List<object>).GetMethod("Add"));
            }
            il.Emit(OpCodes.Ldloc, listLocal);
            il.Emit(OpCodes.Callvirt, typeof(List<object>).GetMethod("ToArray"));
            il.Emit(OpCodes.Stloc, argsLocal);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, _methodsField);
            il.Emit(OpCodes.Ldstr, method.Name);
            if (_methodBuilder.IsGenericMethod)
            {
                il.Emit(OpCodes.Ldtoken, method.GetGenericArguments()[0]);
                il.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
                il.Emit(OpCodes.Callvirt, typeof(Type).GetProperty("FullName").GetGetMethod());
                il.Emit(OpCodes.Call, typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }));
            }
            il.Emit(OpCodes.Callvirt, typeof(Dictionary<string, Delegate>).GetProperty("Item").GetGetMethod());

            il.Emit(OpCodes.Ldloc, argsLocal);
            il.Emit(OpCodes.Callvirt, typeof(Delegate).GetMethod("DynamicInvoke"));
            if (method.ReturnType == typeof(void))
                il.Emit(OpCodes.Pop);
            else if (method.ReturnType.IsValueType || method.ReturnType.IsGenericParameter)
                il.Emit(OpCodes.Unbox_Any, method.ReturnType);

            il.Emit(OpCodes.Ret);
        }

        private void EmitEmptyType<T>()
        {
            _generatedTypeName = BuildDynamicName<T>();
            _typeBuilder = _module.DefineType(_generatedTypeName, TypeAttributes.Public | TypeAttributes.Class);
            _typeBuilder.AddInterfaceImplementation(typeof(T));
            _methodsField = _typeBuilder.DefineField("methods", _methodsDictionary.GetType(), FieldAttributes.Private);
        }

        private static string BuildDynamicName<T>()
        {
            return typeof(T).Name + "Emitted" + _id++;
        }

    }
}
