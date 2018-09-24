using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebApi.Entities;

namespace WebApi.Features.Auth
{
    public class IsUserExists
    {
        public class Query : IRequest<bool>
        {
            public Query(string username)
            {
                Username = username;
            }

            public string Username { get; }
        }

        public class QueryHandler : IRequestHandler<Query, bool>
        {
            private readonly UserManager<User> _manager;

            public QueryHandler(UserManager<User> manager)
            {
                _manager = manager;
            }
            public async Task<bool> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var result = await _manager.FindByNameAsync(request.Username);

                    // Returns true if result is not null
                    return result != null;
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