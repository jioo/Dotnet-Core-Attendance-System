using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;
using Test.Api.Models;
using WebApi.Features.Auth;

namespace Test.Api.Utils
{
    public static class AuthExtensions
    {
        public static async Task<string> GetJwt(TestServerFixture fixture, LoginViewModel viewModel)
        {
            var response = await fixture.Client.PostAsJsonAsync("api/auth/login", viewModel);

            if (!response.IsSuccessStatusCode) return string.Empty;
            
            var result = await response.Content.ReadAsStringAsync();
            var resultModel = JsonConvert.DeserializeObject<LoginResponseModel>(result);

            return resultModel.AccessToken;
        }
        
        public static async Task SetupJwtAuth(TestServerFixture fixture, LoginViewModel viewModel)
        {
            var token = await GetJwt(fixture, viewModel);
            
            fixture.Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        public static async Task SetupRequestAuth(HttpRequestMessage request, TestServerFixture fixture, LoginViewModel viewModel)
        {
            var token = await GetJwt(fixture, viewModel);
            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
        }

        public static string GetCurrentUserId(TestServerFixture fixture, string username)
        {
            var currentUser = fixture.Context.Users.First(m => m.UserName == username);

            var id = currentUser.Id;

            return id;
        }
    }
}