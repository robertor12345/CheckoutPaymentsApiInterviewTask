using System;
using Checkout.Gateway.Payments.Api.Controllers;
using Checkout.Gateway.Payments.Api.UnitTests.Fakes;
using Checkout.Gateway.Payments.Contracts;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ILogger = Serilog.ILogger;

namespace Checkout.Gateway.Payments.Api.UnitTests
{
    public class PaymentsControllerShould
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ReturnOk_WhenGetPaymentDetailsIsCalledAndPastPaymentExists()
        {
            const string merchantSessionId = "DummySessionId";
            
            const string paymentId = "DummyPaymentId";

            var transactionStartTime = DateTime.Now;


            const string cardNumber = "5555555555554444";
            const int expiryMonth = 12;
            const int expiryYear= 2020;
            const int  cvv = 999;

            var  paymentMethodDetails = new PaymentMethodDetails(cardNumber, expiryMonth, expiryYear, cvv);

            const double paymentAmount  = 999.99;

            const string currency = "GBP";

            const string success = "true";

            var expectedResponse = new PaymentTransactionDetails(merchantSessionId, paymentId, transactionStartTime, paymentMethodDetails, paymentAmount, currency, success);

            var fakeGetPaymentService = new FakeGetPaymentService();

            fakeGetPaymentService.SuccessfullyReturns(expectedResponse);

            var paymentsController = new PaymentsController(Mock.Of<ILogger>(), fakeGetPaymentService);

            var result = paymentsController.GetPaymentDetails(merchantSessionId).Result;

            result.Should().BeOfType<OkObjectResult>();

        }

        [Test]
        public void ReturnBadRequestResult_WhenGetPaymentDetailsIsCalledAndPastPaymentDoesNotExist()
        {
            const string merchantSessionId = "DummySessionId";

            var fakeGetPaymentService = new FakeGetPaymentService();

            fakeGetPaymentService.ThrowsException();

            var paymentsController = new PaymentsController(Mock.Of<ILogger>(), fakeGetPaymentService);

            var result = paymentsController.GetPaymentDetails(merchantSessionId).Result;

            result.Should().BeOfType<BadRequestObjectResult>();

        }


        [Test]
        public void ReturnOk_WhenProcessPaymentIsCalledAndProcessedSuccessfully()
        {
            const string merchantSessionId = "DummySessionId";

            const string paymentId = "DummyPaymentId";

            var transactionStartTime = DateTime.Now;

            const string cardNumber = "5555555555554444";
            const int expiryMonth = 12;
            const int expiryYear = 2020;
            const int cvv = 999;

            var paymentMethodDetails = new PaymentMethodDetails(cardNumber, expiryMonth, expiryYear, cvv);

            const double paymentAmount = 999.99;

            const string currency = "GBP";

            var paymentRequest = new PaymentRequest(merchantSessionId, paymentId, transactionStartTime, paymentMethodDetails, paymentAmount, currency);


            var fakeBankTransactionService  = new FakeBankTransactionService(paymentRequest);

            fakeBankTransactionService.SuccessfullyReturns(new PaymentResponse(merchantSessionId, paymentId, true, paymentAmount, currency));

            var paymentsController = new PaymentsController(Mock.Of<ILogger>(), null, fakeBankTransactionService);

            var result = paymentsController.ProcessPayment(paymentRequest).Result;

            result.Should().BeOfType<OkObjectResult>();
            
        }

        [Test]
        public void ReturnBadRequestResult_WhenProcessPaymentIsCalledAndProcessedUnsuccessfully()
        {
            const string merchantSessionId = "DummySessionId";

            const string paymentId = "DummyPaymentId";

            var transactionStartTime = DateTime.Now;

            const string cardNumber = "5555555555554444";
            const int expiryMonth = 12;
            const int expiryYear = 2020;
            const int cvv = 999;

            var paymentMethodDetails = new PaymentMethodDetails(cardNumber, expiryMonth, expiryYear, cvv);

            const double paymentAmount = 999.99;

            const string currency = "GBP";


            var paymentRequest = new PaymentRequest(merchantSessionId, paymentId, transactionStartTime, paymentMethodDetails, paymentAmount, currency);

            var fakeBankTransactionService = new FakeBankTransactionService(paymentRequest);

            fakeBankTransactionService.SuccessfullyReturns(new PaymentResponse(merchantSessionId, paymentId, true, paymentAmount, currency));

            var paymentsController = new PaymentsController(Mock.Of<ILogger>(), null, fakeBankTransactionService);

            var result = paymentsController.ProcessPayment(paymentRequest).Result;

            result.Should().BeOfType<BadRequestObjectResult>();
        }


    }
}