using System.Threading.Tasks;
using Checkout.Gateway.Api.Contracts;

namespace Checkout.Gateway.PaymentsApi.Adapters
{
    public interface IBankAdapter
    {
        public Task<BankResponse> CallApi(PaymentMethodDetails paymentMethodDetails, string currency, double paymentAmount);
    }
}