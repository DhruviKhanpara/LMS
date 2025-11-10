using System.Linq.Dynamic.Core;
using System.Reflection;

namespace LMS.Common.Helpers
{
    public static class SortingExtensions
    {
        private static readonly Dictionary<Type, HashSet<string>> PropertyCache = new();

        public static IQueryable<T> Sort<T>(this IQueryable<T> query, string? orderBy) where T : class
        {
            if (string.IsNullOrWhiteSpace(orderBy))
                return query;

            var orderParams = orderBy
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(param => param.Trim());

            var allowedProperties = GetAllowedProperties(typeof(T));

            var validParams = orderParams
                .Where(param =>
                {
                    var propertyName = param.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0];
                    return allowedProperties.Contains(propertyName);
                })
                .Select(param =>
                {
                    var paramParts = param.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    var propertyName = paramParts[0];
                    var rawDirection = paramParts.Length > 1 ? param.Split(" ")[1].ToLower() : "asc";

                    var direction = rawDirection switch
                    {
                        "asc" => nameof(OrderDirection.ascending),
                        "desc" => nameof(OrderDirection.descending),
                        _ => throw new ArgumentException($"Invalid sorting direction: {rawDirection} in parameter '{param}'")
                    };

                    return $"{propertyName} {direction}";
                })
                .Distinct();

            if (!validParams.Any())
                return query.OrderBy($"Id {nameof(OrderDirection.ascending)}");

            var orderQuery = string.Join(", ", validParams);
            return query.OrderBy(orderQuery);
        }

        private static HashSet<string> GetAllowedProperties(Type type)
        {
            if (!PropertyCache.ContainsKey(type))
            {
                PropertyCache[type] = new HashSet<string>(
                    type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Select(p => p.Name),
                    StringComparer.InvariantCultureIgnoreCase);
            }
            return PropertyCache[type];
        }

        private enum OrderDirection
        {
            ascending,
            descending
        }
    }
}
