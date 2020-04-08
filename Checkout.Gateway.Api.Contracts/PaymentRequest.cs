using System;
using Newtonsoft.Json;

namespace Checkout.Gateway.Api.Contracts
{
    public class PaymentRequest
    {
        [JsonProperty("merchantSessionId", Required = Required.Always)]
        public string MerchantSessionId { get; set; }

        [JsonProperty("transactionStartTime", Required = Required.Always)]
        public DateTime TransactionStartTime { get; set; }

        [JsonProperty("paymentMethodDetails", Required = Required.Always)]
        public PaymentMethodDetails PaymentMethodDetails { get; set; }

        [JsonProperty("paymentAmount", Required = Required.Always)]
        public double PaymentAmount { get; set; }

        [JsonProperty("currency", Required = Required.Always)]
        public string Currency { get; set; }

        public PaymentRequest()
        {
        }

        public PaymentRequest(string merchantSessionId, string paymentId, DateTime transactionStartTime, PaymentMethodDetails paymentMethodDetails, double paymentAmount, string currency)
        {
            MerchantSessionId = merchantSessionId;
            TransactionStartTime = transactionStartTime;
            PaymentMethodDetails = paymentMethodDetails;
            PaymentAmount = paymentAmount;
            Currency = currency;
        }
    }
}