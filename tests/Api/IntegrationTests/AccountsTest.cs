using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    }
}