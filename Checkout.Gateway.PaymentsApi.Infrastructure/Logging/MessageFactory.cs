namespace Checkout.Gateway.PaymentsApi.Infrastructure.Logging
{
    public static class MessageFactory
    {
        public const string GetPaymentDetailsSuccessMessageTemplate =
            "Successfully retrieved payment Information for the Merchant Session ID: {MerchantSessionId}.";

        public const string GetPaymentDetailsFailedMessageTemplate =
            "Failed to retrieve payment Information for the Merchant Session ID: {MerchantSessionId}.";

        public const string GetPaymentDetailsFailedWithUnhandledExceptionMessageTemplate =
            "Payment Failed - an Unhandled Exception has occured when attempting to retrieve payment for the Merchant Session ID {MerchantSessionId}";

        public const string GetPaymentRecordProcessCompletedLogMessageTemplate =
            "Get payment record process Completed for MerchantSession ID: {MerchantSessionId}";

        public const string GetPaymentRecordProcessStartedLogMessageTemplate =
            "Get payment record process started for MerchantSession ID: {MerchantSessionId}";

        public const string GetPaymentRecordProcessSFailedLogMessageTemplate =
            "Get payment record process failed for MerchantSession ID: {MerchantSessionId}. No record was found in the database";

        public const string GetPaymentRecordProcessSFailedMessage =
            "A payment record for MerchantSessionId: {MerchantSession ID} was not found, please ensure that this value is correct";

        public const string PaymentProcessCompletedWithBankApprovalMessageTemplate =
            "Successfully processed payment information for the Merchant Session ID: {MerchantSesionId}. The Bank has issued approval status";

        public const string PaymentProcessCompletedWithBankDeclineMessageTemplate =
            "Unable to process the payment for Merchant Session ID: {MerchantSesionId}. Payment declined at Bank";

        public const string BankTransactionProcessStartedMessageTemplate =
            "Started Bank transaction process for Merchant Session ID {MerchantSessionId}";

        public const string BankTransactionProcessCompletedMessageTemplate =
            "Completed Bank transaction process for Merchant Session ID {MerchantSessionId}";

        public const string InsertPaymentRecordMessageTemplate =
            "Attempting to insert Payment Record status for Merchant Session ID: {MerchantSessionID} into the database.";

        public const string SqlConnectionFailedLogMessageTemplate =
            "Failed insert Payment Record for Merchant Session ID: {MerchantSessionId} into the database. Unable to establish a connection with SQL server";

        public const string SaveTransactionRecordFailedLogMessageTemplate =
            "Failed insert Payment Record for Merchant Session ID: {MerchantSessionId} into the database. A SQL exception Was thrown";

        public const string SaveTransactionRecordExceptionMessage =
            "Failed to insert a new payment transaction record. A SQL Exception has occured.";

        public const string SqlConnectionFailureMessage =
            "An exception has occured when establishing a connection to SQL server. ";

        public const string ProcessPaymentFailedUnableToContactBankLogMessageTemplate =
            "Cannot process this payment. Unable to contact the Bank to validate the transaction";

        public const string ProcessPaymentFailedUnableToContactBankMessage =
            "An Exception has occured when attempting to contact the Bank to validate the transaction";

        public const string RetrieveTransactionRecordExceptionMessage =
            "Failed to retrieve payment record from database. A SQL Exception has occured.";

        public const string BankAdapterProcessStartedLogMessage =
            "Bank Adapter has started the bank transaction process for transaction";

        public const string BankAdapterProcessCompletedLogMessage =
            "Bank Adapter has completed the bank transaction process for transaction";

        public const string BankAdapterProcessFailedLogMessage =
            "Bank Adapter has failed transaction process for transaction. Transaction request to Bank was unsuccessful";

    }
}

