using System;
using Checkout.Gateway.Api.Contracts;
using Checkout.Gateway.PaymentsApi.Infrastructure;
using Checkout.Gateway.PaymentsApi.Infrastructure.Exceptions;

namespace Checkout.Gateway.PaymentsApi.Validators
{
    public static class PaymentRequestValidator
    {
        public static void ValidatePaymentRequestInformation(PaymentRequest paymentRequest)
        {
            ValidateTransactionDetails(paymentRequest);
            ValidateAPaymentDetails(paymentRequest.PaymentMethodDetails);
            ValidateMoneyDetails(paymentRequest.Currency);
        }

        private static void ValidateMoneyDetails(string currency)
        {

            if (!Constants.Currencies.Contains(currency))
            {
                throw new PaymentRequestInvalidException($"Payment Request failed Validation. The Field currencies had the value {currency} which is not a valid currency");
            }
        }

        private static void ValidateAPaymentDetails(PaymentMethodDetails paymentMethodDetails)
        {
            if (paymentMethodDetails.CardNumber.Length != 16)
            {
                throw new PaymentRequestInvalidException($"Payment Request failed Validation. The Field cardNumber was invalid value");
            }

            ValidateCardExpiry(paymentMethodDetails);

            if (paymentMethodDetails.Cvv > 1000 || paymentMethodDetails.Cvv < 100)
            {
                throw new PaymentRequestInvalidException($"Payment Request failed Validation. The Field cvv was invalid value");
            }
        }

        private static void ValidateCardExpiry(PaymentMethodDetails paymentMethodDetails)
        {
            var currentYear = int.Parse(DateTime.Now.ToString("yy"));

            var currentMonth = int.Parse(DateTime.Now.ToString("MM"));

            if (paymentMethodDetails.ExpiryMonth > 12 || paymentMethodDetails.ExpiryMonth < 1)
            {
                throw new PaymentRequestInvalidException($"Payment Request failed Validation. The Field expiryMonth had the value {paymentMethodDetails.ExpiryMonth} which falls outside the valid month range");
            }

            if (paymentMethodDetails.ExpiryYear < currentYear)
            {
                throw new PaymentRequestInvalidException($"Payment Request failed Validation.The Field expiryYear had the value { paymentMethodDetails.ExpiryYear } which falls outside the valid year range") ;
            }
            else if (paymentMethodDetails.ExpiryYear == currentYear && paymentMethodDetails.ExpiryMonth <= currentMonth)
            {
                throw new PaymentRequestInvalidException($"Payment Request failed Validation.The card expiry date details (MM/YY) {paymentMethodDetails.ExpiryMonth}/{paymentMethodDetails.ExpiryYear} falls out of the valid date range");
            }
        }

        private static void ValidateTransactionDetails(PaymentRequest paymentRequest)
        {
            if (!Guid.TryParse(paymentRequest.MerchantSessionId, out var newGuid))
            {
                throw new PaymentRequestInvalidException($"Payment Request failed Validation.The Field merchantSellerID had the value { paymentRequest.MerchantSessionId } which is not a valid GUID");
            }
        }
    }
}