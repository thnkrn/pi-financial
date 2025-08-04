using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;
using Xunit;

namespace Pi.GlobalEquities.Tests.DomainModels;

public class AccountTest
{
    static IAccount NewAccount(
        string id = null,
        string userId = null,
        string custCode = null,
        string TradingAccNo = null,
        string velexaAcc = null,
        DateTime dateTime = new())
    {
        return new Account
        {
            Id = id,
            UserId = userId,
            CustCode = custCode,
            TradingAccountNo = TradingAccNo,
            VelexaAccount = velexaAcc,
            UpdatedAt = dateTime
        };
    }

    public class GetProviderAccount_Test
    {
        [Fact]
        public void WhenProviderAccountIsNotVelexa_ThrowNotSupportedException()
        {
            var accounts = NewAccount();
            var notVelexaProv = (Provider)0;

            var action = () => accounts.GetProviderAccount(notVelexaProv);

            var exception = Assert.Throws<NotSupportedException>(action);
            Assert.Equal(notVelexaProv.ToString(), exception.Message);
        }

        [Fact]
        public void WhenProviderAccountIsVelexa_ReturnVelexaAccount()
        {
            var velexaAcc = "QLO7111.0079";
            var accounts = NewAccount(velexaAcc: velexaAcc);

            var velexaAccount = accounts.GetProviderAccount(Provider.Velexa);

            Assert.Equal(velexaAcc, velexaAccount);
        }
    }

    public class IsExpired_Test
    {
        [Fact]
        public void WhenAccountIsExpired_ReturnTrue()
        {
            var accounts = NewAccount(dateTime: new DateTime(2001, 01, 01));

            var isExpired = accounts.IsExpired();

            Assert.True(isExpired);
        }

        [Fact]
        public void WhenAccountIsNotExpired_ReturnFalse()
        {
            var accounts = NewAccount(dateTime: DateTime.Now);

            var isExpired = accounts.IsExpired();

            Assert.False(isExpired);
        }
    }
}
