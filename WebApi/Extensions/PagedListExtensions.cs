using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WebApi.Entities;

namespace WebApi.Extensions
{
    public static class PagedListExtendions
    {
        public static IQueryable<T> ToPagedList<T>(
            this IQueryable<T> data, 
            BasePagedList parameters) where T : class
        {
            var currentQuery = data;
            
            // Get all properties of T class 
            var stringProperties = typeof(T).GetProperties(); 

            // Check if there is a search parameter
            if(!string.IsNullOrEmpty(parameters.Search))
            {
                // Search all columns
                currentQuery = currentQuery.Where(m => 
                    stringProperties.Any(prop =>
                        prop.GetValue(m, null)
                            .ToString()
                            .ToLower()
                            .Contains(parameters.Search.ToLower())
                    )
                );
            }

            // Check if there is a sortBy parameter and descending is not null
            if(!string.IsNullOrEmpty(parameters.SortBy) &&
               parameters.Descending != null)
            {
                // Find a first the property to sort.
                var sortBy = stringProperties.Where(prop => 
                    prop.GetValue(prop, null).ToString().ToLower() 
                        == parameters.SortBy.ToLower()
                );

                // Pass the property to sort
                currentQuery = ( (bool) parameters.Descending)
                    ? currentQuery.OrderByDescending(m => m.GetType().GetProperty(sortBy.ToString()) )
                    : currentQuery.OrderBy(m => m.GetType().GetProperty(sortBy.ToString()) );
            }

            // Paginate based on parameters
            var pageSize = parameters.RowsPerPage;
            var pageNumber = parameters.Page;
            currentQuery = currentQuery
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize);

            return currentQuery;
        }
    }
}