using Newtonsoft.Json;
using System;

namespace Checkout.Gateway.Payments.Contracts
{
    public class PaymentRequest
    {
        [JsonProperty("merchantSessionId", Required = Required.Always)]
        public string MerchantSessionId { get; }

        [JsonProperty("transactionStartTime", Required = Required.Always)]
        public DateTime TransactionStartTime { get; }

        [JsonProperty("paymentMethodDetails", Required = Required.Always)]
        public PaymentMethodDetails PaymentMethodDetails { get; }

        [JsonProperty("paymentAmount", Required = Required.Always)]
        public double PaymentAmount { get; }

        [JsonProperty("currency", Required = Required.Always)]
        public string Currency { get; }


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