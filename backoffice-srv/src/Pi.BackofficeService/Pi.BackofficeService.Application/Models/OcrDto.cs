using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi.BackofficeService.Application.Models
{
    public record OcrFileUploadModel(
        byte[]? Data,
        string? FileName
    );

    public class OcrThirdPartyApiResponse
    {
        [JsonProperty("data")]
        public string? Data { get; set; }

        [JsonProperty("balance_data")]
        public Dictionary<string, string>? BalanceData { get; set; }

        [JsonProperty("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }

        [JsonProperty("ocr_confidence")]
        public double? OcrConfidence { get; set; }

        [JsonProperty("error")]
        public string? Error { get; set; }
    }

    public enum OcrDocumentType
    {
        [ApiPath("/ocr/bank_statement")]
        BankStatement,

        [ApiPath("/ocr/bank_book")]
        BankBook,

        [ApiPath("/ocr/bonds")]
        Bonds,

        [ApiPath("/ocr/lottery")]
        Lottery,

        [ApiPath("/ocr/mutual_fund")]
        MutualFund,

        [ApiPath("/ocr/securities")]
        Securities,

        [ApiPath("/ocr/salary_slip")]
        SalarySlip
    }

    public enum OcrOutputType
    {
        Data,
        Csv
    }
}
