using System.Threading.Tasks;
using Checkout.Gateway.Api.Contracts;

namespace Checkout.Gateway.PaymentsApi.Services
{
    public interface IBankTransactionService
    {
        public Task<PaymentResponse> ProcessBankTransaction(PaymentRequest paymentRequest);
    }
}