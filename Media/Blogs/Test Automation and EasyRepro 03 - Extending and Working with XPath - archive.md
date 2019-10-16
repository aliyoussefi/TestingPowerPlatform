# **Test Automation and EasyRepro: 03 - Extending and Working with XPath**

## Summary

EasyRepro is a framework allowing automated UI tests to be performed on a specific Dynamics 365 organization. This article will focus on reviewing how EasyRepro works with the Document Object Model otherwise known as the DOM. This will help us understand how we can extend the EasyRepro code for use with custom objects or areas of the platform not included. We will wrap with a brief look at XPath and referencing elements from the DOM.

## Getting Started

If you haven't already, check out the first article which covers getting familiar with EasyRepro and working with the bits locally. It covers cloning from GitHub to DevOps then locally and reviewing setting up dependencies and test settings to run a simple test. 

When working with the sample unit tests you'll soon find that while extremely helpful you'll need to modify or extend these tests to work with your customizations of the platform. While simply using string representations may be sufficient there may come a time where a reference document could help, such as in team development.

References to objects generally fall into four patterns:

1. Resolving via control ID
2. Resolving via XPath
3. Resolving by Element ID
4. Resolving by CSS Class

In this article we will focus on XPath which is what is primarily used in the EasyRepro framework for UCI.

## Working with XPath

XPath allows us to work quickly and efficiently to search for a known element or path within the DOM. Key functions include the contains method which allow to search node attributes. For instance when you review the DOM of a form you'll notice attributes such as data-id which is used quite a bit. Coupling this attribute with the html tag can result in a surprisingly accurate way to locate an element. I suggest leveraging the current element class as well as this LINK that goes into the schema of XPath.

Let's begin by going back to an earlier post where we discussed using EasyRepro and Selenium together...to extend the unit tests for custom sign in pages for your organization.

### Using XPath in the ADFS Redirect Login Method

![](C:\Data\_IPDev\Blogs\Blog2\AdfsLoginRedirect.JPG)

fff

## Understanding how to extend element references

### Building a reference for PowerApp Control Framework controls

For this exercise we will work to extend EasyRepro to work with custom PowerApps Control Framework (PCF) controls. 



#### Locating the App Element Reference File

ddd

#### Locating the Reference file

##### The Element Reference file

As of this writing the location of the reference file resides in the **ElementReference.cs** file within the UCI project.

![1567275690095](C:\Users\alyousse\AppData\Roaming\Typora\typora-user-images\1567275690095.png)

In this file you'll see two static classes: **Elements** and **References**. 

Elements:

![1567279624834](C:\Users\alyousse\AppData\Roaming\Typora\typora-user-images\1567279624834.png)

References:

![1567279740570](C:\Users\alyousse\AppData\Roaming\Typora\typora-user-images\1567279740570.png)

The References class contains classes representing platform objects such as Entities, Grids, Business Process Flows, etc. The properties in each object are used within your unit tests to interact with the user interface.

##### The App Element Reference file

The Unified Interface uses the **ElementReference.cs** file as well as another filed called **AppElementReference.cs**.

AppElementReference:

![1567280777029](C:\Users\alyousse\AppData\Roaming\Typora\typora-user-images\1567280777029.png)

You can see it uses a similar approach as classic with a reference and element object. The element object looks to only use XPath and we will follow a similar pattern when extending.

##### Connecting Reference to Element:

By now you can see that the reference object is used by the EasyRepro code to interact with the element class which in turn is used to search the document object model or DOM. Extending these will require updating both objects which we step through.

#### Adding to the Reference and Element classes

Start by adding an class to the **AppReference** object that will represent your particular extension. This will include static string properties that can be used by the **WebClient** object to ultimately interact with the platform. Review the current implementation for an example on how to build these objects.

Once complete add this to the **Xpath** dictionary in the **AppElements** class. This is a comma delimited key value pair consisting of the reference object property as a key and the **Xpath** command as the value.

Example of the Entity implementation:

![1567281719125](C:\Users\alyousse\AppData\Roaming\Typora\typora-user-images\1567281719125.png)







## Next Steps

### Conclusion

Let me know in the comments how your journey with EasyRepro is going or if you have any questions. Thanks!