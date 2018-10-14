using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebApi.Entities;

namespace WebApi.Features.Auth
{
    public class ValidatePassword
    {
        public class Query : IRequest<bool>
        {
            public Query(string username, string password)
            {
                Username = username;
                Password = password;
            }

            public string Username { get; }
            public string Password { get; }
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
                    // Get account details
                    var user = await _manager.FindByNameAsync(request.Username);

                    // Returns true if password is correct
                    return await _manager.CheckPasswordAsync(user, request.Password);
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