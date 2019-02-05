using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Test.Api.Utils;
using WebApi.Constants;
using WebApi.Entities;
using WebApi.Features.Auth;
using WebApi.Features.Logs;
using WebApi.Utils;
using Xunit;

namespace Test.Api
{
    public class LogTest : IClassFixture<TestServerFixture>
    {
        private const string API_URL = "api/log";
        private readonly TestServerFixture _fixture;

        public LogTest(TestServerFixture fixture)
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
        public async Task GivenAdminAuth_WhenGetRequest_ThenReturnOkWithAllLogs()
        {
            // Act
            var response = await _fixture.Client.GetAsync(API_URL);
            var result = await response.Content.ReadAsStringAsync();
            var resultModel = JsonConvert.DeserializeObject<ListResponse<LogViewModel>>(result);

            // Assert
            response.EnsureSuccessStatusCode();

            var totalList = _fixture.Context.Logs.Count();
            Assert.Equal(totalList, resultModel.Meta.TotalItems);
        }

        [Fact]
        public async Task GivenEmployeeAuth_WhenGetRequest_ThenReturnOkWithOwnLogs()
        {
            // Arrange
            var employeeAccount = _fixture.Context.Users.Last();
            var employeeDetails = _fixture.Context.Employees.Where(m => m.IdentityId == employeeAccount.Id).First();
            var admin = new LoginViewModel
            {
                UserName = employeeAccount.UserName,
                Password = DefaultAdmin.Password,
            };

            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, API_URL);
            await AuthExtensions.SetupRequestAuth(request, _fixture, admin);

            var response = await _fixture.Client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            var resultModel = JsonConvert.DeserializeObject<ListResponse<LogViewModel>>(result);

            // Assert
            response.EnsureSuccessStatusCode();

            var totalList = _fixture.Context.Logs.Where(m => m.EmployeeId == employeeDetails.Id).Count();
            Assert.Equal(totalList, resultModel.Meta.TotalItems);
        }

        [Fact]
        public async Task GivenAdminAuth_WhenGetDetail_ThenReturnOk()
        {
            // Arrange
            var model = _fixture.Context.Logs.MapToViewModel().First();

            // Act
            var response = await _fixture.Client.GetAsync($"{API_URL}/{model.Id}");
            var result = await response.Content.ReadAsStringAsync();
            var resultModel = JsonConvert.DeserializeObject<LogViewModel>(result);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(model.EmployeeId, resultModel.EmployeeId);
            Assert.Equal(model.FullName, resultModel.FullName);
            Assert.Equal(model.TimeIn, resultModel.TimeIn);
            Assert.Equal(model.TimeOut, resultModel.TimeOut);
        }

        [Fact]
        public async Task GivenAdminAuth_WhenPutRequest_ThenReturnOk()
        {
            // Arrange
            var model = _fixture.Context.Logs.MapToViewModel().First();
            model.TimeIn = "02/04/2019 9:06:10 AM";
            model.TimeOut = "02/04/2019 5:44:10 PM";

            // Act
            var response = await _fixture.Client.PutAsJsonAsync(API_URL, model);

            // Assert
            response.EnsureSuccessStatusCode();

            var updatedModel = _fixture.Context.Logs.Find(model.Id);
            Assert.Equal(LogExtensions.ToLocal(updatedModel.TimeIn), model.TimeIn.ToString());
            Assert.Equal(LogExtensions.ToLocal(updatedModel.TimeOut), model.TimeOut.ToString());
        }

        [Theory]
        [InlineData("valid_id", "", "02/04/2019 5:44:10 PM")]
        [InlineData("valid_id", "invalid_date", "02/04/2019 5:44:10 PM")]
        [InlineData("invalid_id", "02/04/2019 9:06:10 AM", "")]
        [InlineData("valid_id", "02/04/2019 9:06:10 AM", "invalid_date")]
        public async Task GivenInvalidModels_WhenPutRequest_ThenReturnBadRequest(string id, string timeIn, string timeOut)
        {
            // Arrange
            var model = _fixture.Context.Logs.MapToViewModel().Last();
            model.Id = (id == "valid_id") ? model.Id: Guid.Empty;
            model.TimeIn = timeIn;
            model.TimeOut = timeOut;

            // Act
            var response = await _fixture.Client.PutAsJsonAsync(API_URL, model);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PostLog_ThenReturnOkWithTimeIn()
        {
            // Arrange
            var employee = _fixture.Context.Employees.Last();
            var viewModel = new LogInOutViewModel
            {
                CardNo = employee.CardNo,
                Password = DefaultAdmin.Password,
            };

            // Act
            var response = await _fixture.Client.PostAsJsonAsync(API_URL, viewModel);

            // Assert
            response.EnsureSuccessStatusCode();

            var updatedLog = _fixture.Context.Logs.Where(m => m.EmployeeId == employee.Id).OrderByDescending(m => m.Created).First();
            Assert.NotNull(updatedLog.TimeIn);
            Assert.Null(updatedLog.TimeOut);
        }

        [Fact]
        public async Task PostLogTwoTimes_ThenReturnOkWithTimeInAndTimeOut()
        {
            // Arrange
            var employee = _fixture.Context.Employees.ToList().ElementAtOrDefault(3);
            var viewModel = new LogInOutViewModel
            {
                CardNo = employee.CardNo,
                Password = DefaultAdmin.Password,
            };

            // Act
            var timeInResponse = await _fixture.Client.PostAsJsonAsync(API_URL, viewModel);
            var timeOutResponse = await _fixture.Client.PostAsJsonAsync(API_URL, viewModel);

            // Assert
            timeOutResponse.EnsureSuccessStatusCode();

            var updatedLog = _fixture.Context.Logs.Where(m => m.EmployeeId == employee.Id).OrderByDescending(m => m.Created).First();
            Assert.NotNull(updatedLog.TimeIn);
            Assert.NotNull(updatedLog.TimeOut);
        }

        [Fact]
        public async Task GivenInactiveEmployee_WhenPostLog_ThenReturnBadRequest()
        {
            // Arrange
            var employee = _fixture.Context.Employees.ToList().ElementAtOrDefault(4);

            // Update the employee with inactive status
            employee.Status = Status.Inactive;
            _fixture.Context.Entry(employee).State = EntityState.Modified;
            await _fixture.Context.SaveChangesAsync();

            var viewModel = new LogInOutViewModel
            {
                CardNo = employee.CardNo,
                Password = DefaultAdmin.Password,
            };

            // Act
            var response = await _fixture.Client.PostAsJsonAsync(API_URL, viewModel);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("valid_cardNo", "")]
        [InlineData("valid_cardNo", "invalid_password")]
        [InlineData("invalid_cardNo", "valid_password")]
        [InlineData("", "valid_password")]
        public async Task GivenInvalidModels_WhenPostLog_ThenReturnBadRequest(string cardNo, string password)
        {
            // Arrange
            var employee = _fixture.Context.Employees.ToList().ElementAtOrDefault(4);
            var viewModel = new LogInOutViewModel
            {
                CardNo = (cardNo == "valid_cardNo") ? employee.CardNo : cardNo,
                Password = (password == "valid_password") ? DefaultAdmin.Password : password,
            };

            // Act
            var response = await _fixture.Client.PostAsJsonAsync(API_URL, viewModel);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}