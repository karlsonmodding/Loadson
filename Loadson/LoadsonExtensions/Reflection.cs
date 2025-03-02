using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadsonExtensions
{
    public static class Reflection
    {
        /// <summary>
        /// Get field value of object via reflection
        /// Use only on private fields
        /// </summary>
        /// <typeparam name="T">Field type</typeparam>
        /// <param name="obj">Object instance (syntactic sugar)</param>
        /// <param name="FieldName">Field name</param>
        /// <returns>Field value</returns>
        public static T ReflectionGet<T>(this object obj, string FieldName)
        {
            return (T)obj.GetType().GetField(FieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(obj);
        }

        /// <summary>
        /// Set field value of object via reflection
        /// </summary>
        /// <typeparam name="T">Field type (syntactic sugar)</typeparam>
        /// <param name="obj">Object instance (syntactic sugar)</param>
        /// <param name="FieldName">Field name</param>
        /// <param name="value">Value to set</param>
        public static void ReflectionSet<T>(this object obj, string FieldName, T value)
        {
            obj.GetType().GetField(FieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public).SetValue(obj, value);
        }

        /// <summary>
        /// Invoke method with no arguments with no return value via reflection
        /// Use only on private methods
        /// </summary>
        /// <param name="obj">Object instance (syntactic sugar)</param>
        /// <param name="MethodName">Method name</param>
        public static void ReflectionInvoke(this object obj, string MethodName) => ReflectionInvoke(obj, MethodName, Array.Empty<object>());

        /// <summary>
        /// Invoke method with no return value via reflection
        /// Use only on private methods
        /// </summary>
        /// <param name="obj">Object instance (syntactic sugar)</param>
        /// <param name="MethodName">Method name</param>
        /// <param name="Args">Arguments to pass to method</param>
        public static void ReflectionInvoke(this object obj, string MethodName, params object[] Args)
        {
            obj.GetType().GetMethod(MethodName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(obj, Args);
        }

        /// <summary>
        /// Invoke method with no arguments with return value via reflection
        /// Use only on private methods
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="obj">Object instance (syntactic sugar)</param>
        /// <param name="MethodName">Method name</param>
        /// <returns></returns>
        public static T ReflectionInvoke<T>(this object obj, string MethodName) => ReflectionInvoke<T>(obj, MethodName, Array.Empty<object>());

        /// <summary>
        /// Invoke method with return value via reflection
        /// Use only on private methods
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="obj">Object instance (syntactic sugar)</param>
        /// <param name="MethodName">Method name</param>
        /// <param name="Args">Arguments to pass to method</param>
        /// <returns></returns>
        public static T ReflectionInvoke<T>(this object obj, string MethodName, params object[] Args)
        {
            return (T)obj.GetType().GetMethod(MethodName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(obj, Args);
        }
    }
}
