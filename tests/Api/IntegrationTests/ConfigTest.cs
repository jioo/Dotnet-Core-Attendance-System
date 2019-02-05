using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Test.Api.ClassData;
using Test.Api.Utils;
using WebApi.Constants;
using WebApi.Features.Auth;
using WebApi.Features.Config;
using Xunit;

namespace Test.Api
{
    public class ConfigTest : IClassFixture<TestServerFixture>
    {
        private const string API_URL = "api/config";
        private readonly TestServerFixture _fixture;

        public ConfigTest(TestServerFixture fixture)
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
        public async Task GetDetails_ThenReturnOk()
        {
            // Act
            var response = await _fixture.Client.GetAsync(API_URL);
            var result = await response.Content.ReadAsStringAsync();
            var resultModel = JsonConvert.DeserializeObject<ConfigViewModel>(result);

            // Assert
            response.EnsureSuccessStatusCode();

            var updatedModel = _fixture.Context.Config.First();
            Assert.Equal(updatedModel.Id, resultModel.Id);
            Assert.Equal(updatedModel.TimeIn, resultModel.TimeIn);
            Assert.Equal(updatedModel.TimeOut, resultModel.TimeOut);
            Assert.Equal(updatedModel.GracePeriod, resultModel.GracePeriod);
        }

        [Fact]
        public async Task PutRequest_ThenReturnOk()
        {
            // Arrange
            var model = _fixture.Context.Config.First();
            model.TimeIn = "09:30";
            model.TimeOut = "17:15";
            model.GracePeriod = 5;

            // Act
            var response = await _fixture.Client.PutAsJsonAsync(API_URL, model);

            // Assert
            response.EnsureSuccessStatusCode();

            var updatedModel = _fixture.Context.Config.First();
            Assert.Equal(updatedModel.TimeIn, model.TimeIn);
            Assert.Equal(updatedModel.TimeOut, model.TimeOut);
            Assert.Equal(updatedModel.GracePeriod, model.GracePeriod);
        }

        [Theory]
        [ClassData(typeof(InvalidConfigModels))]
        public async Task GivenInvalidConfigModels_WhenPutRequest_ThenReutrnBadRequest(ConfigViewModel viewModel)
        {
            // Act
            var response = await _fixture.Client.PutAsJsonAsync(API_URL, viewModel);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}