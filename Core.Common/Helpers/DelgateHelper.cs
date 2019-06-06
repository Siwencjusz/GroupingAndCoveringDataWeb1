﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Core.Common.Helpers
{
    public static class DelgateHelper
    {
        public static Delegate[] ExtractMethods(object obj)
        {
            Type type = obj.GetType();

            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            Delegate[] methodsDelegate = new Delegate[methods.Count()];

            for (int i = 0; i < methods.Count(); i++)
            {
                methodsDelegate[i] = CreateDelegate(obj, methods[i]);
            }

            return methodsDelegate;
        }

        public static Delegate CreateDelegate(object instance, MethodInfo method)
        {
            var parameters = method.GetParameters()
                .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                .ToArray();

            var call = Expression.Call(Expression.Constant(instance), method, parameters);
            var x = Expression.Lambda(call, parameters).Compile();
            return x;
        }
    }
}
