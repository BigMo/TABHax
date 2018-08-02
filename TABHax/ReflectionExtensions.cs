using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TABHax
{
    public static class ReflectionExtensions
    {
        public static T GetField<T>(this object obj, string fieldName)
        {
            return (T)obj.GetType().GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).GetValue(obj);
        }

        public static void SetField<T>(this object obj, string fieldName, object value)
        {
            if (obj == null)
                return;
            obj.GetType().GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).SetValue(obj, value);
        }
        public static void SetFieldNULL(this object obj, string fieldName)
        {
            if (obj == null)
                return;
            obj.GetType().GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).SetValue(obj, null);
        }
        public static T GetProperty<T>(this object obj, string fieldName)
        {
            return (T)obj.GetType().GetProperty(fieldName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).GetValue(obj, null);
        }

        public static void SetProperty<T>(this object obj, string fieldName, object value)
        {
            if (obj == null)
                return;
            obj.GetType().GetProperty(fieldName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).SetValue(null, value, null);
        }


        public static T StaticGetField<T>(this object obj, string fieldName)
        {
            return (T)obj.GetType().GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).GetValue(null);
        }
        public static void StaticSetField<T>(this object obj, string fieldName, object value)
        {
            if (obj == null)
                return;
            obj.GetType().GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).SetValue(null, value);
        }
        public static T StaticGetProperty<T>(this object obj, string fieldName)
        {
            return (T)obj.GetType().GetProperty(fieldName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).GetValue(null, null);
        }
        public static void StaticSetProperty<T>(this object type, string fieldName, object value)
        {
            if (type == null)
                return;
            type.GetType().GetProperty(fieldName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).SetValue(null, value, null);
        }
    }
}