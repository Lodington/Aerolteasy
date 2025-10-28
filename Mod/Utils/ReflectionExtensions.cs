using System.Reflection;

namespace RoR2DevTool.Utils
{
    public static class ReflectionExtensions
    {
        public static void SetFieldValue<T>(this object obj, string fieldName, T value)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (field != null)
            {
                field.SetValue(obj, value);
            }
        }

        public static T GetFieldValue<T>(this object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (field != null)
            {
                return (T)field.GetValue(obj);
            }
            return default(T);
        }
    }
}