using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Checkout.Gateway.Api.Contracts;
using Newtonsoft.Json;

namespace Checkout.Gateway.PaymentsApi.Infrastructure.FakeBankApi
{
    public class BankRequestClient : IBankRequestClient
    {
        private readonly HttpClient _httpClient;

        public BankRequestClient(HttpClient httpClient)
        {
                _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> CallApi(BankTransactionRequest bankTransactionRequest)
        {
            var content = ComposeBankTransactionRequest(bankTransactionRequest);

            var response = await _httpClient.PostAsync("http://dummy-bank-url/api/validate-method", content).ConfigureAwait(false);

            return await Task.FromResult(response);
        }

        private static StringContent ComposeBankTransactionRequest(BankTransactionRequest bankTransactionRequest)
        {
            var content = new StringContent(JsonConvert.SerializeObject(bankTransactionRequest), Encoding.UTF8,
                "application/json");
            return content;
        }
    }
}
