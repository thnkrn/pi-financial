using System;
using System.Text.Json.Serialization;

namespace Pi.WalletService.API.Models
{
    public class FreewillCallbackDtoBase
    {
        public FreewillCallbackDtoBase(string referId, string transId, string sendDate, string sendTime, string resultCode, string reason)
        {
            ReferId = referId;
            TransId = transId;
            SendDate = sendDate;
            SendTime = sendTime;
            ResultCode = resultCode;
            Reason = reason;
        }

        [JsonPropertyName("refer_id")]
        public string ReferId { get; init; }

        [JsonPropertyName("trans_id")]
        public string TransId { get; init; }

        [JsonPropertyName("send_date")]
        public string SendDate { get; init; }

        [JsonPropertyName("send_time")]
        public string SendTime { get; init; }

        [JsonPropertyName("result_code")]
        public string ResultCode { get; }

        [JsonPropertyName("reason")]
        public string Reason { get; }
    }

}

