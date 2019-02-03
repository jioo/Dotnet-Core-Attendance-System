using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Test.Api.ClassData;
using Test.Api.Utils;
using WebApi.Constants;
using WebApi.Features.Auth;
using Xunit;

namespace Test.Api
{
    public class AuthTest : IClassFixture<TestServerFixture>
    {
        private const string API_URL = "api/auth";
        private readonly TestServerFixture _fixture;

        public AuthTest(TestServerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ValidAdminLogin_ThenReturnOk()
        {
            // Arrange
            var credentials = new LoginViewModel
            {
                UserName = DefaultAdmin.UserName,
                Password = DefaultAdmin.Password,
            };

            // Act
            var response = await _fixture.Client.PostAsJsonAsync($"{API_URL}/login", credentials);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("", "password")]
        [InlineData("username", "")]
        [InlineData("admin", "invalid_password")]
        [InlineData("invalid_username", "password")]
        public async Task GivenInvalidModels_WhenPostLogin_ThenReturnBadRequest(string username, string password)
        {
            // Arrange
            var credentials = new LoginViewModel
            {
                UserName = username,
                Password = password,
            };

            // Act
            var response = await _fixture.Client.PostAsJsonAsync($"{API_URL}/login", credentials);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("Employee")]
        public async Task GivenValidAccountsPerRole_WhenPostChallenge_ThenReturnOk(string role)
        {
            // Arrange
            var userName = DefaultAdmin.UserName;
            if (role == "Employee")
            {
                var employee = _fixture.Context.Users.First(m => m.UserName != DefaultAdmin.UserName);
                userName = employee.UserName;
            }
            
            var credentials = new LoginViewModel
            {
                UserName = userName,
                Password = "123456",
            };

            // Act
            var request = new HttpRequestMessage(HttpMethod.Post, $"{API_URL}/challenge/{role}");
            await AuthExtensions.SetupRequestAuth(request, _fixture, credentials);

            var response = await _fixture.Client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GivenNoJwtAuth_WhenPostChallenge_ThenReturnUnauthorized()
        {
            // Act
            var response = await _fixture.Client.PostAsJsonAsync($"{API_URL}/challenge", "");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [ClassData(typeof(AuthorizedEnpoints))]
        public async Task GivenProtectedEndpoints_WhenRequestAsUnauthorized_ThenReturnUnauthorized(HttpMethod method, string url)
        {
            // Act
            var request = new HttpRequestMessage(method, url);
            var response = await _fixture.Client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [ClassData(typeof(AdminOnlyEndpoints))]
        public async Task GivenAuthAsEmployee_WhenRequestInAdminOnlyEndpoints_ThenReturnForbidden(HttpMethod method, string url)
        {
            // Arrange
            var employee = _fixture.Context.Users.First(m => m.UserName != DefaultAdmin.UserName);
            var credentials = new LoginViewModel
            {
                UserName = employee.UserName,
                Password = "123456",
            };

            // Act
            var request = new HttpRequestMessage(method, url);
            await AuthExtensions.SetupRequestAuth(request, _fixture, credentials);
            var response = await _fixture.Client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

    }
}