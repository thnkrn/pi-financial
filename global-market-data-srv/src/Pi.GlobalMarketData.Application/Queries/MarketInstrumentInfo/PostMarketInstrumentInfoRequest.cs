using MassTransit.Mediator;
using Pi.GlobalMarketData.Domain.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi.GlobalMarketData.Application.Queries;

public record PostMarketInstrumentInfoRequest(MarketInstrumentInfoRequest Data) : Request<PostMarketInstrumentInfoResponse>;
