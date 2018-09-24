using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace WebApi.Features.Auth
{
    public class GenerateXsrfToken
    {
        public class Query : IRequest<Object> 
        { 
            public Query(HttpContext httpContext)
            {
                HttpContext = httpContext;
            }

            public HttpContext HttpContext { get; }
        }

        public class QueryHandler : RequestHandler<Query, Object>
        {
            private readonly IAntiforgery _antiforgery;

            public QueryHandler(IAntiforgery antiforgery)
            {
                _antiforgery = antiforgery;
            }

            protected override object Handle(Query request)
            {
                try
                {
                    var tokens = _antiforgery.GetAndStoreTokens(request.HttpContext);

                    return new {
                        token = tokens.RequestToken,
                        tokenName = tokens.HeaderName
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