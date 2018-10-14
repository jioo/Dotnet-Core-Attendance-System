using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
            public Query(DateFilteredList parameters)
            {
                Parameters = parameters;
            }

            public DateFilteredList Parameters { get; }
        }

        public class QueryHandler : IRequestHandler<Query, Object>
        {
            private readonly int DEFAULT_PAGE = 1;
            private readonly int DEFAULT_ROWS_PER_PAGE = 10;
            private readonly ApplicationDbContext _context;
            private readonly IHttpContextAccessor _httpContext;
            private readonly UserManager<User> _manager;

            public QueryHandler(
                ApplicationDbContext context, 
                IHttpContextAccessor httpContext,
                UserManager<User> manager)
            {
                _context = context;
                _httpContext = httpContext;
                _manager = manager;
            }

            public async Task<Object> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    IQueryable<LogViewModel> queryableModel;
                    var startDate = request.Parameters.StartDate;
                    var endDate = request.Parameters.EndDate;
                    var searchQuery = request.Parameters.Search;

                    // Check if the current user is Employee
                    var isEmployee = _httpContext.HttpContext.User.IsInRole("Employee");
                    if (isEmployee) 
                    {
                        // Get the username in sub type claim
                        var username = _httpContext.HttpContext.User.Claims
                            .First(m => m.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
                            .Value;

                        // Get account details
                        var account = _context.Users
                            .Include(m => m.Employee)
                            .First(m => m.UserName == username);

                        queryableModel = _context.Logs.MapToViewModel()
                            .Where(m => 
                                m.EmployeeId == account.Employee.Id &&
                                m.Deleted == null);
                    }
                    // Apply Search filter
                    else if(!string.IsNullOrEmpty(searchQuery))
                    {
                        queryableModel = _context.Logs.MapToViewModel()
                            .Where(m => 
                                m.FullName.Contains(searchQuery) &&
                                m.Deleted == null);
                    }
                    // Get all List
                    else
                    {
                        queryableModel = _context.Logs.MapToViewModel()
                            .Where(m => m.Deleted == null);
                    }

                    // Apply Date filter
                    if (startDate != null && endDate != null)
                    {
                        endDate = Convert.ToDateTime(endDate).AddHours(23).AddMinutes(59);
                        queryableModel = queryableModel.Where(m => Convert.ToDateTime(m.TimeIn) >= startDate && Convert.ToDateTime(m.TimeIn) <= endDate);
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
                    request.Parameters.TotalItems = queryableModel.Count();

                    // Paginated list
                    var result = await queryableModel
                        .ToPagedList(request.Parameters)
                        .OrderByDescending(m => m.Created)
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