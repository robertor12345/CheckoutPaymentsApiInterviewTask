using System;
using System.Net;
using Checkout.Gateway.Api.Contracts;
using NUnit.Framework;
using Checkout.Gateway.PaymentsApi.Controllers;
using Checkout.Gateway.PaymentsApi.UnitTests.Fakes;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ILogger = Serilog.ILogger;
using Checkout.Gateway.PaymentsApi.Infrastructure.Logging;
using Checkout.Gateway.PaymentsApi.Services;

namespace Checkout.Gateway.PaymentsApi.UnitTests
{
    public class PaymentsControllerShould
    {
        private const string MerchantSessionId = "DummySessionId";
        private const string PaymentId = "00000000-0000-0000-0000-000000000000";

        private const string CardNumber = "5555555555554444";
        private const int ExpiryMonth = 12;
        private const int ExpiryYear = 2020;
        private const int Cvv = 999;

        private const double PaymentAmount = 999.99;
        private const string Currency = "GBP";

        [Test]
        public void ReturnOkObjectResult_WhenGetPaymentDetailsIsCalledAndPastPaymentExists()
        {
            var transactionStartTime = DateTime.Now;

            var  paymentMethodDetails = new PaymentMethodDetails(CardNumber, ExpiryMonth, ExpiryYear, Cvv);

            const string success = "true";

            var expectedPaymentTransactionDetails = new PaymentTransactionDetails(MerchantSessionId, PaymentId, transactionStartTime, paymentMethodDetails, PaymentAmount, Currency, success);

            var fakeGetPaymentService = new FakeGetPaymentService();

            fakeGetPaymentService.SuccessfullyReturns(expectedPaymentTransactionDetails);

            var paymentsController = new PaymentsController(Mock.Of<ILogger>(), fakeGetPaymentService, Mock.Of<IBankTransactionService>());

            var result = paymentsController.GetPaymentDetails(MerchantSessionId).GetAwaiter().GetResult();

            var okObjectResult = result.Should().BeOfType<OkObjectResult>();

            okObjectResult.Which.Value.Should().BeEquivalentTo(expectedPaymentTransactionDetails);
        }

        [Test]
        public void ReturnBadRequestResultWithCorrectErrorDetails_WhenGetPaymentDetailsIsCalledAndPastPaymentDoesNotExist()
        {
            var expectedErrorDetails = new ErrorDetails(){StatusCode = (HttpStatusCode)(404), FriendlyErrorMessage = "No Payment record was found for the sessionId DummySessionId" };

            var fakeGetPaymentService = new FakeGetPaymentService();

            fakeGetPaymentService.ThrowsPaymentNotFoundException();

            var paymentsController = new PaymentsController(Mock.Of<ILogger>(), fakeGetPaymentService,Mock.Of<IBankTransactionService>());

            var result = paymentsController.GetPaymentDetails(MerchantSessionId).GetAwaiter().GetResult();

            var badRequestObjectResult = result.Should().BeOfType<BadRequestObjectResult>();
            badRequestObjectResult.Which.StatusCode.Should().Be(404);
            var errorDetails = badRequestObjectResult.Which.Value.Should().BeOfType<ErrorDetails>();
            errorDetails.Which.Should().BeEquivalentTo((expectedErrorDetails));
        }

        [Test]
        public void ReturnOkObjectResult_WhenProcessPaymentIsCalledAndTransactionIsApprovedAtBank()
        {

            var expectedPaymentResponse = new PaymentResponse(MerchantSessionId, PaymentId, true, PaymentAmount, Currency);

            var transactionStartTime = DateTime.Now;

            var paymentMethodDetails = new PaymentMethodDetails(CardNumber, ExpiryMonth, ExpiryYear, Cvv);

            var paymentRequest = new PaymentRequest(MerchantSessionId, PaymentId, transactionStartTime, paymentMethodDetails, PaymentAmount, Currency);


            var fakeBankTransactionService  = new FakeBankTransactionService(paymentRequest);

            fakeBankTransactionService.SuccessfullyReturns(new PaymentResponse(MerchantSessionId, PaymentId, true, PaymentAmount, Currency));

            var paymentsController = new PaymentsController(Mock.Of<ILogger>(), Mock.Of<IGetPaymentsService>(), fakeBankTransactionService);

            var result = paymentsController.ProcessPayment(paymentRequest).GetAwaiter().GetResult();

            var okObjectResult = result.Should().BeOfType<OkObjectResult>();

            okObjectResult.Which.Value.Should().BeEquivalentTo(expectedPaymentResponse);
        }

        [Test]
        public void ReturnOkObjectResult_WhenProcessPaymentIsCalledAndTransactionIsDeclinedAtBank()
        {
            var expectedPaymentResponse = new PaymentResponse(MerchantSessionId, PaymentId, false, PaymentAmount, Currency);

            var transactionStartTime = DateTime.Now;

            var paymentMethodDetails = new PaymentMethodDetails(CardNumber, ExpiryMonth, ExpiryYear, Cvv);

            var paymentRequest = new PaymentRequest(MerchantSessionId, PaymentId, transactionStartTime, paymentMethodDetails, PaymentAmount, Currency);

            var fakeBankTransactionService = new FakeBankTransactionService(paymentRequest);


            fakeBankTransactionService.SuccessfullyReturns(expectedPaymentResponse);

            var paymentsController = new PaymentsController(Mock.Of<ILogger>(), Mock.Of<IGetPaymentsService>(), fakeBankTransactionService);

            var result = paymentsController.ProcessPayment(paymentRequest).GetAwaiter().GetResult();

            var okObjectResult =  result.Should().BeOfType<OkObjectResult>();

            okObjectResult.Which.Value.Should().BeEquivalentTo(expectedPaymentResponse);
        }

        [Test]
        public void ReturnBadRequestResultWithCorrectErrorDetails_WhenProcessPaymentIsCalledAndBankRequestFails()
        {

            var expectedErrorDetails = new ErrorDetails() { StatusCode = (HttpStatusCode)(503), FriendlyErrorMessage = "Cannot process this payment. Unable to contact the Bank to validate the transaction" };

            var transactionStartTime = DateTime.Now;

            var paymentMethodDetails = new PaymentMethodDetails(CardNumber, ExpiryMonth, ExpiryYear, Cvv);

            var paymentRequest = new PaymentRequest(MerchantSessionId, PaymentId, transactionStartTime, paymentMethodDetails, PaymentAmount, Currency);

            var fakeBankTransactionService = new FakeBankTransactionService(paymentRequest);

            fakeBankTransactionService.ThrowsBankRequestFailedException();

            var paymentsController = new PaymentsController(Mock.Of<ILogger>(), Mock.Of<IGetPaymentsService>(), fakeBankTransactionService);

            var result = paymentsController.ProcessPayment(paymentRequest).GetAwaiter().GetResult();


            var badRequestObjectResult = result.Should().BeOfType<BadRequestObjectResult>();
            badRequestObjectResult.Which.StatusCode.Should().Be(503);
            var errorDetails = badRequestObjectResult.Which.Value.Should().BeOfType<ErrorDetails>();
            errorDetails.Which.Should().BeEquivalentTo((expectedErrorDetails));
        }

        [Test] public void ReturnBadRequestResultWithCorrectErrorDetails_WhenProcessPaymentIsCalledAndPaymentDetailsWereInvalid()
        {

            var expectedErrorDetails = new ErrorDetails() { StatusCode = (HttpStatusCode)(422), FriendlyErrorMessage = "Dummy message of payment validation failure type" };

            var transactionStartTime = DateTime.Now;

            var paymentMethodDetails = new PaymentMethodDetails(CardNumber, ExpiryMonth, ExpiryYear, Cvv);

            var paymentRequest = new PaymentRequest(MerchantSessionId, PaymentId, transactionStartTime, paymentMethodDetails, PaymentAmount, Currency);

            var fakeBankTransactionService = new FakeBankTransactionService(paymentRequest);

            fakeBankTransactionService.ThrowsPaymentDetailsInvalidException();

            var paymentsController = new PaymentsController(Mock.Of<ILogger>(), Mock.Of<IGetPaymentsService>(), fakeBankTransactionService);

            var result = paymentsController.ProcessPayment(paymentRequest).GetAwaiter().GetResult();

            var badRequestObjectResult = result.Should().BeOfType<BadRequestObjectResult>();
            badRequestObjectResult.Which.StatusCode.Should().Be(422);
            var errorDetails = badRequestObjectResult.Which.Value.Should().BeOfType<ErrorDetails>();
            errorDetails.Which.Should().BeEquivalentTo((expectedErrorDetails));
        }

        [Test]
        public void WriteSuccessLogMessage_WhenGetPaymentDetailsIsCalledAndPastPaymentExists()
        {
            var transactionStartTime = DateTime.Now;

            var paymentMethodDetails = new PaymentMethodDetails(CardNumber, ExpiryMonth, ExpiryYear, Cvv);

            const string success = "true";

            var expectedResponse = new PaymentTransactionDetails(MerchantSessionId, PaymentId, transactionStartTime, paymentMethodDetails, PaymentAmount, Currency, success);

            var fakeGetPaymentService = new FakeGetPaymentService();

            fakeGetPaymentService.SuccessfullyReturns(expectedResponse);

            var logger = new Mock<ILogger>();

            var paymentsController = new PaymentsController(logger.Object, fakeGetPaymentService, Mock.Of<IBankTransactionService>());

            paymentsController.GetPaymentDetails(MerchantSessionId).ConfigureAwait(false);

            logger.Verify(l => l.Information(MessageFactory.GetPaymentDetailsSuccessMessageTemplate, MerchantSessionId), Times.Once);
        }

        [Test]
        public void WriteUnhandledExceptionFailureLogMessage_WhenGetPaymentDetailsIsCalledAndPastPaymentExists()
        {
            var fakeGetPaymentService = new FakeGetPaymentService();

            fakeGetPaymentService.ThrowsUnhandledException();

            var logger = new Mock<ILogger>();

            var paymentsController = new PaymentsController(logger.Object, fakeGetPaymentService, Mock.Of<IBankTransactionService>());

            paymentsController.GetPaymentDetails(MerchantSessionId).ConfigureAwait(false);

            logger.Verify(l => l.Error(MessageFactory.GetPaymentDetailsFailedWithUnhandledExceptionMessageTemplate, MerchantSessionId), Times.Once);
        }

        [Test]
        public void WritePaymentNotFoundExceptionFailureLogMessage_WhenGetPaymentDetailsIsCalledAndPastPaymentExists()
        {
            var fakeGetPaymentService = new FakeGetPaymentService();

            fakeGetPaymentService.ThrowsPaymentNotFoundException();

            var logger = new Mock<ILogger>();

            var paymentsController = new PaymentsController(logger.Object, fakeGetPaymentService, Mock.Of<IBankTransactionService>());

            paymentsController.GetPaymentDetails(MerchantSessionId).ConfigureAwait(false);

            logger.Verify(l => l.Error(MessageFactory.GetPaymentDetailsFailedMessageTemplate, MerchantSessionId), Times.Once);
        }

        [Test]
        public void WriteSuccessLogMessage_WhenProcessPaymentIsCalledAndTransactionIsApprovedAtBank()
        {
            var transactionStartTime = DateTime.Now;

            var paymentMethodDetails = new PaymentMethodDetails(CardNumber, ExpiryMonth, ExpiryYear, Cvv);

            var paymentRequest = new PaymentRequest(MerchantSessionId, PaymentId, transactionStartTime, paymentMethodDetails, PaymentAmount, Currency);

            var fakeBankTransactionService = new FakeBankTransactionService(paymentRequest);

            fakeBankTransactionService.SuccessfullyReturns(new PaymentResponse(MerchantSessionId, PaymentId, true, PaymentAmount, Currency));

            var logger = new Mock<ILogger>();

            var paymentsController = new PaymentsController(logger.Object, Mock.Of<IGetPaymentsService>(), fakeBankTransactionService);

            paymentsController.ProcessPayment(paymentRequest).ConfigureAwait(false);

            logger.Verify(l => l.Information(MessageFactory.PaymentProcessCompletedWithBankApprovalMessageTemplate, MerchantSessionId), Times.Once);
        }


        [Test]
        public void WriteFailureLogMessage_WhenProcessPaymentIsCalledAndTransactionIsDeclinedAtBank()
        { 

            var transactionStartTime = DateTime.Now;

            var paymentMethodDetails = new PaymentMethodDetails(CardNumber, ExpiryMonth, ExpiryYear, Cvv);

            var paymentRequest = new PaymentRequest(MerchantSessionId, PaymentId, transactionStartTime, paymentMethodDetails, PaymentAmount, Currency);

            var fakeBankTransactionService = new FakeBankTransactionService(paymentRequest);

            fakeBankTransactionService.SuccessfullyReturns(new PaymentResponse(MerchantSessionId, PaymentId, false, PaymentAmount, Currency));

            var logger = new Mock<ILogger>();
            var paymentsController = new PaymentsController(logger.Object, Mock.Of<IGetPaymentsService>(), fakeBankTransactionService);

            paymentsController.ProcessPayment(paymentRequest).ConfigureAwait(false);

            logger.Verify(l => l.Information(MessageFactory.PaymentProcessCompletedWithBankDeclineMessageTemplate, MerchantSessionId), Times.Once);
        }
    }
}