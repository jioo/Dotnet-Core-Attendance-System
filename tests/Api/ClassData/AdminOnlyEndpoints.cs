using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;

namespace Test.Api.ClassData
{
    public class AdminOnlyEndpoints : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { HttpMethod.Post, "api/accounts/register" };
            yield return new object[] { HttpMethod.Put, "api/accounts/update-password" };
            yield return new object[] { HttpMethod.Put, "api/config" };
            yield return new object[] { HttpMethod.Get, "api/employee" };
            yield return new object[] { HttpMethod.Put, "api/employee" };
            yield return new object[] { HttpMethod.Get, $"api/employee/{Guid.NewGuid()}" };
            yield return new object[] { HttpMethod.Put, "api/log" };
            yield return new object[] { HttpMethod.Get, $"api/log/{Guid.NewGuid()}" };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}