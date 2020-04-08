using System.Threading.Tasks;
using Checkout.Gateway.Payments.Contracts;

namespace Checkout.Gateway.PaymentsApi.Adapters
{
    public interface IBankAdapter
    {
        Task<BankResponse> CallApi(PaymentMethodDetails paymentMethod, string currency, double amount);
    }
}