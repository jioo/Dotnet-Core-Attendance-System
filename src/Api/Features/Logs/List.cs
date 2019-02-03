using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using LinqKit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Extensions;
using WebApi.Infrastructure;
using X.PagedList;

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
                    var startDate = request.Parameters.StartDate;
                    var endDate = request.Parameters.EndDate;
                    var searchQuery = request.Parameters.Search;
                    
                    // Build dynamic clause
                    var predicate = PredicateBuilder.New<LogViewModel>(true); // true -where(true) return all
                    predicate = predicate.And(m => m.Deleted == null);

                    // Apply search filter if any
                    if (String.IsNullOrWhiteSpace(searchQuery) == false)
                    {
                        var searchTerms = searchQuery.Split(' ').ToList().ConvertAll(x => x.ToLower());
                        predicate = predicate.And(m => searchTerms.Any(key => m.FullName.ToLower().Contains(key)));
                    }

                    // Apply employee filter if account has `Employee` role
                    var isEmployee = _httpContext.HttpContext.User.IsInRole("Employee");
                    if (isEmployee) 
                    {
                        // Get the username in identity claims
                        var username = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                        // Get account details
                        var account = _context.Users.Include(m => m.Employee).First(m => m.UserName == username);
                        
                        predicate = predicate.And(m => m.EmployeeId == account.Employee.Id);
                    }

                    // Apply Date filter
                    if (startDate != null && endDate != null)
                    {
                        endDate = Convert.ToDateTime(endDate).AddHours(23).AddMinutes(59);
                        predicate = predicate.And(m => Convert.ToDateTime(m.TimeIn) >= startDate && Convert.ToDateTime(m.TimeIn) <= endDate);
                    }

                    var isDescending = request.Parameters.Descending ?? false;

                    // Use default value if null
                    request.Parameters.Page = request.Parameters.Page ?? DEFAULT_PAGE;
                    request.Parameters.RowsPerPage = request.Parameters.RowsPerPage ?? DEFAULT_ROWS_PER_PAGE;

                    // Count total items
                    request.Parameters.TotalItems = _context.Logs.MapToViewModel().Where(predicate).Count();

                    // Parse parameters to int
                    var pageSize = Convert.ToInt32(request.Parameters.RowsPerPage);
                    var pageNumber = Convert.ToInt32(request.Parameters.Page);

                    var result = await _context.Logs
                        .AsExpandable()
                        .MapToViewModel()
                        .Where(predicate)
                        .ApplySort(request.Parameters)
                        .ToPagedList(pageNumber, pageSize)
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