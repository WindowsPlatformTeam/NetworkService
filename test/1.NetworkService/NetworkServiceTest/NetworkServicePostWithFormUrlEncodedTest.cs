using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NetworkService.Contracts.Adapters;
using NetworkService.Contracts.Models.Exceptions.Basic;
using NetworkService.Contracts.Models.Exceptions.Specifics;
using NetworkService.Impl.Models;
using NetworkServiceTest.Helpers;
using NetworkServiceTest.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetworkServiceTest
{
    [TestClass]
    public class NetworkServicePostWithFormUrlEncodedTest
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
        [TestCategory("NetworkService")]
        public async Task WhenCallPostWithFormUrlEncodedIsOK_ShouldReturnsObjectDeserialized()
        {
            InitializeResponseOK(HttpStatusCode.OK);

            var dictionary = new Dictionary<string, string> {
                { "id", "username" }
            };

            var response = await _networkService.PostWithFormUrlEncoded<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, dictionary);

            Assert.IsTrue(response is UserModel);
            Assert.AreEqual(response.Name, TestConstanst.TEST_USERNAME);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [TestCategory("NetworkService")]
        public async Task WhenCallPostWithFormUrlEncodedWithoutUrl_ShouldThrowAnException()
        {
            InitializeResponseOK(HttpStatusCode.OK);

            var response = await _networkService.PostWithFormUrlEncoded<UserModel, ErrorTestModel>(null, null);

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [TestCategory("NetworkService")]
        public async Task WhenCallPostWithFormUrlEncodedParameterNullIsOK_ShouldReturnsObjectDeserialized()
        {
            InitializeResponseOK(HttpStatusCode.OK);

            var response = await _networkService.PostWithFormUrlEncoded<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, null);

            Assert.Fail();
        }

        [TestMethod]
        [TestCategory("NetworkService")]
        public async Task WhenCallPostWithFormUrlEncodedIsInternalServerError_ShouldThrowException()
        {
            InitializeResponseError(HttpStatusCode.InternalServerError);

            var dictionary = new Dictionary<string, string> {
                { "id", "username" }
            };

            try
            {
                var response = await _networkService.PostWithFormUrlEncoded<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, dictionary);
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
        public async Task WhenCallPostWithFormUrlEncodedIsBadRequest_ShouldThrowException()
        {
            InitializeResponseError(HttpStatusCode.BadRequest);

            var dictionary = new Dictionary<string, string> {
                { "id", "username" }
            };

            try
            {
                var response = await _networkService.PostWithFormUrlEncoded<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, dictionary);
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
        public async Task WhenCallPostWithFormUrlEncodedIsNotFound_ShouldThrowException()
        {
            InitializeResponseError(HttpStatusCode.NotFound);

            var dictionary = new Dictionary<string, string> {
                { "id", "username" }
            };

            var response = await _networkService.PostWithFormUrlEncoded<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, dictionary);
        }

        [TestMethod]
        [ExpectedException(typeof(NetworkServiceUnauthorizedException))]
        [TestCategory("NetworkService")]
        public async Task WhenCallPostWithFormUrlEncodedIsUnauthorized_ShouldThrowException()
        {
            InitializeResponseError(HttpStatusCode.Unauthorized);

            var dictionary = new Dictionary<string, string> {
                { "id", "username" }
            };

            var response = await _networkService.PostWithFormUrlEncoded<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, dictionary);
        }

        [TestMethod]
        [ExpectedException(typeof(NetworkServiceForbiddenException))]
        [TestCategory("NetworkService")]
        public async Task WhenCallPostWithFormUrlEncodedIsForbidden_ShouldThrowException()
        {
            InitializeResponseError(HttpStatusCode.Forbidden);

            var dictionary = new Dictionary<string, string> {
                { "id", "username" }
            };

            var response = await _networkService.PostWithFormUrlEncoded<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, dictionary);
        }

        [TestMethod]
        [TestCategory("NetworkService")]
        public async Task WhenCallPostWithFormUrlEncodedReturn100RespondsWithResponse_ShouldThrowExceptionWithResponse()
        {
            var httpInformationalCodes = GenerateRespondeHelper.GetRangeCodes(HttpCodeGroupEnum.Informational);

            foreach (var httpCode in httpInformationalCodes)
            {
                InitializeResponseOK(httpCode);

                var dictionary = new Dictionary<string, string> {
                    { "id", "username" }
                };

                try
                {
                    var response = await _networkService.PostWithFormUrlEncoded<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, dictionary);
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
        public async Task WhenCallPostWithFormUrlEncodedIs200Responds_ShouldReturnsObjectDeserialized()
        {
            var httpSucessfullCodes = GenerateRespondeHelper.GetRangeCodes(HttpCodeGroupEnum.Successful);

            foreach (var httpCode in httpSucessfullCodes)
            {
                InitializeResponseOK(httpCode);

                var dictionary = new Dictionary<string, string> {
                    { "id", "username" }
                };

                var response = await _networkService.PostWithFormUrlEncoded<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, dictionary);

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
        public async Task WhenCallPostWithFormUrlEncodedIs300Responds_ShouldThrowExceptionWithResponse()
        {
            var httpRedirectCodes = GenerateRespondeHelper.GetRangeCodes(HttpCodeGroupEnum.Redirection);

            foreach (var httpCode in httpRedirectCodes)
            {
                InitializeResponseOK(httpCode);

                var dictionary = new Dictionary<string, string> {
                    { "id", "username" }
                };

                try
                {
                    var response = await _networkService.PostWithFormUrlEncoded<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, dictionary);
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
        public async Task WhenCallPostWithFormUrlEncodedIs400Responds_ShouldThrowExceptionWithError()
        {
            var httpClientErrorCodes = GenerateRespondeHelper.GetRangeCodes(HttpCodeGroupEnum.ClientError);

            foreach (var httpCode in httpClientErrorCodes)
            {
                InitializeResponseError(httpCode);

                var dictionary = new Dictionary<string, string> {
                    { "id", "username" }
                };

                try
                {
                    var response = await _networkService.PostWithFormUrlEncoded<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, dictionary);
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
        public async Task WhenCallPostWithFormUrlEncodedIs500Responds_ShouldThrowException()
        {
            var httpServerErrorCodes = GenerateRespondeHelper.GetRangeCodes(HttpCodeGroupEnum.ServerError);

            foreach (var httpCode in httpServerErrorCodes)
            {
                InitializeResponseError(httpCode);

                var dictionary = new Dictionary<string, string> {
                    { "id", "username" }
                };

                try
                {
                    var response = await _networkService.PostWithFormUrlEncoded<UserModel, ErrorTestModel>(TestConstanst.TEST_URL, dictionary);
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
