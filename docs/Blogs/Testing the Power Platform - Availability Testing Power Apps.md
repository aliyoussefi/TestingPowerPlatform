# Monitoring the Power Platform: Continuous Monitoring Power Apps with Availability Tests

## Overview

Continuous monitoring allows enterprises the opportunity to ensure reliable performant services are immediately available to its users. Platforms need to be able to keep up with demand in such a way that its seamless to a user. <u>Systems that become unreliable or unusable are quickly disregarded or abandoned</u>. One sure fire way to ensure uses won't use a service is if the service is unavailable. To account for this, enterprises looks to service level agreements and business continuity strategies. Part of this strategy includes testing for availability.

**Azure Application Insights** provides features to allow organizations to quickly report and take action on current and trending availability metrics. This article will review the various tests that can be configured within the service. From there we will go into how the data is and can be collected for analysis. We will look into a use case involving monitoring the **Dataverse API**. Finally, we wrap with implementing a monitoring strategy to assist with notifications and automation.

## Azure Application Insights Availability Tests

**Azure Application Insights** availability tests come in three distinct groupings. The first, **reaches out to a URL from different points around the world**. The second, allows for **the replay of a recorded user interaction with a site or web based service**. Both of these originate from within the **Azure Application Insights** feature itself, created in the **Azure Portal** or through the **Azure APIs**.

The final type of test is a completely custom test. This **custom test allows flexibility into how, what and where we test.** Due to these attributes, this type of test is ideal and will serve as the test strategy implemented below.

**<u>Important Note on web tests:</u>**

**<u>The web test mechanism has been marked as deprecated.</u>** As expected this announcement comes with various feedback. With this in mind, **I recommend avoiding implementing web tests**. If web tests are currently being used, look to migrate to custom tests.

## Building Ping Tests

**URL Ping Tests** with **Azure Application Insights** are tests that allow the service to **<u>make a request to a user specified URL</u>**. As documented, this test doesn't actually use ICMP but sends an HTTP request allowing for capturing response headers, duration timings, etc.

For the **Power Platform**, this can be useful for testing availability of **Power Apps Portals** or services utilizing **Power Virtual Agents** or custom connectors. When configuring the test, conditions can be set for the actual request to the URL. These include **the ability to include dependent requests (such as images needed for the webpage) or the ability to retry on an initial failure**.

The **<u>test frequency can be set to run every five, ten or fifteen minutes</u>** from various **Azure Data Centers** across the globe. [Its recommended to test from no fewer than five locations, this will help diagnose network and latency issues.](https://docs.microsoft.com/en-us/azure/azure-monitor/app/monitor-web-app-availability#create-a-url-ping-test)

Finally, [the referenced documentation](https://docs.microsoft.com/en-us/azure/azure-monitor/app/monitor-web-app-availability#create-a-url-ping-test) recommends that **the optimal configuration for working with alerts is to set the number of test locations equal to the threshold of the alert plus two**.

## Building Custom Tests

Continuing down the path of availability tests, the need to expand beyond URL ping tests will eventually come up. Situations such as **validating not only uptime but authentication, latency, service health, etc. all could benefit from custom availability tests**.

Before building custom tests, let's look take a closer look at the Availability table within **Azure Application Insights**.

### The Availability Table

The availability table is where our test telemetry will reside, either from URL ping tests or custom testing. The table is designed to allow for capturing if a test was **successful, the duration (typically captured in milliseconds with a stopwatch approach), location of the test and other properties**. I'll review this in depth further in the article but for now keep in mind at a minimum we want to capture success, timing and location for each test.

### Testing Power Apps are launching correctly
Many organizations have an immense catalog of user created Power Apps used for multiple business purposes. These apps need a high level of uptime and need to respond quickly to provide users a reliable platform to perform their work. Uptime is essential for a reliable and performant solution. As we build towards operational excellence, we must continue to find ways to validate and report on all of our enterprise apps, their integrations, etc and be able to respond quickly to change.

For Power Apps, typically a user will open the app from their desktop or mobile device. Considering the mobile aspect that user could be accessing the app from any point in the world. As such, organizations must be able to run synthetic tests globally. Many tools exist and will come that can help to simulate these tests. What we must focus on is not what tools we use but what must haves traits our strategy must adhere to.

These must haves include guaranteed checks on uptime including the hosting platform and reducing any risks in tooling, the ability to deliver accurate reports, the ability to distinguish synthetic tests versus actual outages

### Identifying the tools and platforms needed for guaranteed checks
When choosing a tool or multiple, look for attributes such as supportability, interoperability and maintainability. Ideally, a tool exists that meets these standards, if not, part of the strategy is to account for this and reduce risk. 

Test Studio is a tool that requires no ownership of code, we simply call a URL and analyze the response. Test Studio tests allow us to monitor when the test starts. We can also add tracing statements.

Test Engine allows

### What's in a test
Our tests must be able to tell us accurately if our app is up and running and ideally reporting on standard business requirements like app load times. The tests don't need to validate the app is working correctly, that's a different kind of test so keep the test short and sweet. It simply needs to tell what we want and that's it.

### Synthetic testing and actual outages
The reporting tooling must be able to distinguish between a test and a real user having an issue. This traditionally will come from the test tool which will identify its tests as simulations where real user interactions will come from the app.

## Power App Availability Testing
- Identify mission critical apps
- Build tests for mission critical apps
- Test mission critical apps globally

### Identifying missing app availability tests


### Building

### Testing Globally for Availability and Latency
In this example, we are looking to test availability from multiple spots worldwide. We are looking to test not only that Power Apps are opening up but how long it takes to open the app.

The below image shows that we will deploy to an App Service allowing for Azure Web Jobs or Azure Functions to execute the tests.
![Alt text](../artifacts/AvailabilityTests/AzureWorldMapDeployed.jpg)

Leveraging the Availability Tests message within Azure Application Insights, we can populate