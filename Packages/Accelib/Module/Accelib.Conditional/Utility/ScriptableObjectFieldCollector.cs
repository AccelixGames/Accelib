using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Accelib.Conditional.Utility
{
    public static class ScriptableObjectFieldCollector
    {
        public static List<SerializedFieldInfo> GetSerializedFields(this ScriptableObject so)
        {
            var list = new List<SerializedFieldInfo>();

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var fields = so.GetType().GetFields(flags);

            foreach (var field in fields)
            {
                var isSerializable = field.IsPublic && !field.IsNotSerialized || field.GetCustomAttribute<SerializeField>() != null;
                if (!isSerializable) continue;

                var type = field.FieldType;
                if (type != typeof(int) && type != typeof(long) &&
                    type != typeof(float) && type != typeof(double)) continue;
                
                var value = (double)field.GetValue(so);
                
                list.Add(new SerializedFieldInfo
                {
                    fieldName = field.Name,
                    fieldType = type,
                    Value = value
                });
            }

            return list;
        }
    }
}