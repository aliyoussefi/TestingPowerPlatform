# **Test Automation and EasyRepro: 01 - Overview and Getting Started**

## Summary:

EasyRepro is a framework that allow automated UI tests to be performed on a specific Dynamics 365 organization. You can use it to automate testing such as Smoke, Regression, Load, etc. The framework is built from the Open Source Selenium web drivers used by the industry across a wide range of projects and applications. The entire EasyRepro framework is Open Source and available on GitHub. The purpose of this article is to walk through the setup of the EasyRepro framework. It assumes you are familiar with concepts such as working with Unit Tests in Visual Studio, downloading NuGet packages and cloning repositories from GitHub.

## Getting Started

Now that you have a basic understanding of what EasyRepro is useful for you probably would like to start working with it. Getting EasyRepro Up and Running is very simple as the framework is designed with flexibility and agility in mind. However, like any other utility there is some initial learning and few hurdles to
get over to begin working with EasyRepro. Let's start with dependencies! 

### Dependencies

The first dependency involves the EasyRepro assemblies and the Selenium framework. The second involve .NET, specifically the .NET framework (.NET core can be used and is included as a feature branch!). Finally depending on how you are working with the framework you will want to include a testing framework to design, build and run your unit tests.

### Choosing How to Consume the EasyRepro Framework

There are two ways of consuming the EasyRepro framework, one is using the NuGet packages directly while the other is to clone or download from the GitHub repository. The decision to use one over the other primarily depends on your need to explore or extend the framework and how you go about doing so. Working directly with the source code allows exploration into how EasyRepro interacts with Dynamics 365. However for extending the framework the approach of using the NuGet packages and building on top allows for increased flexibility. 

### Downloading using NuGet Package Manager

The quickest way to get started with the EasyRepro framework is to simply add a NuGet package reference to your unit test project. You can do by running this command in the NuGet Package Manager command line:

![](https://raw.githubusercontent.com/aliyoussefi/D365-Testing/master/Media/Blogs/Blog1/1564863288683.jpg)

Create your unit test project and navigate to the NuGet Package Manager CLI. Use the Install-Package command to get the PowerApps.UIAutomation.Api package as show in the command below (v9.0.2 is the latest as of this writing please refer to [this link](https://www.nuget.org/packages/PowerApps.UIAutomation.Api/) for any updates:

```
Install-Package PowerApps.UIAutomation.Api -Version 9.0.2
```

This will get you the references needed to begin working with the framework immediately. Once installed you should the following packages begin to download into your unit test project:

![](https://raw.githubusercontent.com/aliyoussefi/D365-Testing/master/Media/Blogs/Blog1/1564952837073.png)

When complete the required assemblies are available and you can begin working with the EasyRepro framework. There are some settings needed for the framework to connect to your Dynamics 365 organization which if you're new to the framework maybe unknown. If so I would suggest reviewing the next section which initiates a clone of the EasyRepro framework which happens to include a robust amount of sample unit tests that show how to interact with the framework.

### Cloning from GitHub:

If you're new to the framework in my opinion this is the best way to begin familiarizing yourself how it works and how to build a wide range of unit tests. This is also the way to go if you want to understand how EasyRepro is built upon the Selenium framework and how to extend. 

To begin go to the official EasyRepro project located at https://github.com/Microsoft/EasyRepro. Once you're there take a moment to review the branches available. The branches are structured in a GitFlow approach so if you're wanting to work with the latest in market release of Dynamics 365 review the releases/* branches. For the latest on going development I would suggest the develop branch.

Start by cloning the project locally to review the contents and see how the interaction between the frameworks occurs. 

The gif below shows cloning to Azure DevOps but cloning locally directly from GitHub is also supported.

![]([https://raw.githubusercontent.com/aliyoussefi/D365-Testing/master/Media/Blogs/Blog1/Clone%20from%20GitHub%20to%20Azure%20DevOps.gif](https://raw.githubusercontent.com/aliyoussefi/D365-Testing/master/Media/Blogs/Blog1/Clone from GitHub to Azure DevOps.gif))

#### Cloning locally from Azure DevOps:

Another alternative which I highly recommend is to clone to an Azure DevOps project which can then be cloned locally. This will allow us to automate with CI/CD which we will cover in another article. If you decided to clone to Azure DevOps from GitHub the next step is to clone locally.

The gif below shows cloning locally from an Azure DevOps repository.

![]([https://raw.githubusercontent.com/aliyoussefi/D365-Testing/master/Media/Blogs/Blog1/Clone%20locally%20from%20Azure%20DevOps.gif](https://raw.githubusercontent.com/aliyoussefi/D365-Testing/master/Media/Blogs/Blog1/Clone locally from Azure DevOps.gif))

## Reviewing the EasyRepro Source Code Projects

The EasyRepro source code includes a Visual Studio solution with three class library projects and one for sample unit tests.

![](https://raw.githubusercontent.com/aliyoussefi/D365-Testing/master/Media/Blogs/Blog1/UIAutomation%20Solution%20and%20Projects.JPG)

The projects used by the Unified Interface are Microsoft.Dynamics365.UIAutomation.Api.UCI and Microsoft.Dynamics365.UIAutomation.Api.Browser. Most of the usage between EasyRepro and unit tests will happen with objects and commands within the Microsoft.Dynamics365.UIAutomation.Api.UCI project. This project contains objects to interact with Dynamics Unified Interface modules and forms. The Microsoft.Dynamics365.UIAutomation.Api.Browser project is limited to interacts with the browser driver and other under the hood components.

## Reviewing Sample Unit Tests

### Looking into the Open Account Sample Unit Test

The unit test project **Microsoft.Dynamics365.UIAutomation.Sample** contains hundreds of unit tests which can serve as a great learning tool to better understand how to work with the EasyRepro framework. I highly suggest exploring these tests when you begin to utilize the framework within your test strategy. Many general and specific tasks are essentially laid out and can be transformed to your needs. Examples include opening forms (**OpenRecord**), navigating  (**OpenSubArea**) and searching for records (**Search**), creating and updating records (**Save**).

For this exercise we will open up the **UCITestOpenActiveAccount** unit test, you can find this using Find within Visual Studio (Ctrl+F). Once found you should see something like the following:

<img src="https://raw.githubusercontent.com/aliyoussefi/D365-Testing/master/Media/Blogs/Blog1/1565014162322.png" style="zoom:80%;" />

Following the steps within the unit test you can see its designed to perform basic user actions to read an account. We start by logging into an organization (**Login**). Then we proceed to open the UCI application titled "Sales" (**OpenApp**). Once in the organization we open the Accounts sub area (**OpenSubArea**) and search for "Adventure" in the Quick Find View (**Search**). Finally we open the first record (**OpenRecord(0)**) in the quick find view results.

#### Exploring Test Settings

In the current sample Unit Test project the test settings are set in two places: the **app.config** file located in the root of the project and in the **TestSettings.cs** file, a class object used across all of the tests.

##### Application Configuration file

The **app.config** file includes string configurations that tell the tests what organization to login to, who to login as and other under the hood settings like which browser to run and how to run the tests. 

<u>Application Configuration File Settings</u>

| **Property**      | **Description**                                              |
| ----------------- | ------------------------------------------------------------ |
| OnlineUsername    | String. Used to represent the test user name.                |
| OnlinePassword    | String. Used to represent the test user password.            |
| OnlineCrmUrl      | String. Used to represent the organization (i.e. https://<your org>.crm.dynamics.com/main.aspx) |
| AzureKey          | String. GUID representation of Azure Application Insights Instrumentation Key. |
| BrowserType       | String. Represents enum flag for Microsoft.Dynamics365.UIAutomation.Browser.BrowserType. |
| RemoteBrowserType | String. Represents enum flag for Microsoft.Dynamics365.UIAutomation.Browser.BrowserType. Only used if BrowserType is Remote. |
| RemoteHubServer   | String. Represents Selenium Server remote hub URL. Only used if BrowserType is Remote. |

For this article we will focus on simply running locally with the Google Chrome browser by setting the **BrowserType** to "Chrome". Also inside of the **app.config** file are three settings we need to modify called **OnlineUsername**, **OnlinePassword** and **OnlineCrmUrl**. In my case I am using a trial and as you can see below I am using a "user@tenant.onmicrosoft.com" username and a "https://<orgname>.crm.dynamics.com/main.aspx" URL.

<u>Before:</u>

![](https://raw.githubusercontent.com/aliyoussefi/D365-Testing/master/Media/Blogs/Blog1/1565014621572.png)

<u>After:</u>

![](https://raw.githubusercontent.com/aliyoussefi/D365-Testing/master/Media/Blogs/Blog1/1565015235915.png)

##### Test Settings and the BrowserOptions object

Another key object is the **TestSettings** class and the various properties inside. This class tells the unit tests how to render the browser, where the browser driver can be located as well as other properties. The **TestSettings** class will need to be included in the Unit Test project and instantiate the **BrowserOptions** object as shown below:

![](https://raw.githubusercontent.com/aliyoussefi/D365-Testing/master/Media/Blogs/Blog1/BrowserOptions.JPG)

In the next post we will explore how these settings can change your experience working with unit tests and what options are available.

## Next Steps

### Conclusion

From this article you should be able to begin using EasyRepro with your Dynamics 365 organization immediately. The following articles will go into designing and debugging unit tests, extending the EasyRepro code, implementing with Azure DevOps and other topics. Let me know in the comments how your journey with EasyRepro is going or if you have any questions. Thanks!