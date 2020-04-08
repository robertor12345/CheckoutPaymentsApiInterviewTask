using System.Net.Http;
using System.Threading.Tasks;
using Checkout.Gateway.Api.Contracts;

namespace Checkout.Gateway.PaymentsApi.Infrastructure.FakeBankApi
{
    public interface IBankRequestClient
    {
        public Task<HttpResponseMessage> CallApi(BankTransactionRequest bankTransactionRequest);
    }
}