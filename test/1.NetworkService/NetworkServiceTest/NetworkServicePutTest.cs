using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NetworkService.Contracts.Adapters;
using NetworkService.Contracts.Models.Exceptions.Basic;
using NetworkService.Contracts.Models.Exceptions.Specifics;
using NetworkService.Impl.Models;
using NetworkServiceTest.Helpers;
using NetworkServiceTest.Model;
using System;
using System.Net;
using System.Threading.Tasks;

namespace NetworkServiceTest
{
    [TestClass]
    public class NetworkServicePutTest
    {
        #region Fields

        private NetworkService.Impl.NetworkService _networkService;
        private Mock<IHttpClientAdapter> _httpClient;

        #endregion

        [TestInitialize]
        public void Startup()
        {
            _httpClient = new Mock<IHttpClientAdapter>();
            _networkService = new NetworkService.Impl.NetworkService(_httpClient.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [TestCategory("NetworkService")]
        public async Task WhenCallPutWithoutUrl_ShouldThrowException()
        {
            InitializeResponseOK(HttpStatusCode.OK);

            var response = await _networkService.Put<UserModel, object, ErrorTestModel>(null, null);

            Assert.Fail();
        }

        [TestMethod]
        [TestCategory("NetworkService")]
        public async Task WhenCallPutWithoutParamsIsOK_ShouldReturnsObjectDeserialized()
        {
            InitializeResponseOK(HttpStatusCode.OK);

            var response = await _networkService.Put<UserModel, object, ErrorTestModel>(TestConstanst.TEST_URL, null);

            Assert.IsTrue(response is UserModel);
            Assert.AreEqual(response.Name, TestConstanst.TEST_USERNAME);
        }

        [TestMethod]
        [TestCategory("NetworkService")]
        public async Task WhenCallPutIsOK_ShouldReturnsObjectDeserialized()
        {
            InitializeResponseOK(HttpStatusCode.OK);

            var userModel = new UserModel() { Name = TestConstanst.TEST_USERNAME };

            var response = await _networkService.Put<UserModel, UserModel, ErrorTestModel>(TestConstanst.TEST_URL, userModel);

            Assert.IsTrue(response is UserModel);
            Assert.AreEqual(response.Name, TestConstanst.TEST_USERNAME);
        }

        [TestMethod]
        [TestCategory("NetworkService")]
        public async Task WhenCallPutIsInternalServerError_ShouldThrowException()
        {
            InitializeResponseError(HttpStatusCode.InternalServerError);

            var userModel = new UserModel() { Name = TestConstanst.TEST_USERNAME };

            try
            {
                var response = await _networkService.Put<UserModel, UserModel, ErrorTestModel>(TestConstanst.TEST_URL, userModel);
                Assert.Fail();
            }
            catch (NetworkServiceInternalServerErrorException e)
            {
                Assert.AreEqual(e.Message, TestConstanst.TEST_ERROR_MESSAGE);
            }
        }

        [TestMethod]
        [TestCategory("NetworkService")]
        public async Task WhenCallPutIsBadRequest_ShouldThrowException()
        {
            InitializeResponseError(HttpStatusCode.BadRequest);

            var userModel = new UserModel();

            try
            {
                var response = await _networkService.Put<UserModel, UserModel, ErrorTestModel>(TestConstanst.TEST_URL, userModel);
                Assert.Fail();
            }
            catch (NetworkServiceBadRequestException e)
            {
                Assert.AreEqual(e.Message, TestConstanst.TEST_ERROR_MESSAGE);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NetworkServiceNotFoundException))]
        [TestCategory("NetworkService")]
        public async Task WhenCallPutIsNotFound_ShouldThrowException()
        {
            InitializeResponseError(HttpStatusCode.NotFound);

            var userModel = new UserModel();

            var response = await _networkService.Put<UserModel, UserModel, ErrorTestModel>(TestConstanst.TEST_URL, userModel);
        }

        [TestMethod]
        [ExpectedException(typeof(NetworkServiceUnauthorizedException))]
        [TestCategory("NetworkService")]
        public async Task WhenCallPutIsUnauthorized_ShouldThrowException()
        {
            InitializeResponseError(HttpStatusCode.Unauthorized);

            var userModel = new UserModel();

            var response = await _networkService.Put<UserModel, UserModel, ErrorTestModel>(TestConstanst.TEST_URL, userModel);
        }

        [TestMethod]
        [ExpectedException(typeof(NetworkServiceForbiddenException))]
        [TestCategory("NetworkService")]
        public async Task WhenCallPutIsForbidden_ShouldThrowException()
        {
            InitializeResponseError(HttpStatusCode.Forbidden);

            var userModel = new UserModel();

            var response = await _networkService.Put<UserModel, UserModel, ErrorTestModel>(TestConstanst.TEST_URL, userModel);
        }

        [TestMethod]
        [TestCategory("NetworkService")]
        public async Task WhenCallPutIs100Responds_ShouldThrowExceptionWithResponse()
        {
            var httpInformationalCodes = GenerateRespondeHelper.GetRangeCodes(HttpCodeGroupEnum.Informational);

            foreach (var httpCode in httpInformationalCodes)
            {
                InitializeResponseOK(httpCode);

                var userModel = new UserModel() { Name = TestConstanst.TEST_USERNAME };

                try
                {
                    var response = await _networkService.Put<UserModel, UserModel, ErrorTestModel>(TestConstanst.TEST_URL, userModel);
                    Assert.Fail();
                }
                catch (NetworkServiceInformationalException<UserModel> e)
                {
                    Assert.AreEqual(e.Response.Name, TestConstanst.TEST_USERNAME);
                    Assert.AreEqual(e.StatusCode, httpCode);
                }
            }
        }

        [TestMethod]
        [TestCategory("NetworkService")]
        public async Task WhenCallPutIs200Responds_ShouldReturnsObjectDeserialized()
        {
            var httpSuccessfulCodes = GenerateRespondeHelper.GetRangeCodes(HttpCodeGroupEnum.Successful);

            foreach (var httpCode in httpSuccessfulCodes)
            {
                InitializeResponseOK(httpCode);

                var userModel = new UserModel();

                var response = await _networkService.Put<UserModel, UserModel, ErrorTestModel>(TestConstanst.TEST_URL, userModel);

                if (httpCode != HttpStatusCode.NoContent)
                {
                    Assert.IsTrue(response is UserModel);
                    Assert.AreEqual(response.Name, TestConstanst.TEST_USERNAME);
                }
                else
                {
                    Assert.IsTrue(response == null);
                }
            }
        }

        [TestMethod]
        [TestCategory("NetworkService")]
        public async Task WhenCallPutIs300Responds_ShouldThrowExceptionWithResponse()
        {
            var httpRedirectCodes = GenerateRespondeHelper.GetRangeCodes(HttpCodeGroupEnum.Redirection);

            foreach (var httpCode in httpRedirectCodes)
            {
                InitializeResponseOK(httpCode);

                var userModel = new UserModel() { Name = TestConstanst.TEST_USERNAME };

                try
                {
                    var response = await _networkService.Put<UserModel, UserModel, ErrorTestModel>(TestConstanst.TEST_URL, userModel);
                    Assert.Fail();
                }
                catch (NetworkServiceRedirectionException<UserModel> e)
                {
                    Assert.AreEqual(e.Response.Name, TestConstanst.TEST_USERNAME);
                    Assert.AreEqual(e.StatusCode, httpCode);
                }
            }
        }

        [TestMethod]
        [TestCategory("NetworkService")]
        public async Task WhenCallPutIs400Responds_ShouldThrowException()
        {
            var httpClientErrorCodes = GenerateRespondeHelper.GetRangeCodes(HttpCodeGroupEnum.ClientError);

            foreach (var httpCode in httpClientErrorCodes)
            {
                InitializeResponseError(httpCode);

                var userModel = new UserModel() { Name = TestConstanst.TEST_USERNAME };

                try
                {
                    var response = await _networkService.Put<UserModel, UserModel, ErrorTestModel>(TestConstanst.TEST_URL, userModel);
                    Assert.Fail();
                }
                catch (NetworkServiceException)
                {

                }
            }
        }

        [TestMethod]
        [TestCategory("NetworkService")]
        public async Task WhenCallPutIs500Responds_ShouldThrowException()
        {
            var httpServerErrorCodes = GenerateRespondeHelper.GetRangeCodes(HttpCodeGroupEnum.ServerError);

            foreach (var httpCode in httpServerErrorCodes)
            {
                InitializeResponseError(httpCode);

                var userModel = new UserModel() { Name = TestConstanst.TEST_USERNAME };

                try
                {
                    var response = await _networkService.Put<UserModel, UserModel, ErrorTestModel>(TestConstanst.TEST_URL, userModel);
                    Assert.Fail();
                }
                catch (NetworkServiceException)
                {

                }
            }
        }

        #region Private Methods

        private void InitializeResponseOK(HttpStatusCode code)
        {
            _httpClient.Setup(http => http.PutAsJsonAsync(It.IsAny<string>(), It.IsAny<object>()))
                       .Returns(Task.FromResult(GenerateRespondeHelper.GetResponseOK(code)));
        }

        private void InitializeResponseError(HttpStatusCode code)
        {
            _httpClient.Setup(http => http.PutAsJsonAsync(It.IsAny<string>(), It.IsAny<object>()))
                       .Returns(Task.FromResult(GenerateRespondeHelper.GetResponseError(code)));
        }

        #endregion
    }
}
