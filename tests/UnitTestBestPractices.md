# Unit Test Best Practices


## Summary
The topics below will cover or provide references to best practices for designing unit tests.

## Testing Lifecycle
Please refer to the Dynamics 365 FastTrack team guidance located in the references below:
### References
https://github.com/microsoft/Dynamics-365-FastTrack-Implementation-Assets/blob/master/Customer%20Service/Testing/Strategy/Documentation/Testing%20Lifecycle.docx

## Types of Unit Tests
Please refer to the Dynamics 365 FastTrack team guidance located in the references below:
### References
https://github.com/microsoft/Dynamics-365-FastTrack-Implementation-Assets/blob/master/Customer%20Service/Testing/Strategy/Documentation/Types%20of%20Testing.docx

## Naming of Tests
Names of test should be descriptive. Avoid redundant terms like "test".
The name of your test should consist of three parts:

- The name of the method being tested.
- The scenario under which it's being tested.
- The expected behavior when the scenario is invoked.

An example of a Dynamics 365 unit test name could be:
ApiMethod_Tool_ConnectionParameters_ScenarioRequest_ScenarioResponse
```
msdyn_InvokeIntelligenceAction_SDK_CrmServiceClient_SystemUser_IntentIdentification_VerifyResponse
```
### References
[Naming Tests](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices#naming-your-tests)

