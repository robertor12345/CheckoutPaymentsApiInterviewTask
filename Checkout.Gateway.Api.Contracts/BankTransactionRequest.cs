using Newtonsoft.Json;

namespace Checkout.Gateway.Api.Contracts
{
    [JsonObject]
    public class BankTransactionRequest
    {
        [JsonProperty("paymentMethodDetails", Required = Required.Always)]
        public PaymentMethodDetails PaymentMethodDetails { get; }

        [JsonProperty("paymentAmount", Required = Required.Always)]
        public double PaymentPaymentAmount { get; }

        [JsonProperty("currency", Required = Required.Always)]
        public string Currency { get; }

        public BankTransactionRequest(PaymentMethodDetails paymentMethodDetails, string currency, double paymentAmount)
        {
            PaymentMethodDetails = paymentMethodDetails;
            Currency = currency;
            PaymentPaymentAmount = paymentAmount;
        }
    }
}