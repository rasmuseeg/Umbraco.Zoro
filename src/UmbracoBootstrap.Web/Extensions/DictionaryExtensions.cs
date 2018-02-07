namespace System.Collections.Generic
{
    public static class DictionaryExtensions
    {
        public static T GetValue<T>(this IDictionary<string, object> dictionary, string key)
        {
            return dictionary.GetValue(key, default(T));
        }

        public static T GetValue<T>(this IDictionary<string, object> dictionary, string key, T defaultValue)
        {
            if (!dictionary.ContainsKey(key) || string.IsNullOrEmpty(dictionary[key].ToString()))
                return defaultValue;

            return (T)Convert.ChangeType(dictionary[key], typeof(T));
        }
    }
}