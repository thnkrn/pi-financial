using System.Collections;
using System.Globalization;
using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Helper;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Tests.Utils;

public class InstrumentHelperTest
{
    [Theory]
    [ClassData(typeof(InstrumentTestDeprecateData))]
    public void Should_ReturnExpected_When_IsDeprecated(Instrument instrument, bool expected)
    {
        // Act
        var actual = InstrumentHelper.IsDeprecated(instrument);

        // Assert
        Assert.Equal(expected, actual);
    }
        
    [Theory]
    [InlineData(InstrumentConstants.CS,  "X", true)]
    [InlineData(InstrumentConstants.CS,  "X,SP", true)]
    [InlineData(InstrumentConstants.CS,  "X,", true)]
    [InlineData(InstrumentConstants.CS,  "SP,CB", false)]
    [InlineData(InstrumentConstants.CS,  "", false)]
    [InlineData(InstrumentConstants.PS,  "X", true)]
    [InlineData(InstrumentConstants.ETF,  "X", true)]
    [InlineData(InstrumentConstants.DR,  "X", true)]
    [InlineData(InstrumentConstants.CS,  "S", false)]
    [InlineData(InstrumentConstants.PS,  "S", false)]
    [InlineData(InstrumentConstants.ETF,  "S", false)]
    [InlineData(InstrumentConstants.DR,  "S", false)]
    [InlineData(InstrumentConstants.FC,  "X", false)]
    [InlineData(InstrumentConstants.W,  "X", false)]
    [InlineData(InstrumentConstants.DWC,  "X", false)]
    [InlineData(InstrumentConstants.DWP,  "X", false)]
    [InlineData(InstrumentConstants.CS,  null, false)]
    public void Should_ReturnExpected_When_IsDeprecated_With_TradingSign(string securityType, string? sign, bool expected)
    {
        // Arrange
        var instrument = new Instrument
            { SecurityType = securityType, TradingSigns = [new TradingSign() {Sign = sign}] };
            
        // Act
        var actual = InstrumentHelper.IsDeprecated(instrument);

        // Assert
        Assert.Equal(expected, actual);
    }
    
    public class InstrumentTestDeprecateData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                var today = DateTime.UtcNow.Date;
                
                // TFEX
                foreach (var secType in new[]
                    {
                        InstrumentConstants.FC, InstrumentConstants.FP, InstrumentConstants.OEC,
                        InstrumentConstants.OEP, InstrumentConstants.CMB, InstrumentConstants.SPT,
                        InstrumentConstants.WEC, InstrumentConstants.WEP
                    })
                {
                    yield return [
                        new Instrument
                        {
                            SecurityType = secType,
                            LastTradingDate = today.Subtract(TimeSpan.FromDays(6))
                                .ToString(CultureInfo.InvariantCulture)
                        },
                        true
                    ];
                    yield return [
                        new Instrument
                        {
                            SecurityType = secType,
                            LastTradingDate = today.Subtract(TimeSpan.FromDays(2))
                                .ToString(CultureInfo.InvariantCulture)
                        },
                        false
                    ];
                }
                
                // Warrant
                foreach (var secType in new[] { InstrumentConstants.W })
                {
                    yield return [
                        new Instrument
                        {
                            SecurityType = secType,
                            ExerciseDate = today.Subtract(TimeSpan.FromDays(6))
                                .ToString(CultureInfo.InvariantCulture)
                        },
                        true
                    ];
                    yield return [
                        new Instrument
                        {
                            SecurityType = secType,
                            ExerciseDate = today.Subtract(TimeSpan.FromDays(2))
                                .ToString(CultureInfo.InvariantCulture)
                        },
                        false
                    ];
                }
                
                // DW
                foreach (var secType in new[] { InstrumentConstants.DWC, InstrumentConstants.DWP })
                {
                    yield return [
                        new Instrument
                        {
                            SecurityType = secType,
                            MaturityDate = today.Subtract(TimeSpan.FromDays(6))
                                .ToString(CultureInfo.InvariantCulture)
                        },
                        true
                    ];
                    yield return [
                        new Instrument
                        {
                            SecurityType = secType,
                            MaturityDate = today.Subtract(TimeSpan.FromDays(2))
                                .ToString(CultureInfo.InvariantCulture)
                        },
                        false
                    ];
                }
                
                
                // CheckDate is null
                foreach (var secType in new[]
                    {
                        InstrumentConstants.FC, InstrumentConstants.FP, InstrumentConstants.OEC,
                        InstrumentConstants.OEP, InstrumentConstants.CMB, InstrumentConstants.SPT,
                        InstrumentConstants.WEC, InstrumentConstants.WEP, InstrumentConstants.W,
                        InstrumentConstants.DWC, InstrumentConstants.DWP
                    })
                {
                    yield return [
                        new Instrument
                        {
                            SecurityType = secType,
                        },
                        false
                    ];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
}
