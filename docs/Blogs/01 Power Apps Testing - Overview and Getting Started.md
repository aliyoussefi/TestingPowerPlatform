# Power Apps Testing - Overview and Getting Started

## Overview

Continuous monitoring allows enterprises the opportunity to ensure reliable performant services are immediately available to its users. Platforms need to be able to keep up with demand in such a way that its seamless to a user. <u>Systems that become unreliable or unusable are quickly disregarded or abandoned</u>. One sure fire way to ensure uses won't use a service is if the service is unavailable. To account for this, enterprises looks to service level agreements and business continuity strategies. Part of this strategy includes testing for availability.

**Microsoft Power Apps** provide multiple tools to test and monitor application usage, from launching an app to navigating within an app. We will look into building a test strategy for **Microsoft Power Apps.** We will cover tools available, defining and building test, distinguishing tests versus actual usage telemetry and running automated tests.

### Testing Power Apps are launching correctly
Many organizations have an immense catalog of user created **Power Apps** used for multiple business purposes. These apps need a high level of uptime and need to respond quickly to provide users a reliable platform to perform their work. Uptime is essential for a reliable and performant solution. As we build towards operational excellence, we must continue to find ways to validate and report on all of our enterprise apps, their integrations, etc and be able to respond quickly to change.

For **Power Apps**, typically a user will open the app from their desktop or mobile device. Considering the mobile aspect that user could be accessing the app from any point in the world. As such, organizations must be able to run synthetic tests globally. Many tools exist and will come that can help to simulate these tests. What we must focus on is not what tools we use but what must haves traits our strategy must adhere to.

These must haves include _guaranteed checks on uptime including the hosting platform and reducing any risks in tooling_, the ability to _deliver accurate reports_, the ability to _distinguish synthetic tests versus actual outages._

### Identifying the tools and platforms needed for guaranteed checks
When choosing a tool or multiple, look for attributes such as **supportability**, **interoperability** and **maintainability**. Ideally, a tool exists that meets these standards, if not, part of the strategy is to **account for  risk. **

### What's in a test
Our tests must be able to tell us accurately if our app is up and running and ideally reporting on standard business requirements like app load times. The tests don't need to validate the app is working correctly, that's a different kind of test so keep the test short and sweet. It simply needs to tell what we want and that's it.

### Synthetic testing and actual outages
The reporting tooling must be able to distinguish between a test and a real user having an issue. This traditionally will come from the test tool which will identify its tests as simulations where real user interactions will come from the app.

## Power App Availability Testing
- Identify mission critical apps
- Build tests for mission critical apps
- Test mission critical apps globally

## Identifying missing app availability tests
The [_Center of Excellence_](https://learn.microsoft.com/en-us/power-platform/guidance/coe/overview) does a great job of cataloging apps used by the enterprise. I suggest looking into the data points collected from the COE coupled with the canvas app table within your production **Dataverse** instance.
To view the canvas apps within the instance, make a request to the **Dataverse** API like below:
https://<environment>.crm.dynamics.com/api/data/v9.2/canvasapps

From here, ensure test coverage within your test script repository. One way to achieve this is to list all test suites and cases in a repo and compare. A sample workflow is listed below.
- Loop through all canvas apps from the Dataverse API
- For each canvas app, locate the appropriate test suite
- If missing, create a work item to create test suite

As we progress, we will automate this but for now this approach will work.

## Building tests
There are three ways tests can be built for Power Apps:
- Using Test Studio direct URL or downloaded Power Fx Yml test
- Defining a Power Fx Yml test manually
- Building a UI test with EasyRepro

### Building and executing Test Studio tests
**Test Studio**, a tool that requires no ownership of code, we simply call a URL and analyze the response. Test Studio tests allow us to monitor when the test starts. [We can also add tracing statements.](https://learn.microsoft.com/en-us/power-apps/maker/canvas-apps/working-with-test-studio) --Note: This link contains a video showing how to use **Power Apps Test Studio**

A key call out as we build out the test cases and suite for the **Power App** is the [test setup and breakdown properties.](https://learn.microsoft.com/en-us/power-apps/maker/canvas-apps/working-with-test-studio#setup-your-tests) These allow us to add code prepping tests, add telemetry or other functionality. The properties include:
- OnTestCaseStart
- OnTestCaseComplete
- OnTestSuiteComplete

![](https://learn.microsoft.com/en-us/power-apps/maker/canvas-apps/media/working-with-test-studio/ontestcasestart-example.png)

**Test Studio** allows us to download the **Power Fx** yaml test suite. This can be done with the Download button below.

![](../artifacts/TestStudio/TestStudio_CommandBar_DownloadSuite.JPG)

Once downloaded, the yaml will look like the YAML provided in the Test Definition section.


A challenge here traditionally has been how to automate this test. Historically, the tool to use has been UI Automation using the [PowerAppsTestAutomation open source project.](https://learn.microsoft.com/en-us/power-apps/maker/canvas-apps/test-studio-classic-pipeline-editor) This allows us to automate the tests within CI/CD pipelines but does require dependencies that need to be dealt with. For assistance on understanding browser dependencies and techniques to overcome these challenges, refer to my video:

[EasyRepro - DevOps - Managing Browser Dependencies in Microsoft or GitHub Agents](https://www.youtube.com/watch?v=OOxboLnojMM)

NOTE: While labeled for **EasyRepro**, starting around the 6 minute mark you can follow the same technique within your Microsoft hosted agents.

With the introduction of **Power Apps Test Engine**, we can run this Power Fx test in a test engine bypassing the URL.


### Building and executing Test Engine tests
**Test Engine** allows organizations to submit yaml based tests to an executable that will run the **Power Fx** based yaml tests against a **Power App**. Review the read me and samples to see how to build both the test engine and tests.

To execute locally, you can simply clone local, build the executable and run the test as a scheduled task. That approach does require periodic pulls from the source to update the source code, and recompilation. I would not recommend this approach unless there is a specific SecOps requirement to keep everything on-premise. If that requirement does exist the following approach should meet the requirements if using a self hosted agent.

The solution below meets our requirements of finding a tool that meets our requirements of being supported and maintained by Microsoft. The solution assumes no ownership of code and will pull each time the test suite is set to run.
The solution will remote checkout **PowerApps-TestEngine**, build the engine within the agent and run a test. If the test fails, it will fail the test workflow allowing for a badge to be presented upon completion. By enforcing instrumentation to the app we are testing, we are able to see the test run as shown below.

![](https://raw.githubusercontent.com/aliyoussefi/TestingPowerPlatform/main/docs/artifacts/AvailabilityTests/PowerApps_PageViews_ms-isTestSetToFalse.JPG)

[The sample for this solution is located here.](https://github.com/aliyoussefi/TestingPowerPlatform/blob/main/.github/workflows/Clone-Build-Test.yml)

### Test Definition
```yml
testSuite:
  testSuiteName: Suite
  testSuiteDescription: Test Suite description
  persona: User1
  appLogicalName: ayw_canvasappdevelopmentworkshop_1d6fa
  appId: ''
  onTestCaseStart: |
    = 
    Trace("Test Case Started", TraceSeverity.Information, {
            TestStart: Text(Now())
        });
  onTestCaseComplete: |
    = 
    Trace("Test Case Complete", TraceSeverity.Information, {
             TestPass: TestCaseResult.TestCaseName & ":" & Text(Now())
             ,TestSuiteId: TestCaseResult.TestSuiteId
             ,TestSuiteName: TestCaseResult.TestSuiteName
             ,TestCaseId: TestCaseResult.TestCaseId
             ,TestCaseName: TestCaseResult.TestCaseName
             ,StartTime: TestCaseResult.StartTime
             ,EndTime: TestCaseResult.EndTime
             ,TestSuccess: TestCaseResult.Success
             ,TestTraces: JSON(TestCaseResult.Traces)
             ,TestFailureMessage: TestCaseResult.TestFailureMessage
    }
    );
  onTestSuiteComplete: |
    = 
    Trace("Test Suite Complete", TraceSeverity.Information, {
             TestSuiteId: TestSuiteResult.TestSuiteId
             ,TestSuiteName: TestSuiteResult.TestSuiteName
             ,StartTime: TestSuiteResult.StartTime
             ,EndTime: TestSuiteResult.EndTime
             ,TestPassCount: TestSuiteResult.TestsPassed
             ,TestFailCount: TestSuiteResult.TestsFailed
        }
    );
  networkRequestMocks: 
  testCases:
  - testCaseName: Availability Test
    testCaseDescription: ''
    testSteps: |
      = 
      Trace("App Launched");
testSettings:
  filePath: 
  browserConfigurations:
  - browser: Chromium
    device: 
    screenWidth: 0
    screenHeight: 0
  recordVideo: true
  headless: true
  enablePowerFxOverlay: false
  timeout: 30000
  workerCount: 10
environmentVariables:
  filePath: 
  users:
  - personaName: User1
    emailKey: user1Email
    passwordKey: user1Password

```

 Information on the test settings can be found on [the PowerApp-TestEngine repo.](https://github.com/microsoft/PowerApps-TestEngine/blob/main/docs/Yaml/test.md)
 Key call outs are the *locale* and *browser configurations* for **Test Settings**, *users* in **Environment Variables** and *test cases/steps* within the **Test Suite**.

## Next Steps
In this section, we covered the basics of a **Power Apps** test strategy. We reviewed tools available, tools to help us build tests and test configurations and definitions at a high level. Next, we will dive deeper into executing tests, evaluating responses and how to further expand our testing. We will also look at tools to to help us accelerate the discovery and creation of tests.

## References



