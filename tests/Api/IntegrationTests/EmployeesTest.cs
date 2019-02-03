using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Test.Api.Models;
using Test.Api.Utils;
using WebApi.Constants;
using WebApi.Features.Auth;
using WebApi.Features.Employees;
using Xunit;

namespace Test.Api
{
    public class EmployeeTest : IClassFixture<TestServerFixture>
    {
        private const string API_URL = "api/employee";
        private readonly TestServerFixture _fixture;

        public EmployeeTest(TestServerFixture fixture)
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
        public async Task GetList_ThenReturnOk()
        {
            // Arrange
            var totalEmploeeFromDb = _fixture.Context.Employees.Count();

            // Act
            var response = await _fixture.Client.GetAsync(API_URL);
            var result = await response.Content.ReadAsStringAsync();
            var resultModel = JsonConvert.DeserializeObject<ICollection<EmployeeViewModel>>(result);

            // Assert
            response.EnsureSuccessStatusCode();

            var resultCount = resultModel.Count();
            Assert.Equal(totalEmploeeFromDb, resultCount);
        }

        [Fact]
        public async Task GetDetails_ThenReturnOk()
        {
            // Arrange
            var model = _fixture.Context.Employees.First();
            var employeeId = model.Id;

            // Act
            var response = await _fixture.Client.GetAsync($"{API_URL}/{employeeId}");
            var result = await response.Content.ReadAsStringAsync();
            var resultModel = JsonConvert.DeserializeObject<EmployeeViewModel>(result);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(model.FullName, resultModel.FullName);
            Assert.Equal(model.Position, resultModel.Position);
            Assert.Equal(model.CardNo, resultModel.CardNo);
            Assert.Equal(model.Status, resultModel.Status);
            Assert.Equal(model.IdentityId, resultModel.IdentityId);
        }

        [Fact]
        public async Task GivenValidEmployee_WhenPutRequest_ThenReturnOkWithUpdatedModel()
        {
            // Arrange
            var testModel = new EmployeeTestModel();
            var model = _fixture.Context.Employees.Last();
            model.FullName = testModel.FullName;
            model.CardNo = testModel.CardNo;
            model.Position = testModel.Position;

            // Act
            var response = await _fixture.Client.PutAsJsonAsync(API_URL, model);

            // Assert
            response.EnsureSuccessStatusCode();

            var updatedModel = _fixture.Context.Employees.Find(model.Id);
            Assert.Equal(model, updatedModel);
        }

        [Theory]
        [InlineData("", "full_name", "card_no")]
        [InlineData("718cae62-3856-4620-bada-321cc66023f0", "", "card_no")]
        [InlineData("718cae62-3856-4620-bada-321cc66023f0", "full_name", "")]
        [InlineData("718cae62-3856-4620-bada-321cc66023f0", "full_name", "existing_card_no")]
        public async Task GivenInvalidEmployee_WhenPutRequest_ThenReturnBadRequest(string identityId, string fullName, string cardNo)
        {
            // Arrange
            var testModel = new EmployeeTestModel();
            var model = _fixture.Context.Employees.Last();
            
            model.IdentityId = identityId ?? model.IdentityId;
            model.FullName = fullName ?? model.FullName;
            model.CardNo = cardNo ?? model.CardNo;

            if(cardNo == "existing_card_no")
            {
                var existingCardNo = _fixture.Context.Employees.First().CardNo;
                model.CardNo = existingCardNo;
            }

            // Act
            var response = await _fixture.Client.PutAsJsonAsync(API_URL, model);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}