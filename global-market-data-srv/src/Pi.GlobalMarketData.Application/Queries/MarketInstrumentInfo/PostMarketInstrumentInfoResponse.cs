using Pi.GlobalMarketData.Domain.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi.GlobalMarketData.Application.Queries;

public record PostMarketInstrumentInfoResponse(MarketInstrumentInfoResponse data);
