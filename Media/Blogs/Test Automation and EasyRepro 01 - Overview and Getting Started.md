# **Test Automation and EasyRepro: 01 - Overview and Getting Started**

## Summary:

EasyRepro is a framework allowing automated UI tests to be performed on a specific Dynamics 365 organization. You can use it to perform testing such as Smoke, Regression, Load, etc. The framework is built from the Selenium web drivers used by the industry across a wide range of projects and applications. The entire EasyRepro framework is Open Source and available on GitHub. The purpose of this article is to walk through getting setup to work with the EasyRepro framework. It assumes you are familiar with concepts such as working with Unit Tests in Visual Studio, downloading NuGet packages and cloning repositories from GitHub.

## Getting Started

Now that you have a basic understanding of what EasyRepro is useful for you probably would like to start working with it. Luckily the framework is designed with flexibility and agility in mind so getting up and running is very simple. However as you know there are always a couple of hurdles to get over to begin working with EasyRepro. Let's start with dependencies! 

### Dependencies

The first dependencies involve the EasyRepro assemblies and the framework they are built upon which is Selenium. The second involve .NET, specifically the .NET framework (.NET core can be used and is included as a feature branch!). Finally depending on how you are working with the framework you will want to include a testing framework to design, build and run your unit tests.

### Downloading using Nuget Package Manager:

The quickest way to get started with the EasyRepro framework is to simply add a Nuget package reference to your unit test project. You can do by running this command in the Nuget Package Manager command line:

![1564863288683](C:\Users\alyousse\AppData\Roaming\Typora\typora-user-images\1564863288683.png)

Create your unit test project and navigate to the Nuget Package Manager CLI. Use the Install-Package command to get the PowerApps.UIAutomation.Api package as show in the command below (v9.0.2 is the latest as of this writing please refer to [this link](https://www.nuget.org/packages/PowerApps.UIAutomation.Api/) for any updates:

Install-Package PowerApps.UIAutomation.Api -Version 9.0.2

This will get you the references needed to begin working with the framework immediately. Once installed you should the following packages begin to download into your unit test project:

![1564952837073](C:\Users\alyousse\AppData\Roaming\Typora\typora-user-images\1564952837073.png)

Newtonsoft.Json

Selenium.WebDriver

Selenium.WebDriver

Selenium.Support

PowerApps.UIAutomation.Api

When complete the needed assemblies are available and you can begin working with the EasyRepro framework. There are some settings needed for the framework to connect to your PowerApps organization which if you're new to the framework maybe unknown. If so I would suggest reviewing the next section which initiates a clone of the EasyRepro framework which happens to include a robust amount of sample unit tests that show how to interact with the framework.

### Cloning from GitHub:

If you're new to the framework in my opinion this is the best way to begin familizaring yourself with the how it works and how to build simple and complex unit tests. This is also the way to go if you want to understand how the framework is built upon the Selenium framework and how to go about extending it. 

To begin go to the official EasyRepro project located at https://github.com/Microsoft/EasyRepro. Once you're there take a moment to review the branches available. The branches are structured in a GitFlow approach so if you're wanting to work with the latest in market release of PowerApps review the releases/* branches. For on going development I would suggest the develop branch.

Now that that out of the way let's clone the project locally so we can peek inside and see how things work. Another alternative which I highly recommend is to clone to an Azure DevOps project which can then be clones locally. This will allow us to automate with CI/CD which we will cover in another article. The gif below shows cloning to Azure DevOps but cloning locally directly from GitHub is also supported.

<insert Clone from GitHub to Azure DevOps (1).gif here>

#### Cloning locally from Azure DevOps:

If you decided to clone to Azure DevOps from GitHub the next step is to clone locally

<insert Clone locally from Azure DevOps.gif>

## Reviewing Sample Unit Tests

### Looking into the Open Account Sample Unit Test

Once you have the EasyRepro project cloned locally you should see various projects within a Visual Studio solution. Begin by taking a moment to review the Unit Test project titled **Microsoft.Dynamics365.UIAutomation.Sample**. Inside of this project lies hundreds of unit tests which can serve as a great learning tool to better understand how to work with the framework. I highly suggest exploring these tests when you begin to utilize the framework within your test strategy, many general and specific tasks are essentially laid out and can be transformed to your needs.

For this exercise we will open up the **UCITestOpenActiveAccount** unit test, you can find this using Find within Visual Studio (Ctrl+F). Once found you should see something like the following:

![1565014162322](C:\Users\alyousse\AppData\Roaming\Typora\typora-user-images\1565014162322.png)

Following the steps within the unit test you can see its designed to perform basic user actions to read an account. We start by logging into an organization. Then we proceed to open the UCI application titled "Sales". Once in the organization we open the Accounts sub area and search for "Adventure" in the Quick Find View. Finally we open the first record in the quick find view results.

#### Exploring Test Settings

In the current sample Unit Test project the test settings are set in two places: the **app.config** file located in the root of the project and in the **TestSettings.cs** file, a class object used across all of the tests.

##### Application Configuration file

The **app.config** file includes string configurations that tell the tests what organization to login to, who to login as and other under the hood settings like which browser to run and how to run the tests. For this article we will focus on simply running locally with the Google Chrome browser.

Inside of the **app.config** file are three settings we need to modify called **OnlineUsername**, **OnlinePassword** and **OnlineCrmUrl**. In my case I am using a trial and as you can see below I am using a "user@tenant.onmicrosoft.com" username and a "https://<orgname>.crm.dynamics.com/main.aspx" URL.

Before:

![1565014621572](C:\Users\alyousse\AppData\Roaming\Typora\typora-user-images\1565014621572.png)

After:

![1565015235915](C:\Users\alyousse\AppData\Roaming\Typora\typora-user-images\1565015235915.png)

##### Test Settings BrowserOptions object

Another key area is the **TestSettings** class and the various properties inside. These tell the unit tests how to render the browser, where the browser driver can be located as well as other properties.

![](C:\Data\_IPDev\Blogs\Blog2\BrowserOptions.JPG)

Some examples of settings here include determining what browser type to use, how to open the browser, how to run with a GUI or not as well as if to use a remote browser.

In the next post we will explore how these settings can change your experience working with unit tests and what options are available.

## Next Steps

### Conclusion

From this article you should be able to begin using EasyRepro with your PowerApps model driven apps immediately. The following articles will go into designing and debugging unit tests, extending the EasyRepro code, implementing with Azure DevOps and other topics. Let me know in the comments how your journey with EasyRepro is going or if you have any questions. Thanks!