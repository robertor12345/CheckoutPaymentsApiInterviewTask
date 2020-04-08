using System.Threading.Tasks;
using Checkout.Gateway.Payments.Contracts;
using Checkout.Gateway.Payments.Repositories.Entities;

namespace Checkout.Gateway.Payments.Api.Services
{
    public interface IGetPaymentsService
    {
         Task<PaymentTransactionDetails> CreatePaymentDetailsResponse(string sessionId);
         Task<Payment> GetPaymentBySessionId(string sessionId);


    }
}