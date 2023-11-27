using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework.Extensions
{
    public static class ObjectExtensions
    {
        private static readonly MethodInfo CloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);

        //
        // Parameters:
        //   type:
        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(string))
            {
                return true;
            }

            return type.IsValueType && type.IsPrimitive;
        }

        private static FieldInfo GetFieldInfo(Type type, string fieldName)
        {
            FieldInfo field;
            do
            {
                field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                type = type.BaseType;
            }
            while (field == null && type != null);
            return field;
        }

        //
        // Parameters:
        //   obj:
        //
        //   fieldName:
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //
        //   T:System.ArgumentOutOfRangeException:
        public static object GetFieldValue(this object obj, string fieldName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            Type type = obj.GetType();
            FieldInfo fieldInfo = GetFieldInfo(type, fieldName);
            if (fieldInfo == null)
            {
                throw new ArgumentOutOfRangeException("fieldName", $"Couldn't find field {fieldName} in type {type.FullName}");
            }

            return fieldInfo.GetValue(obj);
        }

        //
        // Parameters:
        //   obj:
        //
        //   fieldName:
        //
        //   val:
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //
        //   T:System.ArgumentOutOfRangeException:
        public static void SetFieldValue(this object obj, string fieldName, object val)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            Type type = obj.GetType();
            FieldInfo fieldInfo = GetFieldInfo(type, fieldName);
            if (fieldInfo == null)
            {
                throw new ArgumentOutOfRangeException("fieldName", $"Couldn't find field {fieldName} in type {type.FullName}");
            }

            fieldInfo.SetValue(obj, val);
        }

        //
        // Summary:
        //     Produces a deep copy of a given object
        //
        // Parameters:
        //   originalObject:
        public static object Copy(this object originalObject)
        {
            return InternalCopy(originalObject, new Dictionary<object, object>(new ReferenceEqualityComparer()));
        }

        private static object InternalCopy(object originalObject, IDictionary<object, object> visited)
        {
            if (originalObject == null)
            {
                return null;
            }

            Type type = originalObject.GetType();
            if (type.IsPrimitive())
            {
                return originalObject;
            }

            if (visited.ContainsKey(originalObject))
            {
                return visited[originalObject];
            }

            if (typeof(Delegate).IsAssignableFrom(type))
            {
                return null;
            }

            object obj = CloneMethod.Invoke(originalObject, null);
            if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                if (!elementType.IsPrimitive())
                {
                    Array clonedArray = (Array)obj;
                    clonedArray.ForEach(delegate (Array array, int[] indices)
                    {
                        array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices);
                    });
                }
            }

            visited.Add(originalObject, obj);
            CopyFields(originalObject, visited, obj, type);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, obj, type);
            return obj;
        }

        private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            if (typeToReflect.BaseType != null)
            {
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
                CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, (FieldInfo info) => info.IsPrivate);
            }
        }

        private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
        {
            FieldInfo[] fields = typeToReflect.GetFields(bindingFlags);
            foreach (FieldInfo fieldInfo in fields)
            {
                if ((filter == null || filter(fieldInfo)) && !fieldInfo.FieldType.IsPrimitive())
                {
                    object value = fieldInfo.GetValue(originalObject);
                    object value2 = InternalCopy(value, visited);
                    fieldInfo.SetValue(cloneObject, value2);
                }
            }
        }

        //
        // Parameters:
        //   original:
        //
        // Type parameters:
        //   T:
        public static T Copy<T>(this T original)
        {
            return (T)((object)original).Copy();
        }
    }
}
