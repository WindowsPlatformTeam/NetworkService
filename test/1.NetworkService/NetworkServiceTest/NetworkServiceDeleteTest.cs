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
    public class NetworkServiceDeleteTest
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
        public async Task WhenCallDeleteWithoutUrl_ShouldThrowAnException()
        {
            InitializeResponseOK(HttpStatusCode.OK);

            var response = await _networkService.Delete<UserModel, ErrorTestModel>(null);

            Assert.Fail();
        }

        [TestMethod]
        [TestCategory("NetworkService")]
        public async Task WhenCallDeleteIsOK_ShouldReturnsObjectDeserialized()
        {
            InitializeResponseOK(HttpStatusCode.OK);

            var response = await _networkService.Delete<UserModel, ErrorTestModel>(TestConstanst.TEST_URL);

            Assert.IsTrue(response is UserModel);
            Assert.AreEqual(response.Name, TestConstanst.TEST_USERNAME);
        }

        [TestMethod]
        [TestCategory("NetworkService")]
        public async Task WhenCallDeleteIsInternalServerError_ShouldThrowException()
        {
            InitializeResponseError(HttpStatusCode.InternalServerError);

            try
            {
                var response = await _networkService.Delete<UserModel, ErrorTestModel>(TestConstanst.TEST_URL);
                Assert.Fail();
            }
            catch (NetworkServiceInternalServerErrorException e)
            {
                Assert.AreEqual(e.Message, TestConstanst.TEST_ERROR_MESSAGE);
            }
        }

        [TestMethod]
        [TestCategory("NetworkService")]
        public async Task WhenCallDeleteIsBadRequest_ShouldThrowException()
        {
            InitializeResponseError(HttpStatusCode.BadRequest);

            try
            {
                var response = await _networkService.Delete<UserModel, ErrorTestModel>(TestConstanst.TEST_URL);
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
        public async Task WhenCallDeleteIsNotFound_ShouldThrowException()
        {
            InitializeResponseError(HttpStatusCode.NotFound);

            var response = await _networkService.Delete<UserModel, ErrorTestModel>(TestConstanst.TEST_URL);
        }

        [TestMethod]
        [ExpectedException(typeof(NetworkServiceUnauthorizedException))]
        [TestCategory("NetworkService")]
        public async Task WhenCallDeleteIsUnauthorized_ShouldThrowException()
        {
            InitializeResponseError(HttpStatusCode.Unauthorized);

            var response = await _networkService.Delete<UserModel, ErrorTestModel>(TestConstanst.TEST_URL);
        }

        [TestMethod]
        [ExpectedException(typeof(NetworkServiceForbiddenException))]
        [TestCategory("NetworkService")]
        public async Task WhenCallDeleteIsForbidden_ShouldThrowException()
        {
            InitializeResponseError(HttpStatusCode.Forbidden);

            var response = await _networkService.Delete<UserModel, ErrorTestModel>(TestConstanst.TEST_URL);
        }

        [TestMethod]
        [TestCategory("NetworkService")]
        public async Task WhenCallDeleteIs100Responds_ShouldThrowExceptionWithResponse()
        {
            var httpInformationalCodes = GenerateRespondeHelper.GetRangeCodes(HttpCodeGroupEnum.Informational);

            foreach (var httpCode in httpInformationalCodes)
            {
                InitializeResponseOK(httpCode);

                try
                {
                    var response = await _networkService.Delete<UserModel, ErrorTestModel>(TestConstanst.TEST_URL);
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
        public async Task WhenCallDeleteIs200Responds_ShouldReturnsObjectDeserialized()
        {
            var httpSucessfulCodes = GenerateRespondeHelper.GetRangeCodes(HttpCodeGroupEnum.Successful);

            foreach (var httpCode in httpSucessfulCodes)
            {
                InitializeResponseOK(httpCode);

                var response = await _networkService.Delete<UserModel, ErrorTestModel>(TestConstanst.TEST_URL);

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
        public async Task WhenCallDeleteIs300Responds_ShouldThrowExceptionWithResponse()
        {
            var httpRedirectCodes = GenerateRespondeHelper.GetRangeCodes(HttpCodeGroupEnum.Redirection);

            foreach (var httpCode in httpRedirectCodes)
            {
                InitializeResponseOK(httpCode);

                try
                {
                    var response = await _networkService.Delete<UserModel, ErrorTestModel>(TestConstanst.TEST_URL);
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
        public async Task WhenCallDeleteIs400Responds_ShouldThrowException()
        {
            var httpClientErrorCodes = GenerateRespondeHelper.GetRangeCodes(HttpCodeGroupEnum.ClientError);

            foreach (var httpCode in httpClientErrorCodes)
            {
                InitializeResponseError(httpCode);

                try
                {
                    var response = await _networkService.Delete<UserModel, ErrorTestModel>(TestConstanst.TEST_URL);
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
        public async Task WhenCallDeleteIs500Responds_ShouldThrowException()
        {
            var httpServerErrorCodes = GenerateRespondeHelper.GetRangeCodes(HttpCodeGroupEnum.ServerError);

            foreach (var httpCode in httpServerErrorCodes)
            {
                InitializeResponseError(httpCode);

                try
                {
                    var response = await _networkService.Delete<UserModel, ErrorTestModel>(TestConstanst.TEST_URL);
                    Assert.Fail();
                }
                catch (NetworkServiceServerErrorException e)
                {
                    Assert.AreEqual(e.StatusCode, httpCode);
                    Assert.AreEqual(e.Message, TestConstanst.TEST_ERROR_MESSAGE);
                }
            }
        }

        #region Private Methods

        private void InitializeResponseOK(HttpStatusCode code)
        {
            _httpClient.Setup(http => http.DeleteAsync(It.IsAny<string>()))
                       .Returns(Task.FromResult(GenerateRespondeHelper.GetResponseOK(code)));
        }

        private void InitializeResponseError(HttpStatusCode code)
        {
            _httpClient.Setup(http => http.DeleteAsync(It.IsAny<string>()))
                       .Returns(Task.FromResult(GenerateRespondeHelper.GetResponseError(code)));
        }

        #endregion
    }
}
