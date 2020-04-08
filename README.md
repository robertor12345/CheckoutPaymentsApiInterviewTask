# Gateway Payments Api Interview Task

This is a simple payments gateway solution I have implemented consisting of an API with SQL server storage and an in memory stub representing a bank approval service.

You can find the API spec documentation in the Documents folder within this solution (gateway-api-v1.yaml).

## Assumptions Made

1. I assumed that we will be receiving a sessionId from the merchants and that this Id would be unique and wont be discarded by them so they can use the same id to query the payments history.
2. I have designed and implemented the bank adapter component around the assumption that all the bank integrations will be done via calls to an external bank api and not events with callbacks.
2. The models I have produced for the payment requests assume that the only payment methods we would accept are card based.

## Areas of Improvement
1. Given more experience in the area, I would have implemented robust endpoint authentication, as well as an encryption system for the responses containing sensitive customer and corporate information like card details and transaction times.
2. Given more time I would have implemented proper containerisation of the internal dependency on database storage for the integration tests, I would also have created a devops-style build script using yaml that would have built and deployed the api alongside all its dependencies to a Docker container for easy deployment and setup.
3. Had I also been able to containerise the application i would also have made a more representative fake app service that would process the bank transaction requests outside of the application’s own memory, saving runtime memory for the real processes.
4. Performance testing is also something I would have explored given more time to be able to create a local container.
2. Extending the payment methods integration to accept different parametes that align with non-card based integrations with services like Klarna would be good.
3. Around perhaps splitting the bank transfer service into two modules with an event bus separating calls to the external bank service and payment record storage would solve a potential issue with the transaction completing but the record not saving but also not being retried.
4. Given more resource- I would have implemented remote centralised logging on a cloud service like application insights, alongside unique operation tagging for each trace message in the logs so it is clearer which actions came from which request.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

The things you need to build and run the API and how to install them.

Install Visual Studio 2019 version 16.4 or higher alongside .NET Core 3.1 and the SSDT (sql server data tools) SDK

Download and install SQL Server Managment Tools here: https://docs.microsoft.com/en-us/sql/ssms/release-notes-ssms?redirectedfrom=MSDN&view=sql-server-ver15

Install a HTTP request composer - Postman is recommended.

Resharper is recommended for the best experience when viewing and running the tests, but can also be run using the default visual studio test runner

```
Give examples
```

### Installing

A step by step guide that tells you how to get the development env running

Setup your local sql data store:

```
Given that you have installed SQL Server Managment Tools  as specified in the prerequisits. 

Run the following command in your command line to create a new local sql server:

sqllocaldb create "MSSQLLocalDB"

This should create a SQL server with the default name "MSSQLLocalDB".
```

Deploy the required database schema to local SQL database instance:

```
until finished

1. Open the solution file: "Checkout.GatewayAPI.sln" using Visual Studio 2019

2. In the Solution Explorer window, double click on the: "Checkout.Gateway.Payments.Database.publish.xml" file

3. In the "Target Database Connection" field  the value should be 
"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False"

4. In the "Database Name" field  the value should be "PaymentsDatabase"

5. Once you have verified that both of these are correct, click publish

6. You should now be able to view the sql server "MSSQLLocalDB" and the new database "PaymentsDatabase"  after connecting within SQL Server Managment Studio
```
Run the Payments API:

```
1. Open the solution file: "Checkout.GatewayAPI.sln" using Visual Studio 2019

2. Once loaded, at the very top of the window in the debug section you should see a play button with the a lable and a dropdown to the right of it.

3. Select the dropdownm and choose the Checkout.Gateway.Api project. Click Run

4. The Api should nbow be running on your local machine at the https address: https://localhost:5001/gateway/payments/v1/ and the http address https://localhost:5000/gateway/payments/v1/ 

5. keep this running for the next set of steps.
```

Making a new Payment request:

```
1. Open the json file \CheckoutGatewayAPI\Documents\PostmanRequestExamples.json within postman

2. You will see three different request examples, a post request with valid parameters (PostPaymentRequest_Valid),
	a post request with invalid card fields (PostPaymentRequest_Invalid), 
	and a get request (GetPaymentForASessionRequest) example template with tag to replace with your unique GUID session Id from the successful post request.
	
3. First, Send the PostPaymentRequest_Valid request from postman take note of the merchantSessionId in the response.

4. Now send the GetPaymentForASessionRequest replacing <GuidHere> with the merchantSessionId from the last request. 
	You should get a response that matches that of the model described in the swagger api spec document (CheckoutGatewayAPI\Documents\gateway-api-v1.yaml).

5. Now to check the bad request responses out, send the (PostPaymentRequest_Invalid) request,
	changing the different fields to those that you would expect to be invalid for a card request, not the specific response messages for the different card validation failures.
	
```
## Logging
Local logging with a file sink to (CheckoutGatewayAPI\Checkout.Gateway.Api\Logs\PaymentsApiLogs.txt) has been setup.
You should see all the log entries from the ap in this file in the following format:

```
2020-04-08 13:45:11.886 +01:00 [INF] Get payment record process started for MerchantSession ID: 80933f95-07e9-4d66-99c3-3b7c406f13e6
2020-04-08 13:45:12.012 +01:00 [INF] Get payment record process Completed for MerchantSession ID: 80933f95-07e9-4d66-99c3-3b7c406f13e6
2020-04-08 13:45:12.012 +01:00 [INF] Successfully retrieved payment Information for the Merchant Session ID: 80933f95-07e9-4d66-99c3-3b7c406f13e6.
```

## Running the tests

The tests in this solution are distributed across two projects: 
	Checkout.Gateway.Payments.Api.IntegrationTests
	Checkout.Gateway.Payments.Api.UnitTests
	
These tests are intended to validate the logic in the solution throughout the main use cases as well as the points of integration with external dependencies.

```
1. To run these tests, open the solution file: "Checkout.GatewayAPI.sln" using Visual Studio 2019 
2. Navigate to Tests --> TestExplorer and select RunAllTests
```


## Authors

* **Roberto Brnco-Rhodes** - *complete solution by* - [robertor12345](https://github.com/robertor12345)

