using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using WebApi.Entities;

namespace WebApi.Extensions
{
    public static class PagedListExtendions
    {
        /// <summary>
        /// Paginate a List based on parameters
        /// </summary>
        /// <param name="parameters">model for pagination/date filtered</param>
        /// <returns>
        /// Returns <see cref="IQueryable{T}"/> that contains paged list
        /// </returns>
        public static IQueryable<T> ToPagedList<T>(this IQueryable<T> queryable, DateFilteredList parameters) where T : class
        {
            var pageSize = Convert.ToInt32(parameters.RowsPerPage);
            var pageNumber = Convert.ToInt32(parameters.Page);

            // Paginate based on parameters
            return queryable.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
        }
    }
}