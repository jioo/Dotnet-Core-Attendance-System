using System;
using System.Linq;
using System.Linq.Expressions;

namespace WebApi.Extensions
{
    /// <summary>
    /// Work in progress...
    /// </summary>
    public static class SortExtensions
    {
        public static IQueryable<T> CustomSort<T>(
            this IQueryable<T> queryable, 
            Expression<Func<T, object>> propertyToSort,
            bool isDescending)
        {
            return (isDescending)
                ? queryable.OrderByDescending(propertyToSort)
                : queryable.OrderBy(propertyToSort);
        }

        public static IQueryable<T> CustomSort<T>(
            this IQueryable<T> queryable, 
            bool isDescending,
            string sortBy,
            params Expression<Func<T, object>>[] propertiesToSort)
        {
            foreach(var property in propertiesToSort)
            {
                var toPascalCase = char.ToUpper(sortBy[0]) + sortBy.Substring(1);  
                if(sortBy.ToLower() == typeof(T).GetProperty(toPascalCase).Name.ToLower())
                {
                    return (isDescending)
                        ? queryable.OrderByDescending(m => GetPropValue(m, toPascalCase))
                        : queryable.OrderBy(m => GetPropValue(m, toPascalCase));
                }
            }

            return queryable.OrderBy(m => GetPropValue(m, "Created"));
        }

        private static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        } 
    }
}