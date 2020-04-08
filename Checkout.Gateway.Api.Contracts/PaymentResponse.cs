using Newtonsoft.Json;

namespace Checkout.Gateway.Api.Contracts
{
    [JsonObject]
    public class PaymentResponse 
    {
        [JsonProperty("merchantSessionId", Required = Required.Always)]
        public string MerchantSessionId { get; }

        [JsonProperty("paymentID", Required = Required.Always)]
        public string PaymentId { get; }

        [JsonProperty("success", Required = Required.Always)]
        public bool Success { get; }

        [JsonProperty("paymentAmount", Required = Required.Always)]
        public double PaymentAmount { get; }

        [JsonProperty("currency", Required = Required.Always)]
        public string Currency { get; }

        public PaymentResponse(string merchantSessionId, string paymentId, bool success, in double paymentAmount, string currency)
        {
            MerchantSessionId = merchantSessionId;
            PaymentId = paymentId;
            Success = success;
            PaymentAmount = paymentAmount;
            Currency = currency;
        }
    }
}