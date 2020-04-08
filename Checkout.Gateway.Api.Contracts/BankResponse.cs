using Newtonsoft.Json;

namespace Checkout.Gateway.Api.Contracts
{
    [JsonObject]
    public class BankResponse
    {
        [JsonProperty("transactionId", Required = Required.Always)]
        public string TransactionId { get; set; }

        [JsonProperty("transactionStatus", Required = Required.Always)]
        public bool TransactionStatus { get; set;}

        public BankResponse(string transactionId, bool transactionStatus)
        {
            TransactionId = transactionId;
            TransactionStatus = transactionStatus;
        }
    }
}