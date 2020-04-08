using System;
using Checkout.Gateway.Api.Contracts;
using Checkout.Gateway.PaymentsApi.Infrastructure.Exceptions;
using NUnit.Framework;
using FluentAssertions;
using Checkout.Gateway.PaymentsApi.Services;
using Checkout.Gateway.PaymentsApi.Validators;

namespace Checkout.Gateway.PaymentsApi.UnitTests
{
    public class PaymentsRequestValidatorShould
    {

        [TestCase("88c36b14-5381-40bf-be2a-a3ce75d6202b", "88c36b14-5381-40bf-be2a-a3ce75d6202b", "5555555555554444", 12, 2020, 999, "GBP", 99.99 )]
        public void NotThrowException_WhenValidatePaymentRequestInformationIsCalledAndPaymentRequestIsValid( string merchantSessionId, string paymentId, string cardNumber, int expiryMonth, int expiryYear, int cvv, string currency, double paymentAmount)
        {
            var transactionStartTime = new DateTime(1, 1, 1, 1, 1, 1, 1);

            var paymentMethodDetails = new PaymentMethodDetails(cardNumber, expiryMonth, expiryYear, cvv);

            var paymentRequest = new PaymentRequest(merchantSessionId, paymentId, transactionStartTime,
                paymentMethodDetails, paymentAmount, currency);

            Action action = () => PaymentRequestValidator.ValidatePaymentRequestInformation(paymentRequest);

            action.Should().NotThrow<PaymentRequestInvalidException>();

        }

        [TestCase("88c36b14-5381-40bf-be2a-a", "88c36b14-5381-40bf-be2a-a3ce75d6202b", "5555555555554444", 12, 20, 999, "GBP", 99.99)]
        [TestCase("88c36b14-5381-40bf-be2a-a3ce75d620b", "88c36b14-5381-40bf-be2a", "5555555555554444", 12, 20, 999, "GBP", 99.99)]
        [TestCase("88c36b14-5381-40bf-be2a-a3ce75d620b", "88c36b14-5381-40bf-be2a-a3ce75d6202b", "5554444", 14, 20, 999, "GBP", 99.99)]
        [TestCase("88c36b14-5381-40bf-be2a-a3ce75d620b", "88c36b14-5381-40bf-be2a-a3ce75d6202b", "5555555555554444", 12, 19, 999, "GBP", 99.99)]
        [TestCase("88c36b14-5381-40bf-be2a-a3ce75d620b", "88c36b14-5381-40bf-be2a-a3ce75d6202b", "5555555555554444", 12, 20, 1000, "GBP", 99.99)]
        [TestCase("88c36b14-5381-40bf-be2a-a3ce75d620b", "88c36b14-5381-40bf-be2a-a3ce75d6202b", "5555555555554444", 12, 20, 999, "NO", 99.99)]
        public void ThrowPaymentRequestInvalidException_WhenValidatePaymentRequestInformationIsCalledAndPaymentRequestIsValid(string merchantSessionId, string paymentId, string cardNumber, int expiryMonth, int expiryYear, int cvv, string currency, double paymentAmount)
        {
            var transactionStartTime = new DateTime(1, 1, 1, 1, 1, 1, 1);

            var paymentMethodDetails = new PaymentMethodDetails(cardNumber, expiryMonth, expiryYear, cvv);

            var paymentRequest = new PaymentRequest(merchantSessionId, paymentId, transactionStartTime,
                paymentMethodDetails, paymentAmount, currency);

            Action action = () => PaymentRequestValidator.ValidatePaymentRequestInformation(paymentRequest);

            action.Should().Throw<PaymentRequestInvalidException>();

        }
    }
}