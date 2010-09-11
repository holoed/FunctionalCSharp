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

namespace FunctionalCSharp.DiscriminatedUnions
{
    public class DataType
    {
        private static AssemblyBuilder _assemblyBuilder;
        private static ModuleBuilder _module;
        private string _generatedTypeName;
        private static int _id;
        private static readonly IDictionary<Type, Func<object>> TypeCache = new Dictionary<Type, Func<object>>();

        static DataType()
        {
            SetupBuilderAndModule("DataTypes");
        }

        private DataType()
        { }

        public static a New<a>()
        {
            return new DataType().Return<a>();
        }              

        private static void SetupBuilderAndModule(string moduleName)
        {
            var assemblyName = new AssemblyName(moduleName);
            _assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            _module = _assemblyBuilder.DefineDynamicModule(moduleName, moduleName + ".dll");
        }

        private static Func<object> CreateDynamicConstructor(TypeBuilder typeBuilder)
        {
            var type = typeBuilder.CreateType();
            var dynamic = new DynamicMethod("CreateInstance",
                                            typeof(object),
                                            new Type[0],
                                            type);
            var il = dynamic.GetILGenerator();
            il.Emit(OpCodes.Newobj, type.GetConstructor(new Type[0]));
            il.Emit(OpCodes.Ret);
            return (Func<object>)dynamic.CreateDelegate(typeof(Func<object>));
        }

        private a Return<a>()
        {
            var typeKey = typeof (a);
            if (!TypeCache.ContainsKey(typeKey))
            {
                var typeBuilder = EmitEmptyType<a>();
                EmitConstructor(typeBuilder);
                EmitMethods<a>(typeBuilder, CreateMethodBody<a>);
                TypeCache.Add(typeKey, CreateDynamicConstructor(typeBuilder));
            }
            return (a)TypeCache[typeKey]();
        }

        private static void EmitConstructor(TypeBuilder typeBuilder)
        {
            var constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new Type[0]);
            var gen = constructorBuilder.GetILGenerator();
            gen.Emit(OpCodes.Ret);
        }

        private static void EmitMethods<a>(TypeBuilder typeBuilder, Action<MethodInfo, MethodBuilder> createBody)
        {
            var interfaceType = typeof(a);
            var methods = interfaceType.GetMethods();
            foreach (var method in methods)
                CreateMethod(method, typeBuilder, createBody);
        }

        private static void CreateMethod(MethodInfo method, TypeBuilder typeBuilder, Action<MethodInfo, MethodBuilder> createBody)
        {
            var attributes = method.Attributes;
            attributes &= ~MethodAttributes.Abstract;
            var parameterInfos = method.GetParameters();
            var methodBuilder = typeBuilder.DefineMethod(
                method.Name,
                attributes,
                CallingConventions.Standard,
                method.ReturnType,
                parameterInfos.Select(p => p.ParameterType).ToArray());
            createBody(method, methodBuilder);
        }

        private static void CreateMethodBody<a>(MethodInfo method, MethodBuilder methodBuilder)
        {
            var il = methodBuilder.GetILGenerator();
            var type = BuildValueConstructor<a>(method);
            var parameterInfos = method.GetParameters();
            int index = 1;
            foreach (var p in parameterInfos)
                il.Emit(OpCodes.Ldarg, index++);
            il.Emit(OpCodes.Newobj, type.CreateType().GetConstructor(parameterInfos.Select(p => p.ParameterType).ToArray()));
            il.Emit(OpCodes.Ret);
        }

        private static void CreateEmptyMethodBody(MethodInfo method, MethodBuilder methodBuilder)
        {
            var il = methodBuilder.GetILGenerator();
            il.ThrowException(typeof(NotSupportedException));
        }

        private static TypeBuilder BuildValueConstructor<a>(MethodInfo method)
        {
            var typeName = typeof (a).FullName;
            var typeBuilder = _module.DefineType(typeName + "." + method.Name, TypeAttributes.NotPublic | TypeAttributes.Class);
            typeBuilder.AddInterfaceImplementation(typeof(a));
            var fields = EmitItemConstructor(typeBuilder, method);
            EmitMethods<a>(typeBuilder, CreateEmptyMethodBody);
            EmitEquality(typeBuilder, fields);
            return typeBuilder;
        }

        private static IEnumerable<FieldBuilder> EmitItemConstructor(TypeBuilder typeBuilder, MethodInfo methodInfo)
        {
            var parameterInfos = methodInfo.GetParameters();
            var constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                parameterInfos.Select(p => p.ParameterType).ToArray());
            int index = 1;
            var fields = new List<FieldBuilder>();
            foreach (var parameterInfo in parameterInfos)
            {
                fields.Add(typeBuilder.DefineField(parameterInfo.Name, parameterInfo.ParameterType, FieldAttributes.Public));
                constructorBuilder.DefineParameter(index++, ParameterAttributes.In, parameterInfo.Name);
            }
            var gen = constructorBuilder.GetILGenerator();
            index = 1;
            foreach (var fieldBuilder in fields)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldarg, index++);                
                gen.Emit(OpCodes.Stfld, fieldBuilder);
            }
            gen.Emit(OpCodes.Ret);
            return fields;
        }

        private static void EmitEquality(TypeBuilder typeBuilder, IEnumerable<FieldBuilder> fields)
        {
            var methodBuilder = typeBuilder.DefineMethod("Equals", MethodAttributes.Public | MethodAttributes.Virtual, CallingConventions.Standard, typeof(bool), new[]{typeof(object)});
            var ilGen = methodBuilder.GetILGenerator();
            var loc0 = ilGen.DeclareLocal(typeof (bool));
            var loc1 = ilGen.DeclareLocal(typeof(bool));
            var loc2 = ilGen.DeclareLocal(typeBuilder);
            ilGen.Emit(OpCodes.Ldnull);
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Call, typeof(object).GetMethod("ReferenceEquals", BindingFlags.Public | BindingFlags.Static));
            ilGen.Emit(OpCodes.Ldc_I4_0);
            ilGen.Emit(OpCodes.Ceq);
            ilGen.Emit(OpCodes.Stloc, loc1);
            ilGen.Emit(OpCodes.Ldloc, loc1);
            var then = ilGen.DefineLabel();
            var end = ilGen.DefineLabel();
            ilGen.Emit(OpCodes.Brtrue_S, then);
            ilGen.Emit(OpCodes.Ldc_I4_0);
            ilGen.Emit(OpCodes.Stloc, loc0);
            ilGen.Emit(OpCodes.Br, end);
            ilGen.MarkLabel(then);
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Call, typeof(object).GetMethod("ReferenceEquals", BindingFlags.Public | BindingFlags.Static));
            ilGen.Emit(OpCodes.Ldc_I4_0);
            ilGen.Emit(OpCodes.Ceq);
            ilGen.Emit(OpCodes.Stloc, loc1);
            ilGen.Emit(OpCodes.Ldloc, loc1);
            var then2 = ilGen.DefineLabel();
            ilGen.Emit(OpCodes.Brtrue_S, then2);
            ilGen.Emit(OpCodes.Ldc_I4_1);
            ilGen.Emit(OpCodes.Stloc, loc0);
            ilGen.Emit(OpCodes.Br, end);
            ilGen.MarkLabel(then2);
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Callvirt, typeof(Object).GetMethod("GetType"));
            ilGen.Emit(OpCodes.Ldtoken, typeBuilder.TypeToken.Token);
            ilGen.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static));
            ilGen.Emit(OpCodes.Call, typeof(Type).GetMethod("op_Inequality", BindingFlags.Public | BindingFlags.Static));
            ilGen.Emit(OpCodes.Ldc_I4_0);
            ilGen.Emit(OpCodes.Ceq);
            ilGen.Emit(OpCodes.Stloc, loc1);
            ilGen.Emit(OpCodes.Ldloc, loc1);
            var then3 = ilGen.DefineLabel();
            ilGen.Emit(OpCodes.Brtrue_S, then3);
            ilGen.Emit(OpCodes.Ldc_I4_0);
            ilGen.Emit(OpCodes.Stloc, loc0);
            ilGen.Emit(OpCodes.Br, end);
            ilGen.MarkLabel(then3);
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Castclass, typeBuilder.TypeToken.Token);
            ilGen.Emit(OpCodes.Stloc, loc2);

            ilGen.Emit(OpCodes.Ldc_I4_1);
            ilGen.Emit(OpCodes.Stloc, loc0);
            foreach (var field in fields)
            {
                ilGen.Emit(OpCodes.Ldloc, loc2);
                ilGen.Emit(OpCodes.Ldfld, field);
                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.Emit(OpCodes.Ldfld, field);
                ilGen.Emit(OpCodes.Call, typeof (object).GetMethod("Equals", BindingFlags.Public | BindingFlags.Static));
                ilGen.Emit(OpCodes.Ldc_I4_1);
                ilGen.Emit(OpCodes.Ceq);
                ilGen.Emit(OpCodes.Stloc, loc1);
                ilGen.Emit(OpCodes.Ldloc, loc1);
                var thenField = ilGen.DefineLabel();
                ilGen.Emit(OpCodes.Brtrue, thenField);
                ilGen.Emit(OpCodes.Ldc_I4_0);
                ilGen.Emit(OpCodes.Stloc, loc0);
                ilGen.Emit(OpCodes.Br, end);
                ilGen.MarkLabel(thenField);             
            }
            
            ilGen.Emit(OpCodes.Br_S, end);
            ilGen.MarkLabel(end);
            ilGen.Emit(OpCodes.Ldloc, loc0);
            ilGen.Emit(OpCodes.Ret);
        }

        private TypeBuilder EmitEmptyType<a>()
        {
            _generatedTypeName = BuildDynamicName<a>();
            var typeBuilder = _module.DefineType(_generatedTypeName, TypeAttributes.Public | TypeAttributes.Class);
            typeBuilder.AddInterfaceImplementation(typeof(a));
            return typeBuilder;
        }

        private static string BuildDynamicName<a>()
        {
            return typeof(a).FullName + "DataType" + _id++;
        }
    }
}
