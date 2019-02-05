using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;

namespace Test.Api.ClassData
{
    public class PublicEndpoints : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { HttpMethod.Post, "api/auth/login" };
            yield return new object[] { HttpMethod.Post, "api/log" };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}