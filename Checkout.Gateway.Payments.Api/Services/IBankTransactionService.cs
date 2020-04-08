using System.Threading.Tasks;
using Checkout.Gateway.Payments.Contracts;

namespace Checkout.Gateway.Payments.Api.Services
{
    public interface IBankTransactionService
    {
         Task<PaymentResponse> ProcessBankTransaction(PaymentRequest paymentRequest);

         Task <BankResponse> RequestBankTransaction();

         Task<int> UpdateTransactionStatus(BankResponse bankResponse);

        Task<PaymentResponse> CreatePaymentResponse(BankResponse bankResponse);

    }
}