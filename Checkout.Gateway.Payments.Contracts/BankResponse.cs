using Newtonsoft.Json;

namespace Checkout.Gateway.Payments.Contracts
{
    [JsonObject]
    public class BankResponse
    {
        [JsonProperty("transactionId", Required = Required.Always)]
        public string TransactionId { get; }

        [JsonProperty("transactionStatus", Required = Required.Always)]
        public bool TransactionStatus { get; }

        public BankResponse(string transactionId, bool transactionStatus)
        {
            TransactionId = transactionId;
            TransactionStatus = transactionStatus;
        }
    }
}