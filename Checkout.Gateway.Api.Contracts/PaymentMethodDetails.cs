using Newtonsoft.Json;

namespace Checkout.Gateway.Api.Contracts
{
    [JsonObject]
    public class PaymentMethodDetails
    {
        [JsonProperty("cardNumber", Required = Required.Always)]
        public string CardNumber { get; set; }

        [JsonProperty("expiryMonth", Required = Required.Always)]
        public int ExpiryMonth { get; set; }

        [JsonProperty("expiryYear", Required = Required.Always)]
        public int ExpiryYear { get; set; }

        [JsonProperty("cvv", Required = Required.Always)]
        public int Cvv { get; set; }

       public  PaymentMethodDetails () 
       {

       }

        public PaymentMethodDetails(string cardNumber, int expiryMonth, int expiryYear, int cvv)
        {
            CardNumber = cardNumber;
            ExpiryMonth = expiryMonth;
            ExpiryYear = expiryYear;
            Cvv = cvv;
        }
    }
}