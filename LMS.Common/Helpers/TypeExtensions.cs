using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace LMS.Common.Helpers
{
    public static class TypeExtensions
    {
        public static T? TryGetDynamicPropertyValue<T>(this object obj, string propertyPath)
        {
            /**
                Simple Property → "Name" (public string Name {get; set;})
                Simple Field → "Age" (public string Age;)
                Nested Property → "Address.City"
                Nested Field → "Config.MaxRetries"
                Array/List Index → "Items[0]"
                Indexed + Property → "Items[2].Name"
                Indexed + Field → "Logs[1].Level"
                Deep Nesting (Mixed) → "Orders[1].Customer.Address.City"
                Mixed Property/Field → "Settings.Theme.Name"
            **/

            if (obj == null || string.IsNullOrWhiteSpace(propertyPath))
                return default;

            try
            {
                object? currentObject = obj;

                // Regex to match property names and indexers like [0] or for .
                var segments = Regex.Matches(propertyPath, @"([^\.\[\]]+)|\[(\d+)\]");

                foreach (Match segment in segments)
                {
                    if (currentObject == null)
                        return default;

                    if (segment.Value.StartsWith("["))
                    {
                        int index = int.Parse(segment.Groups[2].Value);
                        
                        // Handle indexer
                        if (currentObject is IList list)
                        {
                            if (index < 0 || index >= list.Count)
                                return default;

                            currentObject = list[index];
                        }
                        else if (currentObject is IEnumerable enumerable)
                        {
                            var enumerator = enumerable.GetEnumerator();

                            for (int i = 0; i <= index; i++)
                            {
                                if (!enumerator.MoveNext())
                                    return default;
                            }

                            currentObject = enumerator.Current;
                        }
                        else
                        {
                            return default;
                        }
                    }
                    else
                    {
                        // Handle property or field
                        var type = currentObject.GetType();

                        var property = type.GetProperty(segment.Value, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (property != null && property.CanRead)
                        {
                            currentObject = property.GetValue(currentObject);
                            continue;
                        }

                        var field = type.GetField(segment.Value, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (field != null)
                        {
                            currentObject = field.GetValue(currentObject);
                            continue;
                        }

                        return default;
                    }
                }

                if (currentObject == null)
                    return default;

                var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                return (T)Convert.ChangeType(currentObject, targetType);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error accessing dynamic property '{propertyPath}': {ex}");
                return default;
            }
        }

    }
}
