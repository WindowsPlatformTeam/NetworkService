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
using System.Net.Http;
using System.Threading.Tasks;

namespace NetworkServiceTest
{
    [TestClass]
    public class NetworkServicePatchTest
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
        public async Task WhenCallPatchWithoutUrl_ShouldThrowAnException()
        {
            InitializeResponseOK(HttpStatusCode.OK);

            var response = await _networkService.Patch<UserModel, ErrorTestModel>(null, null);

            Assert.Fail();
        }

        [TestMethod]
        [TestCategory("NetworkService")]
        public async Task WhenCallPatchWithNullParamIsOK_ShouldReturnsObjectDeserialized()
        {
            InitializeResponseOK(HttpStatusCode.OK);

            var response = await _networkService.Patch<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, null);

            Assert.IsTrue(response is UserModel);
            Assert.AreEqual(response.Name, TestConstanst.TEST_USERNAME);
        }

        [TestMethod]
        [TestCategory("NetworkService")]
        public async Task WhenCallPatchIsOK_ShouldReturnsObjectDeserialized()
        {
            InitializeResponseOK(HttpStatusCode.OK);

            var userModel = new UserModel();

            var response = await _networkService.Patch<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, userModel);

            Assert.IsTrue(response is UserModel);
            Assert.AreEqual(response.Name, TestConstanst.TEST_USERNAME);
        }

        [TestMethod]
        [TestCategory("NetworkService")]
        public async Task WhenCallPatchIsInternalServerError_ShouldThrowException()
        {
            InitializeResponseError(HttpStatusCode.InternalServerError);

            var userModel = new UserModel();

            try
            {
                var response = await _networkService.Patch<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, userModel);
                Assert.Fail();
            }
            catch (NetworkServiceInternalServerErrorException e)
            {
                Assert.AreEqual(e.StatusCode, HttpStatusCode.InternalServerError);
                Assert.AreEqual(e.Message, TestConstanst.TEST_ERROR_MESSAGE);
            }
        }

        [TestMethod]
        [TestCategory("NetworkService")]
        public async Task WhenCallPatchIsBadRequest_ShouldThrowException()
        {
            InitializeResponseError(HttpStatusCode.BadRequest);

            var userModel = new UserModel();

            try
            {
                var response = await _networkService.Patch<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, userModel);
                Assert.Fail();
            }
            catch (NetworkServiceBadRequestException e)
            {
                Assert.AreEqual(e.StatusCode, HttpStatusCode.BadRequest);
                Assert.AreEqual(e.Message, TestConstanst.TEST_ERROR_MESSAGE);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NetworkServiceNotFoundException))]
        [TestCategory("NetworkService")]
        public async Task WhenCallPatchIsNotFound_ShouldThrowException()
        {
            InitializeResponseError(HttpStatusCode.NotFound);

            var userModel = new UserModel();

            var response = await _networkService.Patch<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, userModel);
        }

        [TestMethod]
        [ExpectedException(typeof(NetworkServiceUnauthorizedException))]
        [TestCategory("NetworkService")]
        public async Task WhenCallPatchIsUnauthorized_ShouldThrowException()
        {
            InitializeResponseError(HttpStatusCode.Unauthorized);

            var userModel = new UserModel();

            var response = await _networkService.Patch<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, userModel);
        }

        [TestMethod]
        [ExpectedException(typeof(NetworkServiceForbiddenException))]
        [TestCategory("NetworkService")]
        public async Task WhenCallPatchIsForbidden_ShouldThrowException()
        {
            InitializeResponseError(HttpStatusCode.Forbidden);

            var userModel = new UserModel();

            var response = await _networkService.Patch<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, userModel);
        }

        [TestMethod]
        [TestCategory("NetworkService")]
        public async Task WhenCallPatchReturn100RespondsWithResponse_ShouldThrowExceptionWithResponse()
        {
            var httpInformationalCodes = GenerateRespondeHelper.GetRangeCodes(HttpCodeGroupEnum.Informational);

            foreach (var httpCode in httpInformationalCodes)
            {
                InitializeResponseOK(httpCode);

                var userModel = new UserModel() { Name = TestConstanst.TEST_USERNAME };

                try
                {
                    var response = await _networkService.Patch<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, userModel);
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
        public async Task WhenCallPatchIs200Responds_ShouldReturnsObjectDeserialized()
        {
            var httpSucessfullCodes = GenerateRespondeHelper.GetRangeCodes(HttpCodeGroupEnum.Successful);

            foreach (var httpCode in httpSucessfullCodes)
            {
                InitializeResponseOK(httpCode);

                var userModel = new UserModel();

                var response = await _networkService.Patch<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, userModel);

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
        public async Task WhenCallPatchIs300Responds_ShouldThrowExceptionWithResponse()
        {
            var httpRedirectCodes = GenerateRespondeHelper.GetRangeCodes(HttpCodeGroupEnum.Redirection);

            foreach (var httpCode in httpRedirectCodes)
            {
                InitializeResponseOK(httpCode);

                var userModel = new UserModel() { Name = TestConstanst.TEST_USERNAME };

                try
                {
                    var response = await _networkService.Patch<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, userModel);
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
        public async Task WhenCallPatchIs400Responds_ShouldThrowExceptionWithError()
        {
            var httpClientErrorCodes = GenerateRespondeHelper.GetRangeCodes(HttpCodeGroupEnum.ClientError);

            foreach (var httpCode in httpClientErrorCodes)
            {
                InitializeResponseError(httpCode);

                var userModel = new UserModel() { Name = TestConstanst.TEST_USERNAME };

                try
                {
                    var response = await _networkService.Patch<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, userModel);
                    Assert.Fail();
                }
                catch (NetworkServiceClientErrorException e)
                {
                    Assert.AreEqual(e.StatusCode, httpCode);
                }
            }
        }

        [TestMethod]
        [TestCategory("NetworkService")]
        public async Task WhenCallPatchIs500Responds_ShouldThrowException()
        {
            var httpServerErrorCodes = GenerateRespondeHelper.GetRangeCodes(HttpCodeGroupEnum.ServerError);

            foreach (var httpCode in httpServerErrorCodes)
            {
                InitializeResponseError(httpCode);

                var userModel = new UserModel() { Name = TestConstanst.TEST_USERNAME };

                try
                {
                    var response = await _networkService.Patch<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, userModel);
                    Assert.Fail();
                }
                catch (NetworkServiceServerErrorException e)
                {
                    Assert.AreEqual(e.Message, TestConstanst.TEST_ERROR_MESSAGE);
                    Assert.AreEqual(e.StatusCode, httpCode);
                }
            }
        }

        #region Private Methods

        private void InitializeResponseOK(HttpStatusCode code)
        {
            _httpClient.Setup(http => http.SendAsync(It.IsAny<HttpRequestMessage>()))
                       .Returns(Task.FromResult(GenerateRespondeHelper.GetResponseOK(code)));
        }

        private void InitializeResponseError(HttpStatusCode code)
        {
            _httpClient.Setup(http => http.SendAsync(It.IsAny<HttpRequestMessage>()))
                       .Returns(Task.FromResult(GenerateRespondeHelper.GetResponseError(code)));
        }

        #endregion
    }
}
