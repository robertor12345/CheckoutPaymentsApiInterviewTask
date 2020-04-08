using System;
using Checkout.Gateway.Payments.Api.Services;
using Checkout.Gateway.Payments.Api.UnitTests.Fakes;
using Checkout.Gateway.Payments.Contracts;
using Checkout.Gateway.Payments.Repositories.Entities;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace Checkout.Gateway.Payments.Api.UnitTests
{
    public class GetPaymentsServiceShould
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CreatePaymentSessionResponse()
        {
            const string merchantSessionId = "DummySessionId";
            
            const string paymentId = "DummyPaymentId";

            var transactionStartTime = new DateTime(1, 1, 1, 1, 1, 1, 1);

            const string cardNumber = "5555555555554444";
            const int expiryMonth = 12;
            const int expiryYear = 2020;
            const int cvv = 999;

            var paymentMethodDetails = new PaymentMethodDetails(cardNumber, expiryMonth, expiryYear, cvv);

            const double paymentAmount = 999.99;

            const string currency = "GBP";
            
            const string success = "true";

            var expectedResponse = new PaymentTransactionDetails(merchantSessionId, paymentId, transactionStartTime, paymentMethodDetails, paymentAmount, currency, success);

            var fakeDbCLient = new FakeDatabaseClient();

            fakeDbCLient.SuccessfullyReturns(new Payment
            {
                SessionId = merchantSessionId, PaymentId = paymentId, TransactionStartTime = transactionStartTime,
                CardNumber = cardNumber,
                ExpiryMonth = expiryMonth, ExpiryYear = expiryYear, Cvv = cvv, PaymentAmount = paymentAmount,
                Currency = currency, Success = success
            });

            var getPaymentService = new GetPaymentService(new Mock<ILogger>().Object, fakeDbCLient);

            var response = getPaymentService.CreatePaymentDetailsResponse(merchantSessionId);


            response.Result.Should().BeOfType<PaymentTransactionDetails>();

            response.Result.Should().BeEquivalentTo(expectedResponse);

        }


        [Test]
        public void ThrowException_WhenCreatePaymentSessionResponseIsCalledAndNoPaymentRecordsWereFound()
        {
            const string merchantSessionId = "DummySessionId";
            
            var fakeDbCLient = new FakeDatabaseClient();

            fakeDbCLient.SuccessfulWithNoResult();

            var getPaymentService = new GetPaymentService(new Mock<ILogger>().Object, fakeDbCLient);

            Action action = () => getPaymentService.CreatePaymentDetailsResponse(merchantSessionId);

            action.Should().Throw<Exception>("Payment Not Found");

        }

    }
}