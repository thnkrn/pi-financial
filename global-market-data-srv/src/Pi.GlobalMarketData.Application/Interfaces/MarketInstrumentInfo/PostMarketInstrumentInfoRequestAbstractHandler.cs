using Pi.GlobalMarketData.Application.Abstractions;
using Pi.GlobalMarketData.Application.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi.GlobalMarketData.Application.Interfaces.MarketInstrumentInfo;

public abstract class PostMarketInstrumentInfoRequestAbstractHandler : RequestHandler<PostMarketInstrumentInfoRequest, PostMarketInstrumentInfoResponse>;
