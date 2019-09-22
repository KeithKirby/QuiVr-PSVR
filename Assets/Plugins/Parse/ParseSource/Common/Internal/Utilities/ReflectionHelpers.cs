// Copyright (c) 2015-present, Parse, LLC.  All rights reserved.  This source code is licensed under the BSD-style license found in the LICENSE file in the root directory of this source tree.  An additional grant of patent rights can be found in the PATENTS file in the same directory.

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Parse.Common.Internal {
  public static class ReflectionHelpers {
    public static IEnumerable<PropertyInfo> GetProperties(Type type) {
      return type.GetProperties();
    }

    public static MethodInfo GetMethod(Type type, string name, Type[] parameters) {
      return type.GetMethod(name, parameters);

    }

    public static bool IsPrimitive(Type type) {

			return type.IsPrimitive;

    }

    public static IEnumerable<Type> GetInterfaces(Type type) {

      return type.GetInterfaces();

    }

    public static bool IsConstructedGenericType(Type type) {

      return type.IsGenericType && !type.IsGenericTypeDefinition;

    }

    public static IEnumerable<ConstructorInfo> GetConstructors(Type type) {

      BindingFlags searchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      return type.GetConstructors(searchFlags);

    }

    public static Type[] GetGenericTypeArguments(Type type) {

      return type.GetGenericArguments();

    }

    public static PropertyInfo GetProperty(Type type, string name) {

      return type.GetProperty(name);

    }

    /// <summary>
    /// This method helps simplify the process of getting a constructor for a type.
    /// A method like this exists in .NET but is not allowed in a Portable Class Library,
    /// so we've built our own.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="parameterTypes"></param>
    /// <returns></returns>
    public static ConstructorInfo FindConstructor(this Type self, params Type[] parameterTypes) {
      var constructors =
        from constructor in GetConstructors(self)
        let parameters = constructor.GetParameters()
        let types = from p in parameters select p.ParameterType
        where types.SequenceEqual(parameterTypes)
        select constructor;
      return constructors.SingleOrDefault();
    }

    public static bool IsNullable(Type t) {
      bool isGeneric;

      isGeneric = t.IsGenericType && !t.IsGenericTypeDefinition;

      return isGeneric && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
    }

    public static IEnumerable<T> GetCustomAttributes<T>(this Assembly assembly) where T: Attribute {

      return assembly.GetCustomAttributes(typeof(T), false).Select(attr => attr as T);

    }
  }
}
