using Newtonsoft.Json;

namespace Checkout.Gateway.Payments.Contracts
{
    [JsonObject]
    public class PaymentMethodDetails
    {
        [JsonProperty("cardNumber", Required = Required.Always)]
        public string CardNumber { get; }

        [JsonProperty("expiryMonth", Required = Required.Always)]
        public int ExpiryMonth { get; }

        [JsonProperty("expiryYear", Required = Required.Always)]
        public int ExpiryYear { get; }

        [JsonProperty("cvv", Required = Required.Always)]
        public int Cvv { get; }

        public PaymentMethodDetails(string cardNumber, int expiryMonth, int expiryYear, int cvv)
        {
            CardNumber = cardNumber;
            ExpiryMonth = expiryMonth;
            ExpiryYear = expiryYear;
            Cvv = cvv;
        }
    }
}