namespace Pi.SetMarketData.Application.Constants;

// ReSharper disable InconsistentNaming
public static class ItchMessageType
{
    public const char T = 'T'; // 7.3.1 Seconds Messages
    public const char R = 'R'; // 7.4.1 Order Book Directory
    public const char e = 'e'; // 7.4.2 Exchange Directory
    public const char m = 'm'; // 7.4.3 Market Directory
    public const char M = 'M'; // 7.4.4 Combination Order Book Leg Directory
    public const char L = 'L'; // 7.4.5 Tick Size Table Entry
    public const char k = 'k'; // 7.4.6 Price Limit Message
    public const char S = 'S'; // 7.5.1 System Event Message
    public const char O = 'O'; // 7.5.2 Order Book State Message
    public const char l = 'l'; // 7.5.3 Halt Information Message
    public const char b = 'b'; // 7.6.1 Market by Price (MBP) Incremental Message
    public const char Z = 'Z'; // 7.6.2 Equilibrium Price Message
    public const char i = 'i'; // 7.6.3 Trade Ticker Message
    public const char I = 'I'; // 7.6.4 Trade Statistics Message
    public const char f = 'f'; // 7.6.5 iNAV Message
    public const char J = 'J'; // 7.6.6 Index Price Message
    public const char g = 'g'; // 7.6.7 Market Statistic Message
    public const char Q = 'Q'; // 7.6.8 Reference Price Message
    public const char h = 'h'; // 7.6.9 Open Interest Message
    public const char N = 'N'; // 7.7.1 Market Announcement Message
    public const char j = 'j'; // 7.7.2 News Message
}