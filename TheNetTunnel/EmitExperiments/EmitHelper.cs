﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EmitExperiments
{
    public static class EmitHelper
    {
        public static MethodBuilder ImplementInterfaceMethod(MethodInfo interfaceMethodInfo, TypeBuilder typeBuilder)
        {
            Type[] inputParams = interfaceMethodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
            Type outputParams = interfaceMethodInfo.ReturnType;


            MethodBuilder metbuilder;
            if (!inputParams.Any() && outputParams == typeof(void))
                metbuilder = typeBuilder.DefineMethod(interfaceMethodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual);
            else
                metbuilder = typeBuilder.DefineMethod(interfaceMethodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual, outputParams, inputParams);

            typeBuilder.DefineMethodOverride(metbuilder, interfaceMethodInfo);
            return metbuilder;
        }


        public static PropertyBuilder ImplementInterfaceProperty(TypeBuilder typeBuilder, PropertyInfo interfacePropertyInfo)
        {
            var fieldBuilder = typeBuilder.DefineField("_" +
               interfacePropertyInfo.Name,
               interfacePropertyInfo.PropertyType,
               FieldAttributes.Private);

            var propertyBuilder = typeBuilder.DefineProperty(
                interfacePropertyInfo.Name,
                PropertyAttributes.HasDefault,
                interfacePropertyInfo.PropertyType,
                null);

            #region создаём GetMethod

            MethodBuilder getPropMthdBldr = typeBuilder.DefineMethod("get_" + interfacePropertyInfo.Name,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                interfacePropertyInfo.PropertyType, Type.EmptyTypes);


            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);
            #endregion

            #region создаём SetMethod

            MethodBuilder setPropMthdBldr = typeBuilder.DefineMethod("set_" + interfacePropertyInfo.Name,
               MethodAttributes.Public |
               MethodAttributes.SpecialName |
               MethodAttributes.HideBySig|
               MethodAttributes.Virtual,
               null, new[] { interfacePropertyInfo.PropertyType });

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            // Label modifyProperty = setIl.DefineLabel();
            // Label exitSet = setIl.DefineLabel();

            // setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            // setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);
            #endregion

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
            typeBuilder.DefineMethodOverride(getPropMthdBldr, interfacePropertyInfo.GetMethod);
            typeBuilder.DefineMethodOverride(setPropMthdBldr, interfacePropertyInfo.SetMethod);
            
            return propertyBuilder;
        }
    }
}
