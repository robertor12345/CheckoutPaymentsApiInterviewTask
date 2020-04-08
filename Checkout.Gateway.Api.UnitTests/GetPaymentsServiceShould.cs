using System;
using Checkout.Gateway.Api.Contracts;
using Checkout.Gateway.PaymentsApi.Entities;
using Checkout.Gateway.PaymentsApi.Infrastructure.Exceptions;
using Checkout.Gateway.PaymentsApi.Infrastructure.Logging;
using Checkout.Gateway.PaymentsApi.Services;
using Checkout.Gateway.PaymentsApi.UnitTests.Fakes;
using NUnit.Framework;
using FluentAssertions;
using Moq;
using Serilog;

namespace Checkout.Gateway.PaymentsApi.UnitTests
{
    public class GetPaymentsServiceShould
    {
        private const string MerchantSessionId = "DummySessionId";
        private const string PaymentId = "DummyPaymentId";

        private const string CardNumber = "5555555555554444";
        private const string MaskedCardNumber = "4444";
        private const int ExpiryMonth = 12;
        private const int ExpiryYear = 2020;
        private const int Cvv = 999;

        [Test]
        public void CreatePaymentSessionResponse()
        {
            var transactionStartTime = new DateTime(1, 1, 1, 1, 1, 1, 1);

            var paymentMethodDetails = new PaymentMethodDetails(MaskedCardNumber, ExpiryMonth, ExpiryYear, Cvv);

            const double paymentAmount = 999.99;

            const string currency = "GBP";

            const string success = "true";

            var expectedResponse = new PaymentTransactionDetails(MerchantSessionId, PaymentId, transactionStartTime, paymentMethodDetails, paymentAmount, currency, success);

            var fakeDatabaseClient = new FakeDatabaseClient();

            fakeDatabaseClient.SuccessfullyReturns(new Payment
            {
                SessionId = MerchantSessionId, PaymentId = PaymentId, TransactionStartTime = transactionStartTime,
                CardNumber = CardNumber,
                ExpiryMonth = ExpiryMonth, ExpiryYear = ExpiryYear, Cvv = Cvv, PaymentAmount = paymentAmount,
                Currency = currency, Success = success
            });

            var getPaymentService = new GetPaymentRecordService(new Mock<ILogger>().Object, fakeDatabaseClient);

            var response = getPaymentService.CreatePaymentDetailsResponse(MerchantSessionId).GetAwaiter().GetResult();

            response.Should().BeOfType<PaymentTransactionDetails>();

            response.Should().BeEquivalentTo(expectedResponse);

        }

        [Test]
        public void MaskLongCardNumber_WhenMaskLongCardNumberIsCalled()
        {
            var transactionStartTime = new DateTime(1, 1, 1, 1, 1, 1, 1);

            const double paymentAmount = 999.99;

            const string currency = "GBP";

            const string success = "true";

            var fakeDatabaseClient = new FakeDatabaseClient();

            fakeDatabaseClient.SuccessfullyReturns(new Payment
            {
                SessionId = MerchantSessionId,
                PaymentId = PaymentId,
                TransactionStartTime = transactionStartTime,
                CardNumber = CardNumber,
                ExpiryMonth = ExpiryMonth,
                ExpiryYear = ExpiryYear,
                Cvv = Cvv,
                PaymentAmount = paymentAmount,
                Currency = currency,
                Success = success
            });

            var getPaymentService = new GetPaymentRecordService(new Mock<ILogger>().Object, fakeDatabaseClient);

            var response = getPaymentService.MaskLongCardNumber(CardNumber);

            response.Should().Be(MaskedCardNumber);
        }

        [Test]
        public void ThrowException_WhenCreatePaymentSessionResponseIsCalledAndNoPaymentRecordsWereFound()
        {
            
            var fakeDatabaseClient = new FakeDatabaseClient();

            fakeDatabaseClient.SuccessfulWithNoResult();

            var getPaymentService = new GetPaymentRecordService(new Mock<ILogger>().Object, fakeDatabaseClient);

            Action action = () => getPaymentService.CreatePaymentDetailsResponse(MerchantSessionId).ConfigureAwait(false);

            action.Should().Throw<Exception>("Payment Not Found");
        }

        [Test]
        public void WriteLogsStartMessage_WhenCreatePaymentSessionResponseIsCalled()
        {

            var transactionStartTime = new DateTime(1, 1, 1, 1, 1, 1, 1);

            const double paymentAmount = 999.99;

            const string currency = "GBP";

            const string success = "true";

            var fakeDatabaseClient = new FakeDatabaseClient();

            fakeDatabaseClient.SuccessfullyReturns(new Payment
            {
                SessionId = MerchantSessionId,
                PaymentId = PaymentId,
                TransactionStartTime = transactionStartTime,
                CardNumber = CardNumber,
                ExpiryMonth = ExpiryMonth,
                ExpiryYear = ExpiryYear,
                Cvv = Cvv,
                PaymentAmount = paymentAmount,
                Currency = currency,
                Success = success
            });

            var logger = new Mock<ILogger>();

            var getPaymentService = new GetPaymentRecordService(logger.Object, fakeDatabaseClient);

            getPaymentService.CreatePaymentDetailsResponse(MerchantSessionId).ConfigureAwait(false);

            logger.Verify(l => l.Information(MessageFactory.GetPaymentRecordProcessStartedLogMessageTemplate, MerchantSessionId), Times.Once);
        }

        [Test]
        public void WriteLogsCompletedMessage_WhenCreatePaymentSessionResponseIsCalledAndRetrievedSuccessfully()
        {
            var transactionStartTime = new DateTime(1, 1, 1, 1, 1, 1, 1);

            const double paymentAmount = 999.99;

            const string currency = "GBP";

            const string success = "true";

            var fakeDatabaseClient = new FakeDatabaseClient();

            fakeDatabaseClient.SuccessfullyReturns(new Payment
            {
                SessionId = MerchantSessionId,
                PaymentId = PaymentId,
                TransactionStartTime = transactionStartTime,
                CardNumber = CardNumber,
                ExpiryMonth = ExpiryMonth,
                ExpiryYear = ExpiryYear,
                Cvv = Cvv,
                PaymentAmount = paymentAmount,
                Currency = currency,
                Success = success
            });

            var logger = new Mock<ILogger>();

            var getPaymentService = new GetPaymentRecordService(logger.Object, fakeDatabaseClient);

            getPaymentService.CreatePaymentDetailsResponse(MerchantSessionId).ConfigureAwait(false);

            logger.Verify(l => l.Information(MessageFactory.GetPaymentRecordProcessCompletedLogMessageTemplate, MerchantSessionId), Times.Once);
        }

        [Test]
        public void WriteLogsError_WhenCreatePaymentSessionResponseIsCalledAndNoPaymentRecordsWereFound()
        {
            var fakeDatabaseClient = new FakeDatabaseClient();

            fakeDatabaseClient.SuccessfulWithNoResult();

            var logger = new Mock<ILogger>();

            var getPaymentService = new GetPaymentRecordService(logger.Object, fakeDatabaseClient);

            Action action = () => getPaymentService.CreatePaymentDetailsResponse(MerchantSessionId).ConfigureAwait(false);

            action.Should().Throw<PaymentNotFoundException>("Payment Not Found");

            logger.Verify(l => l.Error(MessageFactory.GetPaymentRecordProcessSFailedLogMessageTemplate, MerchantSessionId), Times.Once);
        }
    }
}
