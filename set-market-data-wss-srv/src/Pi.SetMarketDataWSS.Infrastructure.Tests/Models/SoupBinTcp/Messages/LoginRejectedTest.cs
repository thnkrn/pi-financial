using Pi.SetMarketDataWSS.Infrastructure.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketDataWSS.Infrastructure.Tests.Models.SoupBinTcp.Messages
{
    public class LoginRejectedTests
    {
        private const char LoginRejectedType = 'J';
        private const char LoginRejectReasonNotAuthorised = 'A';
        private const char LoginRejectReasonSessionNotAvailable = 'S';
        
        [Theory]
        [InlineData(LoginRejectReasonNotAuthorised)]
        [InlineData(LoginRejectReasonSessionNotAvailable)]
        public void Constructor_SetsBytesProperty_Correctly(char rejectReasonCode)
        {
            var loginRejected = new LoginRejected(rejectReasonCode);

            Assert.Equal(LoginRejectedType, Convert.ToChar(loginRejected.Bytes[0]));
            Assert.Equal(rejectReasonCode, Convert.ToChar(loginRejected.Bytes[1]));
        }

        [Fact]
        public void RejectReasonCode_ReturnsCorrectValue()
        {
            var loginRejected = new LoginRejected(LoginRejectReasonNotAuthorised);

            var actualRejectReasonCode = loginRejected.RejectReasonCode;

            Assert.Equal(LoginRejectReasonNotAuthorised, actualRejectReasonCode);
        }

        [Theory]
        [InlineData('X')]
        [InlineData('Z')]
        public void Constructor_ThrowsArgumentException_ForInvalidRejectReasonCode(char rejectReasonCode)
        {
            var exception = Assert.Throws<ArgumentException>(() => new LoginRejected(rejectReasonCode));

            Assert.StartsWith("Reject reason code must be either A or S", exception.Message);
            Assert.Equal("rejectReasonCode", exception.ParamName);
        }
    }
}