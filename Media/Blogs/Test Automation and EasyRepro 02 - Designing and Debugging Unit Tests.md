# **Test Automation and EasyRepro: 02 - Designing and Debugging Unit Tests**

## Summary

EasyRepro is a framework allowing automated UI tests to be performed on a specific Dynamics 365 organization. This article will focus on designing and debugging unit tests. It will follow up on the first post's sample unit test in depth as well as provide design and troubleshooting ideas.

## Getting Started

If you haven't already, check out the first article which covers getting familiar with EasyRepro and working with the bits locally. It covers cloning from GitHub to DevOps then locally and reviewing setting up dependencies and test settings to run a simple test. 

## Reviewing the Open Account Sample Unit Test in depth

![1565014162322](C:\Users\alyousse\AppData\Roaming\Typora\typora-user-images\1565014162322.png)

When working with the sample unit tests you'll soon find that while extremely helpful you'll need to modify or extend these tests to work with your customizations of the platform. The sample unit tests provide a great starting off point and can help us better understand how to work with the EasyRepro framework. Let's review the **UCITestOpenActiveAccount** unit test line by line.

### WebClient and XrmApp objects

The **WebClient** is derived from the **BrowserPage** class and injected into the **XrmApp** object. The **WebClient** contains mainly internal methods used by the **XrmApp** and platform element references such as OnlineLogin, Navigation, Grid, etc as shown in the unit tests above. Typically you will not be interacting with this object until you need to extend the framework.

The **XrmApp** is the primary way of navigating and commanding the platform. When opened you'll see the **WebClient** passed into each object used in the unit tests. Review this object to better understand each of the area of the platform you can work with in your unit tests.

![](C:\Data\_IPDev\Blogs\Blog2\XrmApp.JPG)

 As you can see, each of the commands in the Open Account unit test are represented here. Each element can be explored to determine what functionality can be achieved from the framework natively.

### Commands

At this point let's take a moment to review each one of the commands in the Open Account unit tests so we can better understand what they are doing and where we may need to augment.

#### Login

![](C:\Data\_IPDev\Blogs\Blog2\Login.JPG)

The **Login** method typically involves two paths: One using the standard Microsoft Office login and one that redirects to your organization's sign in page.

The first line shows passing in the URI of the organization, the username and password. How to set these values is covered in the first post in this series **Test Automation and EasyRepro: 01 - Overview and Getting Started**.

The second shows how to incorporate your organization's sign in page. Each sign in page is unique and will require understanding how to work with Selenium and the DOM of the page to input and submit these credentials. I'll cover this in the **Designing Tests towards Customizations** section.

Once the **Login** method has completed we will redirected to our default application in the organization. The next command details moving to another UCI application.

#### Open UCI App

![](C:\Data\_IPDev\Blogs\Blog2\OpenApp.JPG)

The **OpenApp** method uses the **UCIAppName** class to navigate to a specific UCI app. The standard platform applications such as Sales Hub and Customer Service Hub, as well as others, are available in the class. I'll cover how to extend to a custom UCI app in the **Designing Tests towards Customizations** section.

#### Open Sub Area

![](C:\Data\_IPDev\Blogs\Blog2\OpenSubArea.JPG)

**OpenSubArea** introduces are first navigation into the sitemap of your UCI application. This method expects two parameters, the first identifying which area to open and the second which subarea to click. These string values are case sensitive.

#### Search

![](C:\Data\_IPDev\Blogs\Blog2\Search.JPG)

The **Search** method is used on the Grid object to search using the Quick Find view. Wildcard characters can be added here to simulate a user looking for values like or starting with specific characters if desired. For out unit test as long as we have the sample data in the organization we should pull up the Adventure Works  (sample) record in the result set.

#### Open Record

![](C:\Data\_IPDev\Blogs\Blog2\OpenRecord.JPG)

The final navigation command in the unit test is to open the first record in the Grid using **OpenRecord**. As long as we pulled up the Adventure Works (sample) in our results this will open the first record based on the index of the rows in the view. If the index is outside of the bounds an exception will be thrown.

#### Think Time

![](C:\Data\_IPDev\Blogs\Blog2\ThinkTime.JPG)

Finally we have reached the end of the unit test. The final command here is a method called **ThinkTime**. This is used to simulate a user waiting for a period of time, in this case 3 seconds. This is a useful method to  allow elements to render for use with the framework.

## Designing Tests towards Customizations

### Adding custom UCI applications

In our sample unit test we are navigating to the Sales Hub UCI application but there maybe times where you will need to navigate to a specific custom application. A simple way to do so, assuming the UCI app shows in the user's drop down when logged in, is to add to the UCIAppName class.

![](C:\Data\_IPDev\Blogs\Blog2\UciAppName.JPG)

In the example above I've added a new string to the class and labeled it PfeCustomApp. The **OpenApp** method will search for an UCI app with called 'PFE Custom App'. This can then be used in single or many unit tests.

### Searching and reviewing results in a Grid

In our sample unit test we knew what to expect due to sample data in our organization. However when implementing your own unit tests you may want perform additional tasks like counting the number of results are returned, sort the records in the view, switch the view, etc.

![](C:\Data\_IPDev\Blogs\Blog2\Grid-ObjectBrowser.JPG)

Above is an image from the Object Browser showcasing the native EasyRepro functionality for the Grid object. To Search use the **Search** method. For reviewing results look to the **GetGridItems** method to extract the grid items which we can iterate through. 

![](C:\Data\_IPDev\Blogs\Blog2\Grid-GetGridItems.JPG)

### Changing Forms and Referencing Fields

Going back to the UCITestOpenActiveAccount unit test as described above we searched for the Adventure Works (sample) record in clicked on the first record in the Quick Find view. We just touched on actions that can be performed on the view, now let's look at what can be done once we are inside the record.

When the Adventure Works (sample) record is opened we now can shift gears to working with the Entity object. The Entity object includes functionality such as but not limited to:

- Assigning the record to another user or team
- Working with field values
- Working with Sub Grids
- Switching Forms
- Switching Processes

For now let's stick with changing forms and working with field values.

#### Switching Forms

Start by navigating to the **OpenRecord** method and adding a new line. Here we will add **SelectForm**("<name of your form>"). This works with the form selector and allows us to change the form based on the name.

![](C:\Data\_IPDev\Blogs\Blog2\SelectForm.JPG)

#### Working with Field Values

When working with most fields  on the form the actions a user will perform are to clear out a value, read a value or update a value.  In the below image I've added a line using the **GetValue** method. This returns me the current value which can be used for validation or other concerns. **When referencing a field it is important to reference by its schema name and not display name.** For instance on Account instead of using 'Account Name' I'm using 'name'.

![](C:\Data\_IPDev\Blogs\Blog2\GetFieldAndGetValue.JPG)

To clear out a value you simply have to use **ClearValue** and pass in the schema name.

![](C:\Data\_IPDev\Blogs\Blog2\ClearValue.JPG)

Finally to set a value you use the **SetValue** method passing in the schema name of the control (unless working with a complex type field).

![](C:\Data\_IPDev\Blogs\Blog2\SetValue.JPG)



## Debugging Tests

When you begin to extend and debug the sample unit tests towards your customization you may run questions regarding how the test is running, who its running as, how to handle unforeseen issues, etc. For this section we will talk through some common scenarios that come up and how to address them.

### My Unit Test is performing unexpected actions, what can I do?

One of the key benefits of browser automation is that we can watch the unit test in action, halt the execution, and examine the current state of the platform. One of the first steps in doing this is to run the unit test in what's called **Non Headless Mode**.

#### Running Unit Tests in Non Headless Mode

Headless Mode is what tells the unit test if we want to run the test with a GUI or not. Simply stated this will allow us to watch the browser launch and perform the actions or simply run in the background.

In the first post in this series we touched on the **TestSettings** object and how it tells our unit test how we want it to run. In this object is the class called **BrowserOptions** and within this is a property called **Headless**. To turn headless mode on or off, set this value to true (on) or false (off).

![](C:\Data\_IPDev\Blogs\Blog2\BrowserOptions.JPG)

### My Unit Test needs to run as a specific user, what can I do?

By default to run our EasyRepro unit tests (at least for the online platform) we must provide some credentials that determine who we run our tests as. This helps us identify and troubleshoot issues related to authentication and authorization. However sometimes due to your organization's setup you may have pass through authentication which will attempt to run the unit tests as the user you are logged into your machine with.

#### Running Unit Tests in Incognito Mode

Going back to the **TestSettings** object and inside of **BrowserOptions** we find a property called **PrivateMode**. By setting this property to true we can run in "Incognito" or "InPrivate" or whateevr terminology your browser of choice uses. This helps us ensure we are using the credentials we provided as well as not assuming any cached settings are applied.

### My Unit Test needs to check form or grid values, what can I do?

If you have worked with the Visual Studio Unit Testing tools in the past you are probably familiar with the **Assert** class. This class allows a test designer to add assertions to ensure the actions performed are accurate. EasyRepro provides us the ability to check for form or grid values and elements which can then be used to with the Assert class.

#### Using the Assert class

Below is an example of checking if the number of results from our search is not equal to a single result. This could be helpful to determine if duplicates exist as well as other scenarios.

![](C:\Data\_IPDev\Blogs\Blog2\Assert.JPG)

Reference: 

https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.testtools.unittesting.assert?view=mstest-net-1.3.2



## Next Steps

### Exploring the Sample Unit Tests

At this point you should have enough experience to begin exploring the other sample unit tests available in the **Microsoft.Dynamics365.UIAutomation.Sample** project. These tests show how to interact with popular elements like Business Process Flows, Quick Create forms and Command Bar actions.

Each element has its own unique capabilities and should cover most of what you will need to automate testing in the platform. However you may come across a need to extend EasyRepro to account for specific use cases which we will cover in the next post in the series **Extending and Working with XPath**.

### 