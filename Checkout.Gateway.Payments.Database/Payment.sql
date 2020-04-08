CREATE TABLE [dbo].[Payment]
(
	[PaymentId] NVARCHAR(50) NOT NULL , 
    [SessionId] NVARCHAR(50) NOT NULL UNIQUE, 
    [TransactionStartTime] DATETIME2 NOT NULL, 
    [CardNumber] NVARCHAR(50) NOT NULL, 
    [ExpiryMonth] INT NOT NULL, 
    [ExpiryYear] INT NOT NULL, 
    [Cvv] INT NOT NULL, 
    [PaymentAmount] FLOAT NOT NULL, 
    [Currency] NVARCHAR(50) NOT NULL, 
    [Success] NVARCHAR(50) NOT NULL, 
    CONSTRAINT PK_Payment_SesionId PRIMARY KEY CLUSTERED ([SessionId])
)
