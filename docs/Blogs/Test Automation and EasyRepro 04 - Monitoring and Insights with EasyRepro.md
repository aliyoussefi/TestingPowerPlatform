# Test Automation and EasyRepro: 04 - Monitoring and Insights with EasyRepro

## Summary

In this article we will discuss monitoring execution timings and delivering these to Application Insights. We will look into the various timings within an execution, how to add tracing statements and finally to push events to Application Insights.

## Getting Started

If you haven't already, please review the previous articles showing how to create, debug and extend EasyRepro. This article assumes that the reader has this knowledge and will focus specifically on monitoring.

## Command Timings

Commands in EasyRepro unit tests represent an interaction with the browser to perform an action. Each command has a name, a start time, end time and other properties such as how many times it was attempted and was it successful. What's returned to the unit test is a list of command results represented by the **ICommandResult** object.

![](C:\Data\_IPDev\Blogs\Blog4\XrmApp.CommandResults Definition.JPG)

Going to the definition for **ICommandResult** we see the following properties:

<img src="C:\Data\_IPDev\Blogs\Blog4\ICommandResult definition.JPG" style="zoom:50%;" />

These properties represent pretty much everything we need to know about a command result. However since this is a interface, it can be extended to include any other metric needed. Now that we have the definition we need to dig deeper to better understand how these properties are populated. But before we get into that let's review the object that implements the **ICommandResult** interface: the **BrowserCommandResult** class.

### **Exploring the BrowserCommandResult class**

![](C:\Data\_IPDev\Blogs\Blog4\BrowserCommandResult Definition.JPG)

The image above shows the constructor for the BrowserCommandResult class and three important attributes that represent collecting the start and stop time stamp as well as the value returned. 

### Exploring the BrowserCommand.cs file

The **BrowserCommand** object is where the actual command gets executed. It contains options describing tracing, retry policies, exception policy, etc. What's most important is the **Execute** method which performs the action from the unit test.

![](C:\Data\_IPDev\Blogs\Blog4\BrowserCommand-Execute Definition.JPG)

When a command from a unit test is run (i.e. GetValue), it ultimately comes to this method to be performed. In the image above you can see the retry attempts are set as well as the tracing and timing (result.Start()). Once the command has finished the result and timings are returned in the **BrowserCommandResult** object.

### Execution Times

The execution time for a command represents how long the command took to execute as shown in the Execute method in the BrowserCommand class result.Start and result.Stop methods. Since we are focusing on timings these two methods are key. They are used to calculate the execution time we will use to monitor the performance of our unit test. These methods are shown in the BrowserCommandResult image above and return a simple timestamp.

<img src="C:\Data\_IPDev\Blogs\Blog4\BrowserCommandResult.ExecutionTime Calculation.JPG" style="zoom:70%;" />

The execution time calculation takes the StopTime property and substracts from the StartTime to provide a number in milliseconds. This number is then set to the return value of our unit test command.

<img src="C:\Data\_IPDev\Blogs\Blog4\Login BrowserCommandResult - Results.JPG" style="zoom:67%;" />



### Think and Transition Times

You may have noticed above a couple of values for Think and Transition Times. 

<img src="C:\Data\_IPDev\Blogs\Blog4\OpenGridArea BrowserCommandResult - Think and Transition Times.JPG" style="zoom:67%;" />

**Think times** are built in mechanisms to put a thread to sleep. They can be used to allow actions to complete or represent a user wait in the UI. In the image above we see that the ThinkTime value is set to 2000 which is because inside of the OpenApp method the Browser.ThinkTime method is run. 

The **Transition time** value represents the time from the previous command end to the current command's start time and removing think times in between.

<img src="C:\Data\_IPDev\Blogs\Blog4\OpenSubArea BrowserCommandResult - Think and Transition Times.JPG" style="zoom:67%;" />

#### How is all of this calculated?

![](C:\Data\_IPDev\Blogs\Blog4\InteractiveBrowser-CalculateResults function.JPG)

Inside of the **InteractiveBrowser** class is a method called **CalculateResults** which is called from the Execute method described above. In the image above you can see think time is straight forward, it comes from the **CommandThinkTimes** property.

The transition time is a calculation adds the seconds and millisecond differences between the previous command and current command then subtracts the think time. All three of these timings can help us better understand where long running commands are coming from and where we could possibly tune unit tests by reducing think times between unit test commands.

## Logging and Monitoring

### Debug and Trace

The ability to provide output statements is essentially for any type of unit testing including ones built using the EasyRepro framework. Typically you see statements such as Console.WriteLine or Debug.WriteLine used in the unit test link in the image below.

![](C:\Data\_IPDev\Blogs\Blog4\Debug.WriteLine in Unit Tests.JPG)

Its important to note that these same lines can be added to the EasyRepro framework if you choose. Here are two images showing the output from the Debug window and the output from the test result itself.

<u>Debug window:</u>

![](C:\Data\_IPDev\Blogs\Blog4\Output Window Showing Debug.WriteLine statements.JPG)

<u>Test Result Window:</u>

![](C:\Data\_IPDev\Blogs\Blog4\Test Output Window.JPG)

You can see the test output shows only the WriteLine methods I added while removing the debug information from Visual Studio. Utilizing this technique is key to providing detailed information to the developer and testing teams to understand how a unit test is performing. This output will also be included in the test results file which will help us once we begin automating unit testing.

## Application Insights

Application Insights is an extensible Application Performance Management (APM) service that can be used to monitor applications, tests, etc. Source: [Application Insights Overview](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview)

In this section we will cover how to setup the Telemetry Client to be used by your unit tests and how to push command timings to Application Insights. Its assumed you have already setup an Application Insights resource and have the instrumentation key. For help on setting this up refer to [this article](https://docs.microsoft.com/en-us/azure/azure-monitor/learn/nodejs-quick-start) detailing enabling and configuring Application Insights.

### Setup Application Insights Telemetry Client

Begin by ensuring you have the **Microsoft.ApplicationInsights** NuGet package installed. As of this writing I'm using the v2.4.0 which is included in the **Microsoft.Dynamics365.UIAutomation.Sample** unit test project from GitHub.

![](C:\Data\_IPDev\Blogs\Blog4\Application Insights - NuGet.JPG)

Next, navigate to your app.settings and add the Instrumentation Key to the **AzureKey** key.

<img src="C:\Data\_IPDev\Blogs\Blog4\App.Settings.JPG" style="zoom:80%;" />

Once that's done we need to setup the **TelemetryClient** and give it our key. Once complete we are now ready to send messages to Application Insights.

![](C:\Data\_IPDev\Blogs\Blog4\Application Insights - TelemeteryClient.JPG)

### Push Timings to Application Insights

Once the unit test has completed its commands we are going to send the **CommandResults** collection to Application Insights. From the previous section we detailed that each command result includes the Start and End times as well as calculated timing such as execution, think and transition times. 

<img src="C:\Data\_IPDev\Blogs\Blog4\Application Insights - CommandResults.JPG" style="zoom:67%;" />

In the image above you can see how to loop through each command result and set the property and metric bags for the **CustomEvent** table in Application Insights. The telemetry client includes methods for each type of table in Application Insights so you can also include items like exceptions, navigation timings, etc in the appropriate table. Review [this document](https://docs.microsoft.com/en-us/azure/azure-monitor/app/api-custom-events-metrics) for additional information.

## Next Steps

### Expanding messages sent to Application Insights

The example above shows how to collect and send timings for each command to Application Insights which is very useful to understand how the test and platform are performing. In addition you may want to think about adding additional contextual details about each unit test run such as who is running the test, where its running and information about the org that the test is running on. As you can see the property and metric bags are dictionaries that can be added to to include these detail points. There are also table columns native to each specific table that can be leveraged as well to house these data points.

One area of the platform that gets overlooked is the **Performance Center** which is extremely useful for understanding page load timings. These markers are accurate representations of a page load. They can be extracted and sent to Application Insights as well. Check out the **SharedTestUploadTelemetry** sample unit test for more information. This test uses the legacy UI however EasyRepro can be extended to utilize the Performance Center in the unified interface as well. Please refer to the previous section for extending EasyRepro for techniques to accommodate this.

## Conclusion

In this article we have explored EasyRepro commands and how the timings for each command are gathered. We discussed logging techniques within our unit tests. We setup Application Insights within an EasyRepro unit test and push messages from our test. Finally we discussed adding additional data points to our messages.

Please refer to the the previous articles for extending and debugging EasyRepro unit tests. For the next article we will work to incorporate changes to EasyRepro unit tests into an Azure Build Pipeline using Continuous Integration.

