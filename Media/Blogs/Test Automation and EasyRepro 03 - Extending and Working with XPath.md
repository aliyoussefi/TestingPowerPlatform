# **Test Automation and EasyRepro: 03 - Extending the EasyRepro Framework**

## Summary

EasyRepro is a framework allowing automated UI tests to be performed on a specific Dynamics 365 organization. This article will focus on reviewing how EasyRepro works with the Document Object Model otherwise known as the DOM. This will help us understand how we can extend the EasyRepro code for use with custom objects or areas of the platform not included. We will wrap with a brief look at XPath and referencing elements from the DOM.

## Getting Started

If you haven't already, check out the first article which covers getting familiar with EasyRepro and working with the bits locally. It covers cloning from GitHub to DevOps then locally and reviewing setting up dependencies and test settings to run a simple test. 

When working with the sample unit tests you'll soon find that while extremely helpful you'll need to modify or extend these tests to work with your customizations of the platform. 

Some examples include:

1. Using an ADFS Redirect Login
2. Working with custom components and resources
3. Working with strongly typed entity attributes
4. Including navigation and functionality not present natively in the framework



## Sample Scenarios for Extending

### Working with the Browser Driver

As we discussed earlier, your organization may have a sign on page that we will get redirected once we input our username. This is where we begin our journey into extending the framework and utilizing Selenium.

![](C:\Data\_IPDev\Blogs\Blog2\AdfsLoginRedirect.JPG)

The image above shows a final version of a method designed to search through a sign in page for input and submission. You will notice that we are working with the Driver natively and using methods not found in the sample unit tests such as **FindElement** and **WaitForPageToLoad**. I'll touch on these at a high level now.

#### FindElement

**FindElement** is part of the Selenium.WebDriver assembly and as you may have guessed is used to find an element in the DOM. I'll cover how to search through the DOM in the next post **Test Automation and EasyRepro: 03 - Extending and Working with XPath** but I wanted to take this time to show how we will begin extending.

Reference: 

https://www.toolsqa.com/selenium-webdriver/c-sharp/findelement-and-findelements-commands-in-c/

#### WaitForPageToLoad

**WaitForPageToLoad** is part of the EasyRepro framework as a Selenium extension. This is key to point out that we are leveraging both EasyRepro and Selenium natively to achieve our desired result to login. This method waits a specific amount of time and can be adjusted if needed.

#### SendKeys and Clear

**SendKeys** is used to send keystrokes to an element on the page. This can be a string or a single keypress. This can be used to send the username and password to your sign in page. It also can be used to send the Enter or Tab keypress to move to the next field or submit.

**Clear** as it sounds is used to remove any sort of input that may already exist in an element. This is useful if your sign in page attempts to automatically input credentials.

## Understanding how to extend element references

Considerations need to be made when designing unit tests to help reduce the maintenance work needed if something changes. For instance when referencing custom HTML web resources or even the ADFS Redirect from above, think how a change to an element could propagate across some of all of your unit tests. One way to control maintenance is to centralize commonly used references into proxy objects that can hide the underlying mechanics from the test designer. This is exactly how the EasyRepro framework handles references and when extending can leverage the same approach. In this section we will cover the files used by the framework to reference DOM elements and show how to extend them to include references to our custom login page.

### The App Element Reference File

The Unified Interface project uses the **ElementReference.cs** file as well as another file called **AppElementReference.cs**. The **ElementReference** file looks to have been brought over from the classic interface. What's unique about each is how they reference elements in the DOM which I'll cover in the next section. For now let's focus on reviewing the **AppElementReference.cs** file which is located in the **DTO** folder of the UCI project.

![](C:\Data\_IPDev\Blogs\Blog3\AppElementReferenceAndElementReferenceLocationInUciProject.JPG)

Inside of the **AppElementReference** file are two objects used by EasyRepro to represent and locate elements: The **AppReference** and **AppElements** classes.

### The AppReference class

The **AppReference** class includes sub classes that represent the objects used internally by EasyRepro, specifically the **WebClient** object, to connect to DOM elements. This allows the framework to standardize how the search for a particular container or element is performed. 

### The AppElement class

The **AppElement** class is a comma delimited key value pair consisting of the reference object property as a key and the **XPath** command as the value. The key represents the property name in the class object inside of **AppReference** while the value is the **XPath** location. 

I highly suggest reviewing the **AppElement** class when extending EasyRepro as it shows recommended ways to locate and reference elements on the DOM. In the next section we will discuss the different ways you can locate elements including **XPath**.

## Referencing elements in the Document Object Model

References to objects generally fall into four patterns:

1. Resolving via Control Name
2. Resolving via XPath
3. Resolving by Element ID
4. Resolving by CSS Class

In this article we will focus on XPath which is what is primarily used in the EasyRepro framework for UCI. However its key to understand each of the approaches for referencing as they can be used for customizations such as web resources, custom controls, etc.

### Resolve with Control Name

This will search the DOM for elements with a specific name which is an attribute on the element node. This is not used by EasyRepro to my knowledge.

### Resolve with Element ID

This will attempt to find an element by its unique identifier. For instance a textbox on the login form maybe identified as 'txtUserName'. Assuming this element ID is unique we could search for this particular element by an ID and return an IWebElement representation. An example from the UCI project is shown below showing usage with the timeline control.

<u>Definition:</u>

![](C:\Data\_IPDev\Blogs\Blog3\ElementReference-ElementId-Timeline-Definition.JPG)

<u>Usage by **WebClient**:</u>

![](C:\Data\_IPDev\Blogs\Blog3\ElementReference-ElementId-Timeline.JPG)

### Resolve with CSS Class

This allows the ability to search by the css class defined on the element. Be aware that this can return multiple elements due to the nature of css class. There is no usage of this in EasyRepro but again this could be helpful for customizations.

### Resolve with XPath

XPath allows us to work quickly and efficiently to search for a known element or path within the DOM. Key functions include the contains method which allow to search node attributes. For instance when you review the DOM of a form you'll notice attributes such as data-id which is used quite a bit. Coupling this attribute with the html tag can result in a surprisingly accurate way to locate an element. I suggest leveraging the current element class as well as this LINK that goes into the schema of XPath.

Let's begin by going back to an earlier post where we discussed using EasyRepro and Selenium together...to extend the unit tests for custom sign in pages for your organization.

### Using XPath in the ADFS Redirect Login Method

Going back to our original example for the ADFS login method below you'll see an example of referencing the DOM elements using XPath directly with the **Selenium** objects driver and **By.XPath**. Consider the below two images:

<u>Without a static representation of XPath:</u>

![](C:\Data\_IPDev\Blogs\Blog3\AdfsRedirect-XPath-WithoutAppReference.JPG)

<u>Using static classes to represent XPath queries:</u>

![](C:\Data\_IPDev\Blogs\Blog3\AdfsRedirect-XPath-WithAppReference.JPG)

Both of these methods work and perform exactly the same. However the second method provides increased supportability if and when the login page goes through changes. For instance consider if the id of the textbox to input your username changes from txtUserName to txtLoginId. Also what if this txtUserName XPath query is propagated across hundreds or more unit tests?

### Creating custom reference objects

Let's put what we have learned to use by creating a reference to our custom login page. Start by adding an class to the **AppReference** object and title it AdfsLogin. Inside this class declare string properties that will be used as input for your organization's login page. Typical inputs include username, password and a submit button. Here is an example of what mine looks like:

![](C:\Data\_IPDev\Blogs\Blog3\Login Class definition.JPG)

*NOTE: While this document demonstrates how to add to the **AppElementReference.cs** file I would suggest extending this outside of the core files as customizations will have to be merged with any updates from the framework.*

Once the static class we want to use in our unit tests has been created we now need to add the XPath references to the **AppElement** class. Below is an image showing the key value pair discussed in the **AppElement** section above. The key correlates to the string value of the AdfsLogin class while the value is the XPath directive for our elements. 

![](C:\Data\_IPDev\Blogs\Blog3\Login AppElement.JPG)

As shown in the image for Login_UserId we are searching the DOM for an input element with the id attribute of 'txtUserName'. XPath can be used to search for any attribute on the DOM element and can return a single value or multiple depending on what you're searching for. A good reference for XPath is here: 

## Next Steps

### Custom Components and Web Resources

PowerApps Control Framework controls and web resources both represent great options to learn and extend the EasyRepro framework to include your customizations. Try locating the container for the PCF controls and HTML web resources and work towards creating custom objects representing the elements for each as described above.

### Conclusion

In this article we discussed reasons why we will may need to extend the EasyRepro framework and some techniques in doing so. We explored working with Selenium objects and creating references to help us when creating unit tests. Finally we put this in an example for working with a ADFS Redirect page on login.

Thank you again for reading! Please let me know in the comments how your journey is going!