using Microsoft.Extensions.Logging;
using Pi.Common.Http;
using Pi.GlobalEquities.Application.Exceptions;
using Pi.GlobalEquities.Application.Models.Dto;
using Pi.GlobalEquities.Application.Queries;
using Pi.GlobalEquities.Application.Repositories;
using Pi.GlobalEquities.Application.Services.Velexa;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Errors;
using Pi.GlobalEquities.Models;
using Pi.GlobalEquities.Requests;

namespace Pi.GlobalEquities.Application.Commands;

public class OrderCommands : IOrderCommands
{
    private readonly IAccountQueries _accountQueries;
    private readonly IVelexaService _velexaService;
    private readonly IOrderQueries _orderQueries;
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrderCommands> _logger;

    // Blacklist constants
    private static readonly BlacklistItem[] Blacklist = new[]
    {
        new BlacklistItem { Symbol = "AAPB", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "AAPU", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "AAPX", Venue = "BATS" },
        new BlacklistItem { Symbol = "AGQ", Venue = "ARCA" },
        new BlacklistItem { Symbol = "AIBD", Venue = "ARCA" },
        new BlacklistItem { Symbol = "AIBU", Venue = "ARCA" },
        new BlacklistItem { Symbol = "AMDL", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "AMZA", Venue = "ARCA" },
        new BlacklistItem { Symbol = "AMZD", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "AMZU", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "AMZZ", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "AVL", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "AVS", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "BABX", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "BDCX", Venue = "ARCA" },
        new BlacklistItem { Symbol = "BIB", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "BIS", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "BITI", Venue = "ARCA" },
        new BlacklistItem { Symbol = "BITU", Venue = "ARCA" },
        new BlacklistItem { Symbol = "BITX", Venue = "BATS" },
        new BlacklistItem { Symbol = "BOIL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "BRZU", Venue = "ARCA" },
        new BlacklistItem { Symbol = "BTCL", Venue = "BATS" },
        new BlacklistItem { Symbol = "BTCZ", Venue = "BATS" },
        new BlacklistItem { Symbol = "BTFX", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "BULZ", Venue = "ARCA" },
        new BlacklistItem { Symbol = "BZQ", Venue = "ARCA" },
        new BlacklistItem { Symbol = "CARU", Venue = "ARCA" },
        new BlacklistItem { Symbol = "CEFD", Venue = "ARCA" },
        new BlacklistItem { Symbol = "CHAU", Venue = "ARCA" },
        new BlacklistItem { Symbol = "CLDL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "CONL", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "CURE", Venue = "ARCA" },
        new BlacklistItem { Symbol = "CWEB", Venue = "ARCA" },
        new BlacklistItem { Symbol = "DDM", Venue = "ARCA" },
        new BlacklistItem { Symbol = "DFEN", Venue = "ARCA" },
        new BlacklistItem { Symbol = "DGP", Venue = "ARCA" },
        new BlacklistItem { Symbol = "DIG", Venue = "ARCA" },
        new BlacklistItem { Symbol = "DPST", Venue = "ARCA" },
        new BlacklistItem { Symbol = "DRIP", Venue = "ARCA" },
        new BlacklistItem { Symbol = "DRN", Venue = "ARCA" },
        new BlacklistItem { Symbol = "DRV", Venue = "ARCA" },
        new BlacklistItem { Symbol = "DUG", Venue = "ARCA" },
        new BlacklistItem { Symbol = "DUSL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "DUST", Venue = "ARCA" },
        new BlacklistItem { Symbol = "DXD", Venue = "ARCA" },
        new BlacklistItem { Symbol = "EDC", Venue = "ARCA" },
        new BlacklistItem { Symbol = "EDZ", Venue = "ARCA" },
        new BlacklistItem { Symbol = "EET", Venue = "ARCA" },
        new BlacklistItem { Symbol = "EEV", Venue = "ARCA" },
        new BlacklistItem { Symbol = "EFO", Venue = "ARCA" },
        new BlacklistItem { Symbol = "EFU", Venue = "ARCA" },
        new BlacklistItem { Symbol = "EMTY", Venue = "ARCA" },
        new BlacklistItem { Symbol = "EPV", Venue = "ARCA" },
        new BlacklistItem { Symbol = "ERX", Venue = "ARCA" },
        new BlacklistItem { Symbol = "ERY", Venue = "ARCA" },
        new BlacklistItem { Symbol = "ESUS", Venue = "ARCA" },
        new BlacklistItem { Symbol = "ETHT", Venue = "ARCA" },
        new BlacklistItem { Symbol = "ETHU", Venue = "BATS" },
        new BlacklistItem { Symbol = "EUM", Venue = "ARCA" },
        new BlacklistItem { Symbol = "EUO", Venue = "ARCA" },
        new BlacklistItem { Symbol = "EURL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "EVAV", Venue = "ARCA" },
        new BlacklistItem { Symbol = "EWV", Venue = "ARCA" },
        new BlacklistItem { Symbol = "EZJ", Venue = "ARCA" },
        new BlacklistItem { Symbol = "FAS", Venue = "ARCA" },
        new BlacklistItem { Symbol = "FAZ", Venue = "ARCA" },
        new BlacklistItem { Symbol = "FBL", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "FEDL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "FLYU", Venue = "ARCA" },
        new BlacklistItem { Symbol = "FNGD", Venue = "ARCA" },
        new BlacklistItem { Symbol = "FNGG", Venue = "ARCA" },
        new BlacklistItem { Symbol = "FNGO", Venue = "ARCA" },
        new BlacklistItem { Symbol = "FNGU", Venue = "ARCA" },
        new BlacklistItem { Symbol = "FXP", Venue = "ARCA" },
        new BlacklistItem { Symbol = "GDE", Venue = "BATS" },
        new BlacklistItem { Symbol = "GDXU", Venue = "ARCA" },
        new BlacklistItem { Symbol = "GGLL", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "GGLS", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "GLL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "GOOX", Venue = "BATS" },
        new BlacklistItem { Symbol = "GUSH", Venue = "ARCA" },
        new BlacklistItem { Symbol = "HCMT", Venue = "ARCA" },
        new BlacklistItem { Symbol = "HDLB", Venue = "ARCA" },
        new BlacklistItem { Symbol = "HIBL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "HIBS", Venue = "ARCA" },
        new BlacklistItem { Symbol = "INDL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "IWDL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "IWFL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "JDST", Venue = "ARCA" },
        new BlacklistItem { Symbol = "JETU", Venue = "ARCA" },
        new BlacklistItem { Symbol = "KOLD", Venue = "ARCA" },
        new BlacklistItem { Symbol = "KORU", Venue = "ARCA" },
        new BlacklistItem { Symbol = "LABD", Venue = "ARCA" },
        new BlacklistItem { Symbol = "LABU", Venue = "ARCA" },
        new BlacklistItem { Symbol = "LLYX", Venue = "ARCA" },
        new BlacklistItem { Symbol = "LMBO", Venue = "ARCA" },
        new BlacklistItem { Symbol = "LTL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "MAGX", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "METU", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "MEXX", Venue = "ARCA" },
        new BlacklistItem { Symbol = "MIDU", Venue = "ARCA" },
        new BlacklistItem { Symbol = "MLPR", Venue = "ARCA" },
        new BlacklistItem { Symbol = "MSFL", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "MSFU", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "MSFX", Venue = "BATS" },
        new BlacklistItem { Symbol = "MSOX", Venue = "ARCA" },
        new BlacklistItem { Symbol = "MSTX", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "MTUL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "MUD", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "MUU", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "MVPL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "MVRL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "MVV", Venue = "ARCA" },
        new BlacklistItem { Symbol = "MYY", Venue = "ARCA" },
        new BlacklistItem { Symbol = "MZZ", Venue = "ARCA" },
        new BlacklistItem { Symbol = "NAIL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "NFXL", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "NUGT", Venue = "ARCA" },
        new BlacklistItem { Symbol = "NVD", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "NVDD", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "NVDL", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "NVDU", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "NVDX", Venue = "BATS" },
        new BlacklistItem { Symbol = "OILU", Venue = "ARCA" },
        new BlacklistItem { Symbol = "OOTO", Venue = "ARCA" },
        new BlacklistItem { Symbol = "PFFA", Venue = "ARCA" },
        new BlacklistItem { Symbol = "PFFL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "PILL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "PLTU", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "QID", Venue = "ARCA" },
        new BlacklistItem { Symbol = "QLD", Venue = "ARCA" },
        new BlacklistItem { Symbol = "QQQD", Venue = "ARCA" },
        new BlacklistItem { Symbol = "QQQU", Venue = "ARCA" },
        new BlacklistItem { Symbol = "QTAP", Venue = "BATS" },
        new BlacklistItem { Symbol = "QTJA", Venue = "BATS" },
        new BlacklistItem { Symbol = "QTJL", Venue = "BATS" },
        new BlacklistItem { Symbol = "QTOC", Venue = "BATS" },
        new BlacklistItem { Symbol = "QULL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "REK", Venue = "ARCA" },
        new BlacklistItem { Symbol = "REKT", Venue = "ARCA" },
        new BlacklistItem { Symbol = "RETL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "REW", Venue = "ARCA" },
        new BlacklistItem { Symbol = "ROM", Venue = "ARCA" },
        new BlacklistItem { Symbol = "RSEE", Venue = "BATS" },
        new BlacklistItem { Symbol = "RXD", Venue = "ARCA" },
        new BlacklistItem { Symbol = "RXL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SAA", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SBB", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SCC", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SCDL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SCO", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SDS", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SETH", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SHNY", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SIJ", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SKF", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SKYU", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "SMDD", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SMHB", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SMN", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SOXL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SOXS", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SPUU", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SPXL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SPXS", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SPXU", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SPYU", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SQQQ", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "SRS", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SRTY", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SSG", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SZK", Venue = "ARCA" },
        new BlacklistItem { Symbol = "TARK", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "TBF", Venue = "ARCA" },
        new BlacklistItem { Symbol = "TBT", Venue = "ARCA" },
        new BlacklistItem { Symbol = "TBX", Venue = "ARCA" },
        new BlacklistItem { Symbol = "TECL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "TECS", Venue = "ARCA" },
        new BlacklistItem { Symbol = "TMF", Venue = "ARCA" },
        new BlacklistItem { Symbol = "TMV", Venue = "ARCA" },
        new BlacklistItem { Symbol = "TNA", Venue = "ARCA" },
        new BlacklistItem { Symbol = "TPOR", Venue = "ARCA" },
        new BlacklistItem { Symbol = "TQQQ", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "TSL", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "TSLL", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "TSLR", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "TSLS", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "TSLT", Venue = "BATS" },
        new BlacklistItem { Symbol = "TSMX", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "TSMZ", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "TTT", Venue = "ARCA" },
        new BlacklistItem { Symbol = "TWM", Venue = "ARCA" },
        new BlacklistItem { Symbol = "TYD", Venue = "ARCA" },
        new BlacklistItem { Symbol = "TYO", Venue = "ARCA" },
        new BlacklistItem { Symbol = "TZA", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UBOT", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UBR", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UBT", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UCC", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UCO", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UCYB", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "UDOW", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UGE", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UGL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UJB", Venue = "ARCA" },
        new BlacklistItem { Symbol = "ULE", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UMDD", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UPRO", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UPV", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UPW", Venue = "ARCA" },
        new BlacklistItem { Symbol = "URAA", Venue = "ARCA" },
        new BlacklistItem { Symbol = "URAX", Venue = "ARCA" },
        new BlacklistItem { Symbol = "URE", Venue = "ARCA" },
        new BlacklistItem { Symbol = "URTY", Venue = "ARCA" },
        new BlacklistItem { Symbol = "USD", Venue = "ARCA" },
        new BlacklistItem { Symbol = "USML", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UST", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UVIX", Venue = "BATS" },
        new BlacklistItem { Symbol = "UVXY", Venue = "BATS" },
        new BlacklistItem { Symbol = "UWM", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UXI", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UYG", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UYM", Venue = "ARCA" },
        new BlacklistItem { Symbol = "WEBL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "XAPR", Venue = "BATS" },
        new BlacklistItem { Symbol = "XAUG", Venue = "BATS" },
        new BlacklistItem { Symbol = "XBAP", Venue = "BATS" },
        new BlacklistItem { Symbol = "XBJA", Venue = "BATS" },
        new BlacklistItem { Symbol = "XBJL", Venue = "BATS" },
        new BlacklistItem { Symbol = "XBOC", Venue = "BATS" },
        new BlacklistItem { Symbol = "XDAP", Venue = "BATS" },
        new BlacklistItem { Symbol = "XDEC", Venue = "BATS" },
        new BlacklistItem { Symbol = "XDJA", Venue = "BATS" },
        new BlacklistItem { Symbol = "XDJL", Venue = "BATS" },
        new BlacklistItem { Symbol = "XDOC", Venue = "BATS" },
        new BlacklistItem { Symbol = "XDQQ", Venue = "BATS" },
        new BlacklistItem { Symbol = "XDSQ", Venue = "BATS" },
        new BlacklistItem { Symbol = "XFEB", Venue = "BATS" },
        new BlacklistItem { Symbol = "XJAN", Venue = "BATS" },
        new BlacklistItem { Symbol = "XJUL", Venue = "BATS" },
        new BlacklistItem { Symbol = "XJUN", Venue = "BATS" },
        new BlacklistItem { Symbol = "XMAR", Venue = "BATS" },
        new BlacklistItem { Symbol = "XMAY", Venue = "BATS" },
        new BlacklistItem { Symbol = "XNOV", Venue = "BATS" },
        new BlacklistItem { Symbol = "XOCT", Venue = "BATS" },
        new BlacklistItem { Symbol = "XSEP", Venue = "BATS" },
        new BlacklistItem { Symbol = "XTAP", Venue = "BATS" },
        new BlacklistItem { Symbol = "XTJA", Venue = "BATS" },
        new BlacklistItem { Symbol = "XTJL", Venue = "BATS" },
        new BlacklistItem { Symbol = "XTOC", Venue = "BATS" },
        new BlacklistItem { Symbol = "XXCH", Venue = "ARCA" },
        new BlacklistItem { Symbol = "YANG", Venue = "ARCA" },
        new BlacklistItem { Symbol = "YCL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "YCS", Venue = "ARCA" },
        new BlacklistItem { Symbol = "ZSL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "7200", Venue = "HKEX" },
        new BlacklistItem { Symbol = "7226", Venue = "HKEX" },
        new BlacklistItem { Symbol = "7232", Venue = "HKEX" },
        new BlacklistItem { Symbol = "7233", Venue = "HKEX" },
        new BlacklistItem { Symbol = "7234", Venue = "HKEX" },
        new BlacklistItem { Symbol = "7261", Venue = "HKEX" },
        new BlacklistItem { Symbol = "7266", Venue = "HKEX" },
        new BlacklistItem { Symbol = "7288", Venue = "HKEX" },
        new BlacklistItem { Symbol = "7299", Venue = "HKEX" },
        new BlacklistItem { Symbol = "7500", Venue = "HKEX" },
        new BlacklistItem { Symbol = "7522", Venue = "HKEX" },
        new BlacklistItem { Symbol = "7552", Venue = "HKEX" },
        new BlacklistItem { Symbol = "7568", Venue = "HKEX" },
        new BlacklistItem { Symbol = "7588", Venue = "HKEX" },
        new BlacklistItem { Symbol = "AETH", Venue = "ARCA" },
        new BlacklistItem { Symbol = "ARKA", Venue = "BATS" },
        new BlacklistItem { Symbol = "ARKB", Venue = "BATS" },
        new BlacklistItem { Symbol = "ARKC", Venue = "BATS" },
        new BlacklistItem { Symbol = "ARKD", Venue = "BATS" },
        new BlacklistItem { Symbol = "ARKW", Venue = "ARCA" },
        new BlacklistItem { Symbol = "ARKY", Venue = "BATS" },
        new BlacklistItem { Symbol = "ARKZ", Venue = "BATS" },
        new BlacklistItem { Symbol = "BETE", Venue = "ARCA" },
        new BlacklistItem { Symbol = "BETH", Venue = "ARCA" },
        new BlacklistItem { Symbol = "BITB", Venue = "ARCA" },
        new BlacklistItem { Symbol = "BITC", Venue = "ARCA" },
        new BlacklistItem { Symbol = "BITI", Venue = "ARCA" },
        new BlacklistItem { Symbol = "BITO", Venue = "ARCA" },
        new BlacklistItem { Symbol = "BITQ", Venue = "ARCA" },
        new BlacklistItem { Symbol = "BITS", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "BITU", Venue = "ARCA" },
        new BlacklistItem { Symbol = "BITX", Venue = "BATS" },
        new BlacklistItem { Symbol = "BKCH", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "BLCN", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "BLKC", Venue = "BATS" },
        new BlacklistItem { Symbol = "BLOK", Venue = "ARCA" },
        new BlacklistItem { Symbol = "BRRR", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "BTC", Venue = "ARCA" },
        new BlacklistItem { Symbol = "BTCL", Venue = "BATS" },
        new BlacklistItem { Symbol = "BTCO", Venue = "BATS" },
        new BlacklistItem { Symbol = "BTCW", Venue = "BATS" },
        new BlacklistItem { Symbol = "BTCZ", Venue = "BATS" },
        new BlacklistItem { Symbol = "BTF", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "BTFX", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "BTGD", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "BTOP", Venue = "ARCA" },
        new BlacklistItem { Symbol = "CETH", Venue = "BATS" },
        new BlacklistItem { Symbol = "CONI", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "DAPP", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "DECO", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "DEFI", Venue = "ARCA" },
        new BlacklistItem { Symbol = "EETH", Venue = "ARCA" },
        new BlacklistItem { Symbol = "EFUT", Venue = "BATS" },
        new BlacklistItem { Symbol = "ETH", Venue = "ARCA" },
        new BlacklistItem { Symbol = "ETHA", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "ETHD", Venue = "ARCA" },
        new BlacklistItem { Symbol = "ETHE", Venue = "ARCA" },
        new BlacklistItem { Symbol = "ETHT", Venue = "ARCA" },
        new BlacklistItem { Symbol = "ETHU", Venue = "BATS" },
        new BlacklistItem { Symbol = "ETHV", Venue = "BATS" },
        new BlacklistItem { Symbol = "ETHW", Venue = "ARCA" },
        new BlacklistItem { Symbol = "ETQ", Venue = "BATS" },
        new BlacklistItem { Symbol = "ETU", Venue = "BATS" },
        new BlacklistItem { Symbol = "EZBC", Venue = "BATS" },
        new BlacklistItem { Symbol = "EZET", Venue = "BATS" },
        new BlacklistItem { Symbol = "FBTC", Venue = "BATS" },
        new BlacklistItem { Symbol = "FDIG", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "FETH", Venue = "BATS" },
        new BlacklistItem { Symbol = "FIAT", Venue = "ARCA" },
        new BlacklistItem { Symbol = "GBTC", Venue = "ARCA" },
        new BlacklistItem { Symbol = "HECO", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "HODL", Venue = "BATS" },
        new BlacklistItem { Symbol = "IBIT", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "IBLC", Venue = "ARCA" },
        new BlacklistItem { Symbol = "LEGR", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "LMBO", Venue = "ARCA" },
        new BlacklistItem { Symbol = "MAXI", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "MSTX", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "QETH", Venue = "BATS" },
        new BlacklistItem { Symbol = "REKT", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SATO", Venue = "BATS" },
        new BlacklistItem { Symbol = "SETH", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SMST", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "SPBC", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "STCE", Venue = "ARCA" },
        new BlacklistItem { Symbol = "WGMI", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "YBIT", Venue = "ARCA" },
        new BlacklistItem { Symbol = "YBTC", Venue = "BATS" },
        new BlacklistItem { Symbol = "ZZZ", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "AGQ", Venue = "ARCA" },
        new BlacklistItem { Symbol = "ARLP", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "BBU", Venue = "NYSE" },
        new BlacklistItem { Symbol = "BDRY", Venue = "ARCA" },
        new BlacklistItem { Symbol = "BEP", Venue = "NYSE" },
        new BlacklistItem { Symbol = "BIP", Venue = "NYSE" },
        new BlacklistItem { Symbol = "BNO", Venue = "ARCA" },
        new BlacklistItem { Symbol = "BOIL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "BPYPM", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "BPYPN", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "BPYPO", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "BPYPP", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "BSM", Venue = "NYSE" },
        new BlacklistItem { Symbol = "CANE", Venue = "ARCA" },
        new BlacklistItem { Symbol = "CAPL", Venue = "NYSE" },
        new BlacklistItem { Symbol = "CODI", Venue = "NYSE" },
        new BlacklistItem { Symbol = "CORN", Venue = "ARCA" },
        new BlacklistItem { Symbol = "CPER", Venue = "ARCA" },
        new BlacklistItem { Symbol = "CQP", Venue = "NYSE" },
        new BlacklistItem { Symbol = "CRT", Venue = "NYSE" },
        new BlacklistItem { Symbol = "DBA", Venue = "ARCA" },
        new BlacklistItem { Symbol = "DBB", Venue = "ARCA" },
        new BlacklistItem { Symbol = "DBC", Venue = "ARCA" },
        new BlacklistItem { Symbol = "DBO", Venue = "ARCA" },
        new BlacklistItem { Symbol = "DBP", Venue = "ARCA" },
        new BlacklistItem { Symbol = "DEFI", Venue = "ARCA" },
        new BlacklistItem { Symbol = "DKL", Venue = "NYSE" },
        new BlacklistItem { Symbol = "DLNG", Venue = "NYSE" },
        new BlacklistItem { Symbol = "DMLP", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "EPD", Venue = "NYSE" },
        new BlacklistItem { Symbol = "ET", Venue = "NYSE" },
        new BlacklistItem { Symbol = "EUO", Venue = "ARCA" },
        new BlacklistItem { Symbol = "FPH", Venue = "NYSE" },
        new BlacklistItem { Symbol = "FTAI", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "GEL", Venue = "NYSE" },
        new BlacklistItem { Symbol = "GLL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "GLP", Venue = "NYSE" },
        new BlacklistItem { Symbol = "GSG", Venue = "ARCA" },
        new BlacklistItem { Symbol = "HESM", Venue = "NYSE" },
        new BlacklistItem { Symbol = "IEP", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "KNOP", Venue = "NYSE" },
        new BlacklistItem { Symbol = "KRP", Venue = "NYSE" },
        new BlacklistItem { Symbol = "LAZ", Venue = "NYSE" },
        new BlacklistItem { Symbol = "MPLX", Venue = "NYSE" },
        new BlacklistItem { Symbol = "NFE", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "NGL", Venue = "NYSE" },
        new BlacklistItem { Symbol = "NMM", Venue = "NYSE" },
        new BlacklistItem { Symbol = "NYXH", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "PAA", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "PAGP", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "PBT", Venue = "NYSE" },
        new BlacklistItem { Symbol = "SBR", Venue = "NYSE" },
        new BlacklistItem { Symbol = "SCO", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SGU", Venue = "NYSE" },
        new BlacklistItem { Symbol = "SOYB", Venue = "ARCA" },
        new BlacklistItem { Symbol = "SPH", Venue = "NYSE" },
        new BlacklistItem { Symbol = "SUN", Venue = "NYSE" },
        new BlacklistItem { Symbol = "SVIX", Venue = "BATS" },
        new BlacklistItem { Symbol = "SVXY", Venue = "BATS" },
        new BlacklistItem { Symbol = "UAN", Venue = "NYSE" },
        new BlacklistItem { Symbol = "UCO", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UGL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "ULE", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UNG", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UNL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "USAC", Venue = "NYSE" },
        new BlacklistItem { Symbol = "USCI", Venue = "ARCA" },
        new BlacklistItem { Symbol = "USL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "USO", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UUP", Venue = "ARCA" },
        new BlacklistItem { Symbol = "UVIX", Venue = "BATS" },
        new BlacklistItem { Symbol = "UVXY", Venue = "BATS" },
        new BlacklistItem { Symbol = "VIXM", Venue = "BATS" },
        new BlacklistItem { Symbol = "VIXY", Venue = "BATS" },
        new BlacklistItem { Symbol = "VNOM", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "WAVE", Venue = "NASDAQ" },
        new BlacklistItem { Symbol = "WEIX", Venue = "BATS" },
        new BlacklistItem { Symbol = "WES", Venue = "NYSE" },
        new BlacklistItem { Symbol = "WLKP", Venue = "NYSE" },
        new BlacklistItem { Symbol = "YCL", Venue = "ARCA" },
        new BlacklistItem { Symbol = "ZSL", Venue = "ARCA" }
    };

    public OrderCommands(
        IAccountQueries accountQueries,
        IVelexaService velexaService,
        IOrderQueries orderQueries,
        IOrderRepository orderRepository,
        ILogger<OrderCommands> logger)
    {
        _accountQueries = accountQueries;
        _velexaService = velexaService;
        _orderQueries = orderQueries;
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<OrderDto> PlaceOrder(string userId, IOrder request, CancellationToken ct = default)
    {
        ValidateBlacklist(request.Symbol, request.Venue);

        var accountId = request.AccountId;
        var currency = Order.CurrencyFromVenue(request.Venue);

        IAccount? account;

        if (request.Side == OrderSide.Buy && request.OrderType != OrderType.Market)
            account = await _accountQueries.GetAccountBalanceByAccountId(userId, accountId, currency, ct);
        else
            account = await _accountQueries.GetAccountByAccountId(userId, accountId, ct);

        if (account == null)
            throw new GeException(AccountErrors.NotExist);

        ValidateOrderPermission(account, request.Side);

        var providerAccount = account.GetProviderAccount(Provider.Velexa);
        request.SetOwner(account);

        if (request.Side == OrderSide.Buy && request.OrderType != OrderType.Market)
        {
            var accountBalance = (IAccountBalance)account;
            var balance = accountBalance.GetBalance(Provider.Velexa, currency);
            if (!request.CanBuy(balance))
                throw new GeException(AccountErrors.InsufficientBalance);
        }
        else if (request.Side == OrderSide.Sell)
        {
            var position = await _orderQueries.GetPosition(providerAccount, request.SymbolId, ct);
            if (position == null || !position.CanSell(request.Quantity))
                throw new GeException(PositionErrors.InsufficientHoldings);
        }

        if (request.OrderType == OrderType.Tpsl)
            return await PlaceTpSlOrder(userId, request, providerAccount, ct);

        var order = await PlaceOrder(request, providerAccount, ct);
        return new OrderDto(order);
    }

    public async Task<OrderDto> ModifyOrder(string userId, string refId, IOrderValues request,
        CancellationToken ct = default)
    {
        //NOTE: Get order and filter out inactive order
        //NOTE: If there’s a partially filled (not cancelled) TP/SL, another active order is considered as normal stop or limit order
        var orders = (await GetOrders(refId, ct)).Where(x => !x.IsFinalStatus()).ToList();

        if (orders.Any(x => x.OrderType == OrderType.TakeProfit) && orders.Any(x => x.OrderType == OrderType.StopLoss))
            return await ModifyTpSlOrder(userId, orders, request, ct);

        var order = orders.FirstOrDefault();
        if (order == null)
            throw new GeException(OrderErrors.OrderNotFound);

        var providerAccount = order.ProviderInfo.AccountId;
        IAccount? account;

        if (order.Side == OrderSide.Buy && order.OrderType != OrderType.Market)
            account = await _accountQueries.GetAccountBalanceByProviderAccount(userId, Provider.Velexa, providerAccount, order.Currency, ct);
        else
            account = await _accountQueries.GetAccountByProviderAccount(userId, Provider.Velexa, providerAccount, ct);

        if (account == null)
            throw new GeException(AccountErrors.NotExist);

        ValidateOrderPermission(account, order.Side);
        ValidateOrderOwner(order, account);
        order.SetOwner(account);

        switch (order.Side)
        {
            case OrderSide.Buy when order.OrderType != OrderType.Market:
                ValidateBuyOrderModification((IAccountBalance?)account, order, request);
                break;
            case OrderSide.Sell:
                await ValidateSellOrderModification(order, request, ct);
                break;
        }

        var result = await ModifyOrder(order, request, ct);
        return new OrderDto(result);
    }

    public async Task<OrderDto> CancelOrder(string userId, string refId, CancellationToken ct = default)
    {
        var orders = (await GetOrders(refId, ct)).Where(x => !x.IsFinalStatus()).ToList();

        if (orders.Any(x => x.OrderType == OrderType.TakeProfit) && orders.Any(x => x.OrderType == OrderType.StopLoss))
            return await CancelTpSlOrder(userId, orders, ct);

        var order = orders.FirstOrDefault();
        if (order == null)
            throw new GeException(OrderErrors.OrderNotFound);

        var account = await _accountQueries.GetAccountByProviderAccount(userId, Provider.Velexa, order.ProviderInfo.AccountId, ct);
        if (account == null)
            throw new GeException(AccountErrors.NotExist);

        ValidateOrderOwner(order, account);

        orders.ForEach(x => x.SetOwner(account));

        var result = await CancelOrder(order, ct);
        return new OrderDto(result);
    }

    private async Task<OrderDto> PlaceTpSlOrder(string userId, IOrder orderRequest, string providerAccount, CancellationToken ct)
    {
        IOrder tpOrder, slOrder;
        var groupId = Guid.NewGuid();

        tpOrder = new Order
        {
            Id = null,
            GroupId = groupId.ToString(),
            UserId = userId,
            AccountId = orderRequest.AccountId,
            Venue = orderRequest.Venue,
            Symbol = orderRequest.Symbol,
            OrderType = OrderType.TakeProfit,
            Duration = orderRequest.Duration,
            Side = orderRequest.Side,
            Quantity = orderRequest.Quantity,
            LimitPrice = orderRequest.LimitPrice,
            Channel = orderRequest.Channel,
            CreatedAt = orderRequest.CreatedAt
        };
        tpOrder = await PlaceOrder(tpOrder, providerAccount, ct);

        try
        {
            slOrder = new Order
            {
                Id = null,
                GroupId = groupId.ToString(),
                UserId = userId,
                AccountId = orderRequest.AccountId,
                Venue = orderRequest.Venue,
                Symbol = orderRequest.Symbol,
                OrderType = OrderType.StopLoss,
                Duration = orderRequest.Duration,
                Side = orderRequest.Side,
                Quantity = orderRequest.Quantity,
                StopPrice = orderRequest.StopPrice,
                Channel = orderRequest.Channel,
                CreatedAt = orderRequest.CreatedAt
            };
            slOrder = await PlaceOrder(slOrder, providerAccount, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cannot place stop order. TpOrderId: {TpOrderId}", tpOrder.Id);

            await CancelOrder(tpOrder, ct);
            throw;
        }

        return await CancelIfOrdersNotExist(tpOrder, slOrder, ct);
    }

    private async Task<OrderDto> ModifyTpSlOrder(string userId, IEnumerable<IOrder> orders, IOrderValues request, CancellationToken ct)
    {
        var groupedOrders = orders.ToList();
        //NOTE: Check if the orders are owned by the same account
        if (groupedOrders[0].ProviderInfo.AccountId != groupedOrders[1].ProviderInfo.AccountId)
            throw new GeException(OrderErrors.UserAccessDenied);

        var account = await _accountQueries.GetAccountByProviderAccount(userId, Provider.Velexa, groupedOrders[0].ProviderInfo.AccountId, ct);
        if (account == null)
            throw new GeException(AccountErrors.NotExist);

        ValidateOrderPermission(account, OrderSide.Sell);
        ValidateOrdersOwner(groupedOrders, account);

        groupedOrders.ForEach(x => x.SetOwner(account));

        var tpOrder = groupedOrders.FirstOrDefault(x => x.OrderType == OrderType.TakeProfit)!;
        var slOrder = groupedOrders.FirstOrDefault(x => x.OrderType == OrderType.StopLoss)!;

        var fallbackTpPrice = tpOrder.LimitPrice;
        var fallbackQuantity = tpOrder.Quantity;

        var tpReq = new UpdateOrderRequest { Quantity = request.Quantity, LimitPrice = request.LimitPrice };
        var updatedTpOrder = await ModifyOrder(tpOrder, tpReq, ct);

        try
        {
            var slReq = new UpdateOrderRequest { Quantity = request.Quantity, StopPrice = request.StopPrice };
            var updatedSlOrder = await ModifyOrder(slOrder, slReq, ct);
            return new OrderDto(updatedTpOrder, updatedSlOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cannot modify stop loss order tpOrderId: {TpOrderId}, slOrderId: {SlOrderId}", tpOrder.Id, slOrder.Id);

            var fallbackReq = new UpdateOrderRequest { Quantity = fallbackQuantity, LimitPrice = fallbackTpPrice!.Value };
            await ModifyOrder(updatedTpOrder, fallbackReq, ct);

            throw;
        }
    }

    private async Task<OrderDto> CancelTpSlOrder(string userId, IEnumerable<IOrder> orders, CancellationToken ct)
    {
        var groupedOrders = orders.ToList();
        //NOTE: Check if the orders are owned by the same account
        if (groupedOrders[0].ProviderInfo.AccountId != groupedOrders[1].ProviderInfo.AccountId)
            throw new GeException(OrderErrors.UserAccessDenied);

        var account = await _accountQueries.GetAccountByProviderAccount(userId, Provider.Velexa, groupedOrders.FirstOrDefault()!.ProviderInfo.AccountId, ct);
        if (account == null)
            throw new GeException(AccountErrors.NotExist);

        ValidateOrdersOwner(groupedOrders, account);

        groupedOrders.ForEach(x => x.SetOwner(account));

        var tpOrder = groupedOrders.FirstOrDefault(x => x.OrderType == OrderType.TakeProfit)!;
        var slOrder = groupedOrders.FirstOrDefault(x => x.OrderType == OrderType.StopLoss)!;

        var tTpCancel = CancelOrder(tpOrder, ct);
        var tSlCancel = CancelOrder(slOrder, ct);

        try
        {
            await Task.WhenAll(tTpCancel, tSlCancel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Fail to cancel one or both TP/SL orders. GroupId: {GroupId},  tpOrderId: {TpOrderId}, slOrderId: {SlOrderId}",
                tpOrder.GroupId, tpOrder.Id, slOrder.Id);
            throw;
        }

        var updatedTpOrder = await tTpCancel;
        var updatedSlOrder = await tSlCancel;

        return new OrderDto(updatedTpOrder, updatedSlOrder);
    }

    private async Task<IOrder> PlaceOrder(IOrder order, string providerAccount, CancellationToken ct)
    {
        var result = await _velexaService.PlaceOrder(order, providerAccount, ct);
        order.Update(result, out _);
        UpsertOrder(order, ct);

        return order;
    }

    private async Task<IOrder> ModifyOrder(IOrder order, IOrderValues request, CancellationToken ct)
    {
        IOrderUpdates result = await HandlingProviderProcess(() => _velexaService.UpdateOrder(order.Id, request, ct));

        order.Update(result, out _);
        UpsertOrder(order, ct);

        return order;
    }

    private async Task<IOrder> CancelOrder(IOrder order, CancellationToken ct)
    {
        IOrderStatus? result = await HandlingProviderProcess(() => _velexaService.CancelOrder(order.Id, ct));

        order.Update(result, out _);
        UpsertOrder(order, ct);

        return order;
    }

    private async Task<OrderDto> CancelIfOrdersNotExist(IOrder tpOrder, IOrder slOrder, CancellationToken ct)
    {
        try
        {
            var tLatestTpOrder = _velexaService.GetOrder(tpOrder.Id, ct);
            var tLatestSlOrder = _velexaService.GetOrder(slOrder.Id, ct);
            await Task.WhenAll(tLatestTpOrder, tLatestSlOrder);

            var latestTpOrder = await tLatestTpOrder;
            var latestSlOrder = await tLatestSlOrder;

            //NOTE: [1 Matched or Cancelled] with [1 Working]
            if (latestTpOrder.IsFinalStatus() && !latestSlOrder.IsFinalStatus())
            {
                latestSlOrder = await CancelOrder(latestSlOrder, ct);

                return latestTpOrder.Status is OrderStatus.Matched or OrderStatus.PartiallyMatched
                    ? new OrderDto(latestTpOrder)
                    : new OrderDto(latestTpOrder, latestSlOrder);
            }
            if (latestSlOrder.IsFinalStatus() && !latestTpOrder.IsFinalStatus())
            {
                latestTpOrder = await CancelOrder(latestTpOrder, ct);

                return latestSlOrder.Status is OrderStatus.Matched or OrderStatus.PartiallyMatched
                    ? new OrderDto(latestSlOrder)
                    : new OrderDto(latestTpOrder, latestSlOrder);
            }

            //NOTE: (Both are Working) or (Both are rejected or cancelled)
            return new OrderDto(latestTpOrder, latestSlOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Fail to Cancel Grouped Order, GroupId: {GroupId}, TpOrderId: {TpOrderId}, SlOrderId: {SlOrderId}",
                tpOrder.GroupId, tpOrder.Id, slOrder.Id);
            throw;
        }
    }

    private async void UpsertOrder(IOrder order, CancellationToken ct)
    {
        try
        {
            await _orderRepository.UpdateOrder(order.Id, order, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cannot Upsert Order {OrderId} to the Database", order.Id);
        }
    }

    private static void ValidateBlacklist(string symbol, string venue)
    {
        var isBlacklisted = Blacklist.Any(item =>
            string.Equals(item.Symbol, symbol, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(item.Venue, venue, StringComparison.OrdinalIgnoreCase));

        if (isBlacklisted)
        {
            throw new NotSupportedException($"Trading is not supported for symbol '{symbol}' on venue '{venue}'");
        }
    }

    private static void ValidateOrderOwner(IOrder order, IAccount account)
    {
        if (!string.IsNullOrWhiteSpace(order.UserId) && order.UserId != account.UserId ||
            !string.IsNullOrWhiteSpace(order.AccountId) && order.AccountId != account.Id)
            throw new GeException(OrderErrors.UserAccessDenied);
    }

    private static void ValidateOrdersOwner(IEnumerable<IOrder> orders, IAccount account)
    {
        if (orders.Any(x => (!string.IsNullOrWhiteSpace(x.UserId) && x.UserId != account.UserId) ||
                            (!string.IsNullOrWhiteSpace(x.AccountId) && x.AccountId != account.Id)))
            throw new GeException(OrderErrors.UserAccessDenied);
    }

    private static void ValidateOrderPermission(IAccount account, OrderSide side)
    {
        switch (side)
        {
            case OrderSide.Buy when !account.EnableBuy:
                throw new GeException(AccountErrors.NotAllowToBuy);
            case OrderSide.Sell when !account.EnableSell:
                throw new GeException(AccountErrors.NotAllowToSell);
            default: return;
        }
    }

    private async Task<IEnumerable<IOrder>> GetOrders(string refId, CancellationToken ct)
    {
        var orders = (await _orderRepository.GetOrders(refId, ct))?.ToArray();

        //NOTE: can be found in database
        if (orders != null && orders.Length != 0)
            return orders;

        //NOTE: cannot be found in database => not online order and not tp/sl order => single order => refId is orderId
        var order = await _velexaService.GetOrder(orderId: refId, ct);
        return [order];
    }

    private static void ValidateBuyOrderModification(IAccountBalance? account, IOrder order,
        IOrderValues request)
    {
        if (order.OrderType == OrderType.Market)
            return;

        if (!IsIncreaseCost(request, order, out var moreNeededBalance))
            return;

        var currency = order.Currency;
        var balance = account?.GetBalance(Provider.Velexa, currency);
        if (moreNeededBalance >= balance)
            throw new GeException(AccountErrors.InsufficientBalance);
    }

    private async Task ValidateSellOrderModification(IOrder order, IOrderValues request,
        CancellationToken ct)
    {
        if (!IsIncreaseQuantity(request, order, out var moreNeededQuantity))
            return;

        var providerAccountId = order.ProviderInfo.AccountId;
        var position = await _orderQueries.GetPosition(providerAccountId, order.SymbolId, ct);
        if (position == null || !position.CanSell(moreNeededQuantity))
            throw new GeException(PositionErrors.InsufficientHoldings);
    }


    private static bool IsIncreaseCost(IOrderValues req, IOrder order, out decimal? moreNeededBalance)
    {
        decimal? currentCost, newCost;
        if (order.OrderType == OrderType.Stop)
            (currentCost, newCost) = GetCost(order.Quantity, req.Quantity, order.StopPrice!.Value, req.StopPrice);
        else
            (currentCost, newCost) = GetCost(order.Quantity, req.Quantity, order.LimitPrice!.Value, req.LimitPrice);

        var isIncrease = newCost > currentCost;
        moreNeededBalance = isIncrease ? newCost - currentCost : 0;

        return isIncrease;
    }

    private static (decimal? currentCost, decimal? newCost) GetCost(decimal oldQuantity, decimal newQuantity, decimal oldPrice, decimal? newPrice)
    {
        return (oldQuantity * oldPrice, newQuantity * (newPrice ?? oldPrice));
    }

    private static bool IsIncreaseQuantity(IOrderValues res, IOrder order, out decimal moreNeededQuantity)
    {
        var isIncrease = res.Quantity > order.Quantity;
        moreNeededQuantity = isIncrease ? res.Quantity - order.Quantity : 0;

        return isIncrease;
    }

    private async Task<T> HandlingProviderProcess<T>(Func<Task<T>> method) where T : class
    {
        try
        {
            var result = await method();
            return result;
        }
        catch (PiHttpResponseException ex)
        {
            if (!ex.Message.Contains("Market is closed", StringComparison.OrdinalIgnoreCase))
                throw;

            _logger.LogError(ex, "The order cannot be edited or canceled when the market is closed.");
            throw new GeException(OrderErrors.MarketClose);
        }
    }
}

public class BlacklistItem
{
    public string Symbol { get; set; } = string.Empty;
    public string Venue { get; set; } = string.Empty;
}
