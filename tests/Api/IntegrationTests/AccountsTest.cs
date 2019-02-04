using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Test.Api.ClassData;
using Test.Api.Models;
using Test.Api.Utils;
using WebApi.Constants;
using WebApi.Entities;
using WebApi.Features.Accounts;
using WebApi.Features.Auth;
using Xunit;

namespace Test.Api
{
    public class AccountsTest : IClassFixture<TestServerFixture>
    {
        private const string API_URL = "api/accounts";
        private readonly TestServerFixture _fixture;

        public AccountsTest(TestServerFixture fixture)
        {
            _fixture = fixture;

            // Set up Default Auth as Admin
            var userCredentials = new LoginViewModel
            {
                UserName = DefaultAdmin.UserName,
                Password = DefaultAdmin.Password
            };
            AuthExtensions.SetupJwtAuth(_fixture, userCredentials).Wait();
        }

        [Fact]
        public async Task GivenValidModel_WhenPostRegisterRequest_ThenReturnOk()
        {
            // Arrange
            var model = new EmployeeTestModel();

            // Act
            var response = await _fixture.Client.PostAsJsonAsync($"{API_URL}/register", model);

            // Assert
            response.EnsureSuccessStatusCode();

            var updatedModel = await _fixture.Context.Employees.FirstAsync(m => m.FullName == model.FullName);
            Assert.Equal(updatedModel.FullName, model.FullName);
            Assert.Equal(updatedModel.CardNo, model.CardNo);
            Assert.Equal(updatedModel.Position, model.Position);

            var assertAccount = await AuthExtensions.GetJwt(
                _fixture, 
                new LoginViewModel 
                {
                    UserName = model.UserName,
                    Password = model.Password
                }
            );
            Assert.NotEmpty(assertAccount);
        }

        [Theory]
        [ClassData(typeof(InvalidRegisterModels))]
        public async Task GivenInvalidModels_WhenPostRegisterRequest_ThenReturnBadRequest(RegisterViewModel viewModel)
        {
            // Arrange
            if (viewModel.UserName == "existing_username")
            {
                var existingUserName = _fixture.Context.Users.Last().UserName;
                viewModel.UserName = existingUserName;
            }

            if (viewModel.CardNo == "existing_cardNo")
            {
                var existingCardNo = _fixture.Context.Employees.Last().CardNo;
                viewModel.CardNo = existingCardNo;
            }

            // Act
            var response = await _fixture.Client.PostAsJsonAsync($"{API_URL}/register", viewModel);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GivenEmployeeModel_WhenPutUpdatePasswordRequest_ThenReturnOk()
        {
            // Arrange
            const string NEW_PASSWORD = "654321";

            var model = _fixture.Context.Users.ToList().ElementAt(5);
            var viewModel = new UpdatePasswordViewModel
            {
                UserName = model.UserName,
                Password = NEW_PASSWORD,
            };

            // Act
            var response = await _fixture.Client.PutAsJsonAsync($"{API_URL}/update-password", viewModel);

            // Assert
            response.EnsureSuccessStatusCode();

            var assertLogin = await AuthExtensions.GetJwt(
                _fixture, 
                new LoginViewModel
                {
                    UserName = model.UserName,
                    Password = NEW_PASSWORD,
                }
            );
            Assert.NotEmpty(assertLogin);
        }

        [Theory]
        [InlineData("", "valid_password")]
        [InlineData("valid_username", "")]
        [InlineData("invalid_username", "valid_password")]
        // error: min length for password 6
        [InlineData("valid_username", "12345")] 
        public async Task GivenInvalidViewModels_WhenPutUpdatePasswordRequest_ThenReturnBadRequest(string username, string password)
        {
            // Arrange
            var viewModel = new UpdatePasswordViewModel
            {
                UserName = username,
                Password = password,
            };

            if (username == "valid_username")
            {
                var validUserName = _fixture.Context.Users.Last().UserName;
                viewModel.UserName = validUserName;
            }

            // Act
            var response = await _fixture.Client.PutAsJsonAsync($"{API_URL}/update-password", viewModel);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GivenAuthByEmployee_WhenPutChangePasswordRequest_ThenReturnOk()
        {
            // Arrange
            const string NEW_PASSWORD = "654321";
            const string DEFAULT_PASSWORD = DefaultAdmin.Password;

            var model = _fixture.Context.Users.Last();
            var viewModel = await Task.Run(() => JsonConvert.SerializeObject(
                new ChangePasswordViewModel
                {
                    OldPassword = DEFAULT_PASSWORD,
                    NewPassword = NEW_PASSWORD,
                }
            ));

            // Act
            var request = new HttpRequestMessage(HttpMethod.Put, $"{API_URL}/change-password");
            // Send request as Employee
            request.Content = new StringContent(viewModel, Encoding.UTF8, "application/json");

            await AuthExtensions.SetupRequestAuth(
                request, 
                _fixture, 
                new LoginViewModel 
                {   
                    UserName = model.UserName,
                    Password = DEFAULT_PASSWORD,
                }
            );

            var response = await _fixture.Client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();

            var assertLogin = await AuthExtensions.GetJwt(
                _fixture, 
                new LoginViewModel
                {
                    UserName = model.UserName,
                    Password = NEW_PASSWORD,
                }
            );
            Assert.NotEmpty(assertLogin);
        }

        [Theory]
        [InlineData("", "valid_password")]
        [InlineData("valid_old_password", "")]
        [InlineData("invalid_old_password", "valid_password")]
        // error: min length for password 6
        [InlineData("valid_old_password", "12345")] 
        public async Task GivenInvalidModels_WhenPutChangePasswordRequest_ThenReturnBadRequest(string oldPassword, string newPassword)
        {
            // Arrange
            var viewModel = new ChangePasswordViewModel
            {
                OldPassword = oldPassword,
                NewPassword = newPassword,
            };

            if (oldPassword == "valid_old_password")
                viewModel.OldPassword = DefaultAdmin.Password;
            
            // Act
            var response = await _fixture.Client.PutAsJsonAsync($"{API_URL}/change-password", viewModel);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}