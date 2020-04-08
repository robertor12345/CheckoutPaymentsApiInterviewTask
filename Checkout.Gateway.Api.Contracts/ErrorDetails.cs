using System.Net;
using Newtonsoft.Json;

namespace Checkout.Gateway.Api.Contracts
{
    [JsonObject]
    public class ErrorDetails
    {
        [JsonProperty("statusCode")]
        public HttpStatusCode StatusCode { get; set; }

        [JsonProperty("errorMessage")]
        public string FriendlyErrorMessage { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}