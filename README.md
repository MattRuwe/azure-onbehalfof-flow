# Azure On-Behalf-Of Flow Sample
Demonstrates how to use on-behalf-of flow in Microsoft's identity platform

While Microsoft's documentation on this topic is good, there aren't any fully functional demonstrations of the on-behalf-of flow.  The following articles were used to create this demo:
- [https://learn.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-on-behalf-of-flow](https://learn.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-on-behalf-of-flow)
- [https://learn.microsoft.com/en-us/azure/active-directory/develop/sample-v2-code?tabs=apptype#web-api](https://learn.microsoft.com/en-us/azure/active-directory/develop/sample-v2-code?tabs=apptype#web-api)
- [https://learn.microsoft.com/en-us/azure/active-directory/develop/scenario-web-api-call-api-overview](https://learn.microsoft.com/en-us/azure/active-directory/develop/scenario-web-api-call-api-overview)

# Using this sample

**Note:**  
> The details for the app registrations used while creating this demonstration have been obfuscated.  New app registrations will need to be created within a tenant owned by the end user.

## App Registration Setup

Start by creating the app registrations in your own tenant.  The PowerShell script found in the Solution Items demonstrates all of the necessary settings to include.

## Setup application

Modify the `program.cs` files in both the Web API projects updating the `ClientId`, `Domain`, `Tenant`, and `Secret` values as needed.

In the ApiA project, the scope for ApiB will need to be configured as well.