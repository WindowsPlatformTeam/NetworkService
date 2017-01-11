using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NetworkService.Contracts.Models.Interfaces;
using NetworkService.Impl.Models;
using System;

namespace NetworkServiceTest
{
    [TestClass]
    public class ApiTokenTest
    {
        #region Test Const

        private const string TEST_TOKEN = "asdfddsswdwswsaaadsdffesadewaseHgdAdwSFs";
        private const string TEST_TOKEN_METADATA = "META:asdfddsswdwswsaaadsdffesadewaseHgdAdwSFs";

        #endregion

        #region Fields

        private ApiToken _apiToken;
        private Mock<IToken> _token;
        private Mock<ITokenMetadata> _tokenMetadata;

        #endregion

        #region Startup

        [TestInitialize]
        public void Startup()
        {
            _apiToken = new ApiToken();
        }

        #endregion

        [TestMethod]
        [TestCategory("NetworkService_ApiToken")]
        public void WhenApiTokenNoAddToken_ShouldGetTokenReturnNull()
        {
            Assert.AreEqual(_apiToken.GetToken(), null);
        }

        [TestMethod]
        [TestCategory("NetworkService_ApiToken")]
        public void WhenApiTokenConstructorWithToken_ShouldGetTokenReturnTheToken()
        {
            _token = new Mock<IToken>();
            _token.Setup(token => token.GetToken()).Returns(TEST_TOKEN);

            _apiToken = new ApiToken(_token.Object);
            Assert.AreEqual(_apiToken.GetToken(), TEST_TOKEN);
        }

        [TestMethod]
        [TestCategory("NetworkService_ApiToken")]
        public void WhenApiTokenAddToken_ShouldGetTokenReturnTheToken()
        {
            _token = new Mock<IToken>();
            _token.Setup(token => token.GetToken()).Returns(TEST_TOKEN);

            _apiToken.AddToken(_token.Object);
            Assert.AreEqual(_apiToken.GetToken(), TEST_TOKEN);
        }

        [TestMethod]
        [TestCategory("NetworkService_ApiToken")]
        public void WhenApiTokenAddTokenIToken_ShouldIsValidTokenReturnTrue()
        {
            _token = new Mock<IToken>();
            _token.Setup(token => token.GetToken()).Returns(TEST_TOKEN);

            _apiToken.AddToken(_token.Object);
            Assert.IsTrue(_apiToken.IsValidToken());
        }

        [TestMethod]
        [TestCategory("NetworkService_ApiToken")]
        public void WhenApiTokenAddTokenITokenNull_ShouldIsValidTokenReturnTrue()
        {
            _token = new Mock<IToken>();
            _token.Setup(token => token.GetToken()).Returns(default(string));

            _apiToken.AddToken(_token.Object);
            Assert.IsFalse(_apiToken.IsValidToken());
        }

        [TestMethod]
        [TestCategory("NetworkService_ApiToken")]
        public void WhenApiTokenAddTokenITokenMetadataNullWithExpiredDateBiggerThanToday_ShouldIsValidTokenReturnFalse()

        {
            _tokenMetadata = new Mock<ITokenMetadata>();
            _tokenMetadata.Setup(token => token.GetToken()).Returns(default(string));
            _tokenMetadata.Setup(token => token.GetTokenExpirationDate()).Returns(DateTime.Today.AddDays(1));

            _apiToken.AddToken(_tokenMetadata.Object);
            Assert.IsFalse(_apiToken.IsValidToken());
        }

        [TestMethod]
        [TestCategory("NetworkService_ApiToken")]
        public void WhenApiTokenAddTokenITokenMetadataWithExpiredDateBiggerThanToday_ShouldIsValidTokenReturnTrue()

        {
            _tokenMetadata = new Mock<ITokenMetadata>();
            _tokenMetadata.Setup(token => token.GetToken()).Returns(TEST_TOKEN_METADATA);
            _tokenMetadata.Setup(token => token.GetTokenExpirationDate()).Returns(DateTime.Today.AddDays(1));

            _apiToken.AddToken(_tokenMetadata.Object);
            Assert.IsTrue(_apiToken.IsValidToken());
        }

        [TestMethod]
        [TestCategory("NetworkService_ApiToken")]
        public void WhenApiTokenAddTokenITokenMetadataNullWithExpiredDateSmallerThanToday_ShouldIsValidTokenReturnFalse()

        {
            _tokenMetadata = new Mock<ITokenMetadata>();
            _tokenMetadata.Setup(token => token.GetToken()).Returns(default(string));
            _tokenMetadata.Setup(token => token.GetTokenExpirationDate()).Returns(DateTime.Today.AddDays(-1));

            _apiToken.AddToken(_tokenMetadata.Object);
            Assert.IsFalse(_apiToken.IsValidToken());
        }

        [TestMethod]
        [TestCategory("NetworkService_ApiToken")]
        public void WhenApiTokenAddTokenITokenMetadataWithExpiredDateSmallerThanToday_ShouldIsValidTokenReturnFalse()

        {
            _tokenMetadata = new Mock<ITokenMetadata>();
            _tokenMetadata.Setup(token => token.GetToken()).Returns(TEST_TOKEN_METADATA);
            _tokenMetadata.Setup(token => token.GetTokenExpirationDate()).Returns(DateTime.Today.AddDays(-1));

            _apiToken.AddToken(_tokenMetadata.Object);
            Assert.IsFalse(_apiToken.IsValidToken());
        }
    }
}
