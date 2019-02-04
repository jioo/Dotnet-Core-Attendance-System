using System;
using System.Linq;
using WebApi.Entities;

namespace WebApi.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Apply sort filters based on parameters
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> queryable, DateFilteredList parameters)
        {
            var isDescending = parameters.Descending ?? false;

            // Check if `SoryBy` has value
            if(!string.IsNullOrEmpty(parameters.SortBy))
            {
                var sortBy = parameters.SortBy;
                // Transform to pascal case
                sortBy = char.ToUpper(sortBy[0]) + sortBy.Substring(1);

                // Apply sort filter 
                return (isDescending)
                    ? queryable.OrderByDescending(m => GetPropValue(m, sortBy))
                    : queryable.OrderBy(m => GetPropValue(m, sortBy));
            }

            // Default sorting
            return queryable.OrderByDescending(m => GetPropValue(m, "Created"));
        }

        /// <summary>
        /// Apply pagination based on parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> queryable, DateFilteredList parameters) where T : class
        {
            // Parse parameters to int
            var pageSize = Convert.ToInt32(parameters.RowsPerPage);
            var pageNumber = Convert.ToInt32(parameters.Page);

            // Calculate next page
            var skip = pageSize * (pageNumber - 1);

            // Paginate based on parameters
            return queryable.Skip(pageSize * (pageNumber - 1)).Take(pageSize).OrderByDescending(m => GetPropValue(m, "Created"));
        }

        /// <summary>
        /// Helper method to get the Value from Reflection class
        /// </summary>
        /// <param name="src"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        private static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        } 
    }
}