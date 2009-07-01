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
    internal class ObjectBuilder
    {
        protected TypeBuilder _typeBuilder;
        protected static ModuleBuilder _module;
        private static AssemblyBuilder _assemblyBuilder;
        private string _generatedTypeName;
        private static int _id;
        private ILGenerator _gen;
        private FieldBuilder _membersField;
        private MethodBuilder _methodBuilder;

        static ObjectBuilder()
        {
            SetupBuilderAndModule("ObjectExpressions");
        }

        private static void SetupBuilderAndModule(string moduleName)
        {
            var assemblyName = new AssemblyName(moduleName);
            _assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            _module = _assemblyBuilder.DefineDynamicModule(moduleName, moduleName + ".dll");
        }

        public static T Build<T>(IDictionary<string, Member> impl)
        {
            var builder = new ObjectBuilder();
            var type = builder.CreateType<T>(impl);
            return (T)Activator.CreateInstance(type, impl);
        }

        private Type CreateType<T>(IDictionary<string, Member> impl)
        {
            EmitEmptyType<T>();
            EmitConstructor();
            EmitMembers<T>(impl);
            var type = _typeBuilder.CreateType();
            return type;
        }

        private void EmitEmptyType<T>()
        {
            _generatedTypeName = BuildDynamicName<T>();
            _typeBuilder = _module.DefineType(_generatedTypeName, TypeAttributes.Public | TypeAttributes.Class);
            _typeBuilder.AddInterfaceImplementation(typeof(T));
            _membersField = _typeBuilder.DefineField(
                   "_members",
                   typeof(Dictionary<string, Member>),
                   FieldAttributes.Private);
        }

        private static string BuildDynamicName<T>()
        {
            return typeof(T).Name + "Generated" + _id++;
        }

        private void EmitConstructor()
        {
            var constructorBuilder = _typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new[] { typeof(Dictionary<string, Member>) });
            constructorBuilder.DefineParameter(1, ParameterAttributes.In, "members");

            _gen = constructorBuilder.GetILGenerator();
            EmitConstructorBody();
        }

        private void EmitConstructorBody()
        {
            _gen.Emit(OpCodes.Ldarg_0);
            _gen.Emit(OpCodes.Ldarg_1);
            _gen.Emit(OpCodes.Stfld, _membersField);
            _gen.Emit(OpCodes.Ret);
        }

        private void EmitMembers<T>(IDictionary<string, Member> members)
        {
            foreach (var member in members.Values)
                EmitMember<T>(member);

            foreach (var info in GetAllMethods(typeof(T)).Distinct()
                .Except(members.Select(m => m.Value.Info)).Where(m => !m.IsSpecialName))
                EmitNotImplementedMethod(info);
        }

        private void EmitMember<T>(Member member)
        {
            foreach (var info in
                GetAllMethods(typeof(T))
                    .Where(item => item.MetadataToken == member.Info.MetadataToken)
                    .Distinct())
                EmitMember<T>(info);
        }

        private void EmitMember<T>(MethodInfo info)
        {
            var attributes = info.Attributes;
            attributes &= ~MethodAttributes.Abstract;
            _methodBuilder = _typeBuilder.DefineMethod(
                info.Name, attributes, info.ReturnType, GetParameterTypes(info));
            if (info.ContainsGenericParameters)
                _methodBuilder.DefineGenericParameters(info.GetGenericArguments().Select(type => type.Name).ToArray());
            _gen = _methodBuilder.GetILGenerator();
            EmitMemberBody<T>(info);
        }

        private void EmitMemberBody<T>(MethodInfo info)
        {
            _gen.Emit(OpCodes.Ldarg_0);
            _gen.Emit(OpCodes.Ldfld, _membersField);
            _gen.Emit(OpCodes.Ldstr, info.MetadataToken.ToString());
            if (_methodBuilder.IsGenericMethod)
            {
                _gen.Emit(OpCodes.Ldtoken, info.GetGenericArguments()[0]);
                _gen.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
                _gen.Emit(OpCodes.Callvirt, typeof(Type).GetProperty("FullName").GetGetMethod());
                _gen.Emit(OpCodes.Call, typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }));
            }

            _gen.Emit(OpCodes.Callvirt, typeof(Dictionary<string, Member>).GetProperty("Item").GetGetMethod());
            _gen.Emit(OpCodes.Ldfld, typeof(Member).GetField("Implementation"));


            if (info.ReturnType == typeof (void))
                EmitActionCall(info);
            else
                EmitFunctionCall<T>();
            _gen.Emit(OpCodes.Ret);
        }

        private void EmitFunctionCall<T>()
        {
            _gen.Emit(OpCodes.Callvirt, typeof (Func<T>).GetMethod("Invoke"));
        }

        private void EmitActionCall(MethodInfo info)
        {
            var parameters = info.GetParameters();
            Type type;
            if (parameters.Length > 0)
            {
                type = GetType(parameters);
                _gen.Emit(OpCodes.Ldarg_1);
            }
            else type = typeof (Action);

            _gen.Emit(OpCodes.Callvirt, type.GetMethod("Invoke"));
        }

        private static Type GetType(IEnumerable<ParameterInfo> parameters)
        {
            return typeof(Action<>).MakeGenericType(parameters
                                                        .Select(item => item.ParameterType)
                                                        .ToArray());
        }

        private static IEnumerable<MethodInfo> GetAllMethods(Type type)
        {
            foreach (var method in type.GetMethods().Where(m => !m.IsSpecialName))
                yield return method;
            foreach (var baseType in type.GetInterfaces())
                foreach (var baseMethod in GetAllMethods(baseType))
                    yield return baseMethod;
        }

        private void EmitNotImplementedMethod(MethodInfo info)
        {
            var attributes = info.Attributes;
            attributes &= ~MethodAttributes.Abstract;
            _methodBuilder = _typeBuilder.DefineMethod(
                info.Name, attributes, info.ReturnType, GetParameterTypes(info));
            if (info.ContainsGenericParameters)
                _methodBuilder.DefineGenericParameters(info.GetGenericArguments().Select(type => type.Name).ToArray());
            _gen = _methodBuilder.GetILGenerator();
            _gen.ThrowException(typeof(NotImplementedException));
        }

        private static Type[] GetParameterTypes(MethodInfo method)
        {
            var parameters = method.GetParameters();
            var parameterTypes = new List<Type>();
            foreach (var parameter in parameters)
                parameterTypes.Add(parameter.ParameterType);
            return parameterTypes.ToArray();
        }
    }
}