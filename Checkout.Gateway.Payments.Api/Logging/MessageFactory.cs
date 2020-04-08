using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Checkout.Gateway.PaymentsApi.Logging
{
    public static class LogMessageFactory
    {

        public const string GetPaymentDetailsSuccessMessageTemplate =
            "Successfully retrieved payment Information for the Merchant Session ID: {MerchantSessionID}.";

        public const string GetPaymentDetailsFailedMessageTemplate =
            "Failed to retrieve payment Information for the Merchant Session ID: {MerchantSessionID}.";

        public const string GetPaymentDetailsFailedWithUnhandledExceptionMessageTemplate =
            "Payment Failed - an Unhandled Exception has occured when attempting to retrieve payment for the Merchant Session ID {MerchantSessionID}";

        public const string PaymentProcessSuccessMessageTemplate =
            "Successfully retrved and processed payment information for the Merchant Session ID: {MerchantSesionID}";

        public const string PaymentProcessFailedMessageTemplate =
            "Unable to process the payment for Merchant Session ID: {MerchantSesionID}. Payment declined at Bank";

        public const string BankTransactionProcessStartedMessageTemplate =
            "Started Bank transaction process for Merchant Session ID {MerchantSessionID}";

        public const string BankTransactionProcessCompletedMessageTemplate =
            "Completed Bank transaction process for Merchant Session ID {MerchantSessionID}";

        public const string InsertPaymentRecordMessageTemplate =
            "Attempting to insert Payment Record status for Merchant Session ID: {MerchantSessionID} into the database.";

        public const string InsertPaymentRecordFailedMessageTemplate =
            "Failed insert Payment Record status for Merchant Session ID: {MerchantSessionID} into the database. A SQL exception Was thrown";

    }
}

