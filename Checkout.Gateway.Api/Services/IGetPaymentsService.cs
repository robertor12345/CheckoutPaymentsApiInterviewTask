using System.Threading.Tasks;
using Checkout.Gateway.Api.Contracts;
using Checkout.Gateway.PaymentsApi.Entities;

namespace Checkout.Gateway.PaymentsApi.Services
{
    public interface IGetPaymentsService
    {
        public Task<PaymentTransactionDetails> CreatePaymentDetailsResponse(string sessionId);
        public Task<Payment> GetPaymentBySessionId(string sessionId);
    }
}