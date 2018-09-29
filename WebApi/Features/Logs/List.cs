using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Extensions;
using WebApi.Infrastructure;

namespace WebApi.Features.Logs
{
    /// <summary>
    /// Paginate list of Logs with optional filters (search and sort)
    /// </summary>
    public class List
    {
        public class Query : IRequest<Object> 
        { 
            public Query(BasePagedList parameters)
            {
                Parameters = parameters;
            }

            public BasePagedList Parameters { get; }
        }

        public class QueryHandler : IRequestHandler<Query, Object>
        {
            private readonly int DEFAULT_PAGE = 1;
            private readonly int DEFAULT_ROWS_PER_PAGE = 10;
            private readonly ApplicationDbContext _context;

            public QueryHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Object> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    IQueryable<LogViewModel> queryableModel;

                    // Apply Search filter if not null
                    var searchQuery = request.Parameters.Search;
                    if(!string.IsNullOrEmpty(searchQuery))
                    {
                        queryableModel = _context.Logs.MapToViewModel()
                            .Where(m => 
                                m.FullName.Contains(searchQuery) &&
                                m.Deleted == null);
                    }
                    else
                    {
                        queryableModel = _context.Logs.MapToViewModel()
                            .Where(m => m.Deleted == null);
                    }

                    // Handle sortable columns
                    if(!string.IsNullOrEmpty(request.Parameters.SortBy) &&
                       request.Parameters.Descending != null)
                    {
                        var isDescending = (bool) request.Parameters.Descending;

                        /// <summary>
                        /// TODO: Fix <see cref="Extensions.SortExtensions" /> to refactor this switch statement.
                        /// </summary>
                        switch (request.Parameters.SortBy)
                        {
                            case "fullName":
                                // queryableModel.CustomSort(m => m.FullName, isDescending);
                                queryableModel = (isDescending)
                                    ? queryableModel.OrderByDescending(m => m.FullName)
                                    : queryableModel.OrderBy(m => m.FullName);
                                break;

                            case "timeIn":
                                queryableModel = (isDescending)
                                    ? queryableModel.OrderByDescending(m => m.TimeIn)
                                    : queryableModel.OrderBy(m => m.TimeIn);
                                break;

                            case "timeOut":
                                queryableModel = (isDescending)
                                        ? queryableModel.OrderByDescending(m => m.TimeOut)
                                        : queryableModel.OrderBy(m => m.TimeOut);
                                break;

                            default:
                                queryableModel.OrderByDescending(m => m.Created);
                                break;
                        }

                        // queryableModel.CustomSort(
                        //     isDescending, 
                        //     request.Parameters.SortBy,
                        //     m => m.FullName,
                        //     m => m.TimeIn,
                        //     m => m.TimeOut);
                    }

                    // Use default value if null
                    request.Parameters.Page = request.Parameters.Page ?? DEFAULT_PAGE;
                    request.Parameters.RowsPerPage = request.Parameters.RowsPerPage ?? DEFAULT_ROWS_PER_PAGE;

                    // Count total items
                    request.Parameters.TotalItems = _context.Logs.Count();

                    // Paginated list
                    var result = await queryableModel
                        .ToPagedList(request.Parameters)
                        .ToListAsync(cancellationToken);

                    return new 
                    {
                        meta = request.Parameters,
                        data = result
                    };
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}