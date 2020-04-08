using Newtonsoft.Json;
using System;

namespace Checkout.Gateway.Payments.Contracts
{
    [JsonObject]
    public class PaymentTransactionDetails
    {
        [JsonProperty("merchantSessionId", Required = Required.Always)]
        public string MerchantSessionId { get; }

        [JsonProperty("paymentID", Required = Required.Always)]
        public string PaymentId { get; }

        [JsonProperty("transactionStartTime", Required = Required.Always)]
        public DateTime TransactionStartTime { get; }

        [JsonProperty("paymentMethodDetails", Required = Required.Always)]
        public PaymentMethodDetails PaymentMethodDetails { get; }

        [JsonProperty("paymentAmount", Required = Required.Always)]
        public double PaymentAmount { get; }

        [JsonProperty("currency", Required = Required.Always)]
        public string Currency { get; }

        [JsonProperty("success", Required = Required.Always)]
        public string Success { get; }

        public PaymentTransactionDetails(string merchantSessionId, string paymentId, DateTime transactionStartTime, PaymentMethodDetails paymentMethodDetails, double paymentAmount, string currency, string success)
        {
            MerchantSessionId = merchantSessionId;
            PaymentId = paymentId;
            TransactionStartTime = transactionStartTime;
            PaymentMethodDetails = paymentMethodDetails;
            PaymentAmount = paymentAmount;
            Currency = currency;
            Success = success;
        }

    }
}