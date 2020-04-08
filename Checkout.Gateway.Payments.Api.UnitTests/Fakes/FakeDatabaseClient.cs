using System;
using System.Threading.Tasks;
using Checkout.Gateway.Payments.Contracts;
using Checkout.Gateway.Payments.Repositories.Clients;
using Checkout.Gateway.Payments.Repositories.Entities;

namespace Checkout.Gateway.Payments.Api.UnitTests.Fakes
{
    public class FakeDatabaseClient: IDatabaseClient
    {

        private Payment _dbResponse;
        public bool _isSuccessful;

        public void SuccessfullyReturns(Payment dbResponse)
        {
            _isSuccessful = true;
            _dbResponse = dbResponse;

        }

        public void ThrowsException()
        {
            _isSuccessful = false; ;
        }

        public void SuccessfulWithNoResult()
        {
            _isSuccessful = true;
        }

        public Task<Payment> QueryPaymentsBySessionAsync(string sessionId)
        {

            if (!_isSuccessful)
            {
                throw new Exception("Db Query Failed");
            }

            return Task.FromResult(_dbResponse);
        }

        public Task<int> InsertPaymentRecord(PaymentRequest paymentRequest, string paymentId, bool success)
        {
            return Task.FromResult(_isSuccessful ? 1 : 0);
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public void SuccessfullyInsertsPaymemnt()
        {
            _isSuccessful = true;
        }
    }
}